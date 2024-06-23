using System.Collections.Immutable;
using Cmb.Application;
using Cmb.Application.Sensors;
using Cmb.Application.Sensors.Fakes;
using Cmb.Application.Services;
using Cmb.Common;
using Cmb.Common.Kafka;
using Cmb.Database;
using Cmb.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Kafka 
var kafkaOptions = GetConfigurationOnRun<KafkaOptions>();
builder.Services.AddConsumer(kafkaOptions.OrderEventsConsumer)
    .AddEvent<CoffeeWasOrderedEvent, CoffeeWasOrderedEventHandler>();
builder.Services.AddProducer(kafkaOptions.OrderEventsProducer);

// Services
builder.Services.AddScoped<IngredientsService>();

builder.Services.AddSingleton<ICoffeePresenceChecker, FakeCoffeePresenceChecker>();
builder.Services.AddScoped<IIngredientsSensor, FakeIngredientsSensor>();
builder.Services.AddSingleton<IRecipesSensor, FakeRecipesSensor>();
builder.Services.AddScoped<OrderExecutionProcess>();

// Options 
builder.Services.Configure<CoffeeMachineConfiguration>(builder.Configuration);
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.Name));

builder.Services.AddDbContext<DbCoffeeMachineContext>(u => 
    u.UseInMemoryDatabase(DbCoffeeMachineContext.DatabaseName), ServiceLifetime.Singleton);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();
var app = builder.Build();

if (app.Environment.IsDevelopment())
    await InitiateTestData();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



#region Helpers

async Task InitiateTestData()
{
    await using var scope = app.Services.CreateAsyncScope();
    var ingredientsService = scope.ServiceProvider.GetRequiredService<IngredientsService>();
    var config = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<CoffeeMachineConfiguration>>();
    await ingredientsService.Initiate(config.CurrentValue.Ingredients.Select(u => (u.IngredientId, u.SensorId)).ToImmutableList());
}

T GetConfigurationOnRun<T>() where T : IOptions
{
    var authOptions = builder.Configuration.GetSection(T.Name).Get<T>();
    if (authOptions is null)
        throw new ArgumentNullException($"Cannot get {nameof(T)}");
    return authOptions;
}

#endregion