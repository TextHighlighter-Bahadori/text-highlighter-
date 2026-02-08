using ClojureHighlighter.Application;
using ClojureHighlighter.Application.FactoryMethods;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IClojureParserServiceFactory, ClojureParserServiceFactory>();
builder.Services.AddScoped<IClojureTokenizerServiceFactory, ClojureTokenizerServiceFactory>();
builder.Services.AddScoped<ISyntaxHighlighter, SyntaxHighlighterService>();

builder.Services.AddControllers();

var app = builder.Build();



app.MapControllers();
app.Run();