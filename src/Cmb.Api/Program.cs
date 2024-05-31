using Cmb.Application;
using Cmb.Application.Services;
using Cmb.Common;
using Cmb.Common.Kafka;
using Cmb.Database;
using Cmb.Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Kafka 
var kafkaOptions = GetConfigurationOnRun<KafkaOptions>();
builder.Services.AddKafka();
builder.Services.AddConsumer<CoffeeWasOrderedEvent, CoffeeWasOrderedEventHandler>(kafkaOptions);

// Services
builder.Services.AddScoped<IngredientsService>();
builder.Services.AddScoped<CoffeeRecipeService>();
builder.Services.AddSingleton<OrderExecutionProcess>();

// Options 
builder.Services.Configure<CoffeeMachineConfiguration>(builder.Configuration);
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.Name));

builder.Services.AddDbContext<DbCoffeeMachineContext>(u => 
    u.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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