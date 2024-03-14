using BDO_Perform.WebApi.Infrastructure.OpenApi;
using NJsonSchema.Generation.TypeMappers;
using NSwag.Generation.Processors.Security;
using SmtpTester.Services;
using SmtpTester.Services.Interfaces;
using ZymLabs.NSwag.FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ISmtpTesterService, SmtpTesterService>();
builder.Services.AddScoped<FluentValidationSchemaProcessor>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument((document, serviceProvider) =>
{
    document.PostProcess = doc =>
    {
        doc.Info.Title = "SMTP Scout";
        doc.Info.Version = "V1.0.0";
        doc.Info.Description = "A simple web service that veries whether or not a given SMTP configuration is valid.\n\nWhen provided with a valid SMTP configuration, SMTP Scout will send an email to the specified 'To' addresses in the request.";
    };

    document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());

    document.SchemaSettings.TypeMappers.Add(new PrimitiveTypeMapper(typeof(TimeSpan), schema =>
    {
        schema.Type = NJsonSchema.JsonObjectType.String;
        schema.IsNullableRaw = true;
        schema.Pattern = @"^([0-9]{1}|(?:0[0-9]|1[0-9]|2[0-3])+):([0-5]?[0-9])(?::([0-5]?[0-9])(?:.(\d{1,9}))?)?$";
        schema.Example = "02:00:00";
    }));

    document.OperationProcessors.Add(new SwaggerHeaderAttributeProcessor());

    var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<FluentValidationSchemaProcessor>();
    document.SchemaSettings.SchemaProcessors.Add(fluentValidationSchemaProcessor);
});


var app = builder.Build();

app.UseStaticFiles();
app.UseOpenApi();
app.UseSwaggerUi(options =>
                {
                    options.DefaultModelsExpandDepth = -1;
                    options.DocExpansion = "none";
                    options.TagsSorter = "alpha";
                    options.CustomStylesheetPath = "swagger-ui.css";
                });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
