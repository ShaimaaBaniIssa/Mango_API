using Mango.GatewaySolution.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
//Ocelot is an Open Source.NET Core-based API Gateway designed specifically for microservices architectures that require centralised points of entry into their system.
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json",optional:false,reloadOnChange:true);
builder.Services.AddOcelot(builder.Configuration);

// needs to pass JWT tokens from web project to each service
builder.AddAppAuthentication();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseOcelot().GetAwaiter().GetResult();
app.Run();
