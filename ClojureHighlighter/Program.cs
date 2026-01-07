using ClojureHighlighter.Application;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IClojureParserService, ClojureParserService>();
builder.Services.AddScoped<IClojureTokenizerService, ClojureTokenizerService>();
builder.Services.AddScoped<ISyntaxHighlighter, SyntaxHighlighterService>();
builder.Services.AddScoped<IClojureTokenizerService, ClojureTokenizerService>();


var app = builder.Build();


app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.Run();