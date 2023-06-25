using MudBlazor.Services;
using PrometheusAPI;
using MongoDB.Driver;
using DeepTrace.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddHttpClient<PrometheusClient>(c => c.BaseAddress = new UriBuilder(builder.Configuration.GetValue<string>("Connections:Prometheus")!).Uri);

builder.Services
    .AddSingleton<IMongoClient>( s => new MongoClient(builder.Configuration.GetValue<string>("Connections:MongoDb") ))
    .AddSingleton<IDataSourceStorageService, DataSourceStorageService>()
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
