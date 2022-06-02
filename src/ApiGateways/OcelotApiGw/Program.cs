using Ocelot.Cache.CacheManager;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
    })
    .ConfigureLogging((hostingContext, loggingBuilder) =>
    {
        loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        loggingBuilder.AddConsole();
        loggingBuilder.AddDebug();
    });

builder.Services
    .AddOcelot(builder.Configuration)
    .AddCacheManager(settings => settings.WithDictionaryHandle());

var app = builder.Build();

app.UseOcelot().Wait();

app.MapGet("/", () => "Hello World!");

app.Run();
