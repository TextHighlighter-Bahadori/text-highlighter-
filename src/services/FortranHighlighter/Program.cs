using FortranHighlighter.Application;
using FortranHighlighter.Application.FactoryMethods;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IFortranParserServiceFactory, FortranParserServiceFactory>();
builder.Services.AddScoped<IFortranTokenizerServiceFactory, FortranTokenizerServiceFactory>();
builder.Services.AddScoped<ISyntaxHighlighterService, SyntaxHighlighterService>();



builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = 
            context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["timestamp"] = 
            DateTime.UtcNow;
    };
});

/*
 * by default the telemetry data are sent via Grpc protocol
 */
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(builder.Configuration["OTLP_Receiver_Endpoint"]!);
            options.Protocol = OtlpExportProtocol.Grpc;
        }))
    .WithMetrics(metrics => metrics
        .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(builder.Configuration["OTLP_Receiver_Endpoint"]!);
            options.Protocol = OtlpExportProtocol.Grpc;
        }));


builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
    logging.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(serviceName: builder.Environment.ApplicationName));
    logging.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri(builder.Configuration["OTLP_Receiver_Endpoint"]!);
        options.Protocol = OtlpExportProtocol.Grpc;
    });
});



var app = builder.Build();
app.MapControllers();
app.Run();