using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace RealEstateAPI.Configuration;

/// <summary>
/// Filter to handle IFormFile parameters in Swagger documentation
/// </summary>
public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParameters = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile) || 
                       p.ParameterType == typeof(IFormFile[]) ||
                       (p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>) && 
                        p.ParameterType.GetGenericArguments()[0] == typeof(IFormFile)))
            .ToArray();

        if (!fileParameters.Any())
            return;

        // Check if this is a multipart/form-data endpoint
        var hasFromFormAttribute = context.MethodInfo.GetParameters()
            .Any(p => p.GetCustomAttribute<FromFormAttribute>() != null);

        if (!hasFromFormAttribute)
            return;

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>(),
                        Required = new HashSet<string>()
                    }
                }
            }
        };

        var schema = operation.RequestBody.Content["multipart/form-data"].Schema;

        foreach (var parameter in context.MethodInfo.GetParameters())
        {
            var fromFormAttribute = parameter.GetCustomAttribute<FromFormAttribute>();
            if (fromFormAttribute == null)
                continue;

            var parameterName = parameter.Name ?? "file";

            if (parameter.ParameterType == typeof(IFormFile) || 
                (parameter.ParameterType.IsGenericType && parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>) && 
                 parameter.ParameterType.GetGenericArguments()[0] == typeof(IFormFile)))
            {
                schema.Properties[parameterName] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                };
            }
            else if (parameter.ParameterType == typeof(IFormFile[]))
            {
                schema.Properties[parameterName] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    }
                };
            }
            else if (parameter.ParameterType.IsClass && parameter.ParameterType != typeof(string))
            {
                // Handle complex types (DTOs)
                var dtoProperties = parameter.ParameterType.GetProperties();
                foreach (var prop in dtoProperties)
                {
                    var propName = prop.Name.ToLowerInvariant();
                    
                    if (prop.PropertyType == typeof(IFormFile) || 
                        (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && 
                         prop.PropertyType.GetGenericArguments()[0] == typeof(IFormFile)))
                    {
                        schema.Properties[propName] = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        };
                    }
                    else if (prop.PropertyType == typeof(IFormFile[]))
                    {
                        schema.Properties[propName] = new OpenApiSchema
                        {
                            Type = "array",
                            Items = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            }
                        };
                    }
                    else
                    {
                        // Handle primitive types
                        schema.Properties[propName] = GetSchemaForType(prop.PropertyType);
                    }

                    // Check if property is required
                    var requiredAttribute = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>();
                    if (requiredAttribute != null)
                    {
                        schema.Required.Add(propName);
                    }
                }
            }
            else
            {
                // Handle primitive types
                schema.Properties[parameterName] = GetSchemaForType(parameter.ParameterType);
            }
        }

        // Remove parameters that are handled by the request body
        operation.Parameters = operation.Parameters?.Where(p => 
            !context.MethodInfo.GetParameters().Any(param => 
                param.GetCustomAttribute<FromFormAttribute>() != null && 
                param.Name == p.Name)).ToList();
    }

    private static OpenApiSchema GetSchemaForType(Type type)
    {
        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType.Name switch
        {
            nameof(String) => new OpenApiSchema { Type = "string" },
            nameof(Int32) => new OpenApiSchema { Type = "integer", Format = "int32" },
            nameof(Int64) => new OpenApiSchema { Type = "integer", Format = "int64" },
            nameof(Decimal) => new OpenApiSchema { Type = "number", Format = "decimal" },
            nameof(Double) => new OpenApiSchema { Type = "number", Format = "double" },
            nameof(Single) => new OpenApiSchema { Type = "number", Format = "float" },
            nameof(Boolean) => new OpenApiSchema { Type = "boolean" },
            nameof(DateTime) => new OpenApiSchema { Type = "string", Format = "date-time" },
            nameof(DateOnly) => new OpenApiSchema { Type = "string", Format = "date" },
            _ => new OpenApiSchema { Type = "string" }
        };
    }
}
