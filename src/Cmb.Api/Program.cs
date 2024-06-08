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

var builder = WebApplication.CreateBuilder(args);

// Kafka 
var kafkaOptions = GetConfigurationOnRun<KafkaOptions>();
builder.Services.AddKafka();
builder.Services.AddConsumer<CoffeeWasOrderedEvent, CoffeeWasOrderedEventHandler>(kafkaOptions);
builder.Services.AddProducer<CoffeeStartedBrewingEvent>(kafkaOptions);
builder.Services.AddProducer<CoffeeIsReadyToBeGottenEvent>(kafkaOptions);
builder.Services.AddProducer<OrderHasBeenCompletedEvent>(kafkaOptions);
builder.Services.AddProducer<OrderHasBeenFailedEvent>(kafkaOptions);

// Services
builder.Services.AddScoped<IngredientsService>();

builder.Services.AddSingleton<ICoffeePresenceChecker, FakeCoffeePresenceChecker>();
builder.Services.AddSingleton<IIngredientsSensor, FakeIngredientsSensor>();
builder.Services.AddSingleton<IRecipesSensor, FakeRecipesSensor>();
builder.Services.AddSingleton<OrderExecutionProcess>();

// Options 
builder.Services.Configure<CoffeeMachineConfiguration>(builder.Configuration);
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.Name));

builder.Services.AddDbContext<DbCoffeeMachineContext>(u => 
    u.UseInMemoryDatabase(DbCoffeeMachineContext.DatabaseName));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var ingredientsService = app.Services.GetRequiredService<IngredientsService>();
var config = app.Services.GetRequiredService<IOptionsMonitor<CoffeeMachineConfiguration>>();
await ingredientsService.Initiate(config.CurrentValue.Ingredients.Select(u => u.IngredientId).ToImmutableList());

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



#region Helpers

T GetConfigurationOnRun<T>() where T : IOptions
{
    var authOptions = builder.Configuration.GetSection(T.Name).Get<T>();
    if (authOptions is null)
        throw new ArgumentNullException($"Cannot get {nameof(T)}");
    return authOptions;
}

#endregion