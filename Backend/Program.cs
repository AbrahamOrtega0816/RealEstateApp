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
using Microsoft.Extensions.FileProviders;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Bind to platform-provided PORT when present (e.g., Railway/Render)
var portFromEnv = Environment.GetEnvironmentVariable("PORT");
if (int.TryParse(portFromEnv, out var portValue))
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(portValue);
    });
}

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
builder.Services.AddHttpContextAccessor(); // Required for generating full URLs
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IPropertyTraceService, PropertyTraceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMongoDbStartupService, MongoDbStartupService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IUrlService, UrlService>();

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

    // Configurar manejo de archivos y formularios multipart
    c.MapType<IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });

    c.MapType<IFormFile[]>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "array",
        Items = new Microsoft.OpenApi.Models.OpenApiSchema
        {
            Type = "string",
            Format = "binary"
        }
    });

    // Ignorar endpoints problemáticos temporalmente para que Swagger funcione
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Temporalmente excluir endpoints con IFormFile que causan problemas
        var actionName = apiDesc.ActionDescriptor.DisplayName ?? "";
        if (actionName.Contains("CreateOwner") || actionName.Contains("CreateProperty") || actionName.Contains("UpdateProperty"))
        {
            return false;
        }
        return true;
    });

    // Agrupar endpoints por controlador (a prueba de null)
    c.TagActionsBy(api =>
    {
        var group = api.GroupName;
        if (string.IsNullOrWhiteSpace(group))
        {
            api.ActionDescriptor.RouteValues.TryGetValue("controller", out group);
            group ??= "General";
        }
        return new[] { group };
    });

    // Ordenar endpoints alfabéticamente (a prueba de null)
    c.OrderActionsBy(apiDesc =>
    {
        apiDesc.ActionDescriptor.RouteValues.TryGetValue("controller", out var controller);
        controller ??= "General";
        return $"{controller}_{apiDesc.HttpMethod}";
    });

    // Evitar conflictos de nombres de esquemas si hay tipos con el mismo nombre
    c.CustomSchemaIds(type => type.FullName);

    // En caso de rutas duplicadas por convenciones, escoger la primera
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOriginsEnv = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");
        string[] allowedOrigins = string.IsNullOrWhiteSpace(allowedOriginsEnv)
            ? new[] { 
                "http://localhost:3000", 
                "http://localhost:3001",
                "https://real-estate-app-kappa-six.vercel.app" // Add your Vercel domain as fallback
              }
            : allowedOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        Console.WriteLine($"🌐 CORS: Allowing origins: {string.Join(", ", allowedOrigins)}");

        policy.WithOrigins(allowedOrigins)
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
var enableSwagger = app.Environment.IsDevelopment() ||
    string.Equals(Environment.GetEnvironmentVariable("ENABLE_SWAGGER"), "true", StringComparison.OrdinalIgnoreCase);
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Real Estate API V1");
        // In Development serve Swagger at root; in Production serve at /swagger
        c.RoutePrefix = app.Environment.IsDevelopment() ? string.Empty : "swagger";
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

// HTTPS redirection is disabled by default in containerized environments.
// Enable only when a valid certificate is configured and proxied TLS is handled.
var enableHttpsRedirect = string.Equals(
    Environment.GetEnvironmentVariable("ENABLE_HTTPS_REDIRECT"),
    "true",
    StringComparison.OrdinalIgnoreCase);
if (enableHttpsRedirect)
{
    app.UseHttpsRedirection();
}

// Servir archivos estáticos (imágenes)
app.UseStaticFiles();

// Mapeo explícito para /uploads -> wwwroot/uploads (robusto para producción)
var webRootPath = app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
var uploadsPhysicalPath = Path.Combine(webRootPath, "uploads");
Directory.CreateDirectory(uploadsPhysicalPath);
Console.WriteLine($"🗂️ Static files - WebRoot: {webRootPath} | Uploads: {uploadsPhysicalPath}");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPhysicalPath),
    RequestPath = "/uploads"
});

// Usar CORS
app.UseCors("AllowFrontend");

// Usar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores
app.MapControllers();

app.Run();
