using ClojureHighlighter.Application;
using ClojureHighlighter.Application.FactoryMethods;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IClojureParserServiceFactory, ClojureParserServiceFactory>();
builder.Services.AddScoped<IClojureTokenizerServiceFactory, ClojureTokenizerServiceFactory>();
builder.Services.AddScoped<ISyntaxHighlighter, SyntaxHighlighterService>();
builder.Services.AddControllers();


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
        .AddOtlpExporter())
    .WithMetrics(metrics => metrics
        .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter());


builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
    logging.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(serviceName: builder.Environment.ApplicationName));
    logging.AddOtlpExporter();
});


var app = builder.Build();


app.MapControllers();
app.Run();