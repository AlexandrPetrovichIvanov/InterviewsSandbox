using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using YandexSandbox.Api;
using YandexSandbox.Api.Messaging;
using YandexSandbox.Bll.Configuration;
using YandexSandbox.Bll.Interfaces;
using YandexSandbox.Bll.Messaging;
using YandexSandbox.Bll.Services;
using YandexSandbox.Dal.Interfaces;
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
builder.Services.AddScoped<ICarService, CarService>();

builder.Services.AddSingleton<InMemoryOutboxStorage>();
builder.Services.AddSingleton<InMemoryMessageProducer>();
builder.Services.AddScoped<IMessageProducer, OutboxMessageProducerDecorator>();
builder.Services.AddHostedService<OutboxDispatcherService>();

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
