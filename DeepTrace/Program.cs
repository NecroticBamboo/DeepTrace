using MudBlazor.Services;
using PrometheusAPI;
using MongoDB.Driver;
using DeepTrace.Services;
using DeepTrace.ML;
using ApexCharts;
using log4net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    GlobalContext.Properties["LOGS_ROOT"] = Environment.GetEnvironmentVariable("LOGS_ROOT") ?? "";
    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
    logging.AddLog4Net("log4net.config");
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddHttpClient<PrometheusClient>(c => c.BaseAddress = new UriBuilder(builder.Configuration.GetValue<string>("Connections:Prometheus")!).Uri);

builder.Services
    .AddSingleton<IMongoClient>( s => new MongoClient(builder.Configuration.GetValue<string>("Connections:MongoDb") ))
    .AddSingleton<IDataSourceStorageService, DataSourceStorageService>()
    .AddSingleton<IModelStorageService, ModelStorageService>()
    .AddSingleton<ITrainedModelStorageService, TrainedModelStorageService>()
    .AddSingleton<IEstimatorBuilder, EstimatorBuilder>()
    ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();
app.MapControllerRoute("default","{controller}/{action}");
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
