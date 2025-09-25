using RealEstateAPI.Configuration;
using RealEstateAPI.Features.Owners.Services;
using RealEstateAPI.Features.Properties.Services;
using RealEstateAPI.Features.PropertyTraces.Services;
using RealEstateAPI.Features.Authentication.Services;
using RealEstateAPI.Features.Shared.Services;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load .env file if it exists
try
{
    Env.Load();
    Console.WriteLine("✅ .env file loaded successfully");
}
catch (FileNotFoundException)
{
    Console.WriteLine("⚠️  .env file not found - using environment variables and appsettings.json");
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️  Error loading .env file: {ex.Message}");
}

// Configure MongoDB
var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
if (mongoSettings != null)
{
    // Replace placeholder with actual password from environment variable
    var mongoPassword = Environment.GetEnvironmentVariable("MONGODB_PASSWORD") 
                       ?? builder.Configuration["MONGODB_PASSWORD"] 
                       ?? throw new InvalidOperationException("MongoDB password not found in environment variables or configuration");
    
    mongoSettings.ConnectionString = mongoSettings.ConnectionString.Replace("{MONGODB_PASSWORD}", mongoPassword);
}

builder.Services.Configure<MongoDbSettings>(options =>
{
    if (mongoSettings != null)
    {
        options.ConnectionString = mongoSettings.ConnectionString;
        options.DatabaseName = mongoSettings.DatabaseName;
        options.PropertiesCollectionName = mongoSettings.PropertiesCollectionName;
        options.OwnersCollectionName = mongoSettings.OwnersCollectionName;
        options.PropertyTracesCollectionName = mongoSettings.PropertyTracesCollectionName;
        options.UsersCollectionName = mongoSettings.UsersCollectionName;
    }
});

// Configure JWT
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var secretKey = Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? throw new InvalidOperationException("JWT SecretKey is required"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Registrar servicios
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IPropertyTraceService, PropertyTraceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMongoDbStartupService, MongoDbStartupService>();
builder.Services.AddScoped<IImageService, ImageService>();

// Add controllers
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "Real Estate API", 
        Version = "v1",
        Description = @"API completa para gestión de propiedades inmobiliarias.",
        Contact = new()
        {
            Name = "Real Estate API Team",
            Email = "api@realestate.com"
        },
        License = new()
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
    
    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurar respuestas por defecto
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa el token JWT en el formato: Bearer {token}"
    });

    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Agrupar endpoints por controlador
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    c.DocInclusionPredicate((name, api) => true);
    
    // Ordenar endpoints alfabéticamente
    c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Verify MongoDB connection at startup
using (var scope = app.Services.CreateScope())
{
    var mongoStartupService = scope.ServiceProvider.GetRequiredService<IMongoDbStartupService>();
    await mongoStartupService.VerifyConnectionAsync();
}

// Configure logging for requests in development
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("🔵 REQUEST: {Method} {Path} {QueryString}", 
            context.Request.Method, 
            context.Request.Path, 
            context.Request.QueryString);
        
        await next();
        
        logger.LogInformation("🟢 RESPONSE: {StatusCode} for {Method} {Path}", 
            context.Response.StatusCode,
            context.Request.Method, 
            context.Request.Path);
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Real Estate API V1");
        c.RoutePrefix = string.Empty; // Swagger at root
        c.DocumentTitle = "Real Estate API - Documentación";
        c.DefaultModelsExpandDepth(2);
        c.DefaultModelExpandDepth(2);
        c.DisplayOperationId();
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
        c.SupportedSubmitMethods(new[] { 
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Get, 
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Post, 
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Put, 
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Delete,
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Patch
        });
        
        // Personalización visual
        c.InjectStylesheet("/swagger-ui/custom.css");
        c.InjectJavascript("/swagger-ui/custom.js");
    });
}

app.UseHttpsRedirection();

// Usar CORS
app.UseCors("AllowFrontend");

// Usar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores
app.MapControllers();

app.Run();
