using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load env name as string
var envName = builder.Environment.EnvironmentName;

string ocelotConfigFile = envName switch
{
    "Minikube" => "ocelot.Minikube.json",
    "Development" => "ocelot.Development.json",
    _ => "ocelot.json"
};

// Add the selected Ocelot config JSON file before AddOcelot
builder.Configuration.AddJsonFile(ocelotConfigFile, optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173") // React frontend URL
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

await app.UseOcelot();

app.Run();
