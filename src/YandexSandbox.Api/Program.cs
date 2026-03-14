using FluentValidation;
using YandexSandbox.Api.Filters;
using YandexSandbox.Bll.Interfaces;
using YandexSandbox.Bll.Services;
using YandexSandbox.Dal.Interfaces;
using YandexSandbox.Dal.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationActionFilter>();
});
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddSingleton<ICarRepository, InMemoryCarRepository>();
builder.Services.AddScoped<ICarService, CarService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

public partial class Program;
