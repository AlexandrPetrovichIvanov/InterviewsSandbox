using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using YandexSandbox.Api;
using YandexSandbox.Api.Messaging;
using YandexSandbox.Api.Messaging.Consume;
using YandexSandbox.Api.Messaging.Produce;
using YandexSandbox.Bll.Configuration;
using YandexSandbox.Bll.Interfaces.Messaging;
using YandexSandbox.Bll.Interfaces.Repositories;
using YandexSandbox.Bll.Interfaces.Services;
using YandexSandbox.Bll.Handlers;
using YandexSandbox.Bll.Services;
using YandexSandbox.Dal.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.Configure<CarValidationSettings>(
    builder.Configuration.GetSection("CarValidation"));
builder.Services.Configure<TopicSettings>(
    builder.Configuration.GetSection("Topics"));

builder.Services.AddSingleton<ICarRepository, InMemoryCarRepository>();
builder.Services.AddSingleton<IRentOrderRepository, InMemoryRentOrderRepository>();
builder.Services.AddScoped<ICarsService, CarsService>();
builder.Services.AddScoped<IRentOrdersService, RentOrdersService>();
builder.Services.AddScoped<IRentOrderProcessedMessageHandler, RentOrderProcessedMessageHandler>();

builder.Services.AddSingleton<InMemoryOutboxStorage>();
builder.Services.AddScoped<ICarCreatedMessageProducer, CarCreatedMessageProducerAdapter>();
builder.Services.AddScoped<IRentOrderPlacedMessageProducer, RentOrderPlacedMessageProducerAdapter>();
builder.Services.AddSingleton<IRentOrderProcessingConsumer, RentOrderProcessingConsumerAdapter>();
builder.Services.AddHostedService<RentOrderConsumerHostedService>();

var app = builder.Build();

app.UseBusinessExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

public partial class Program;
