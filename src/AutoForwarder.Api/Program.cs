using AutoForwarder.Service.Interfaces;
using AutoForwarder.Service.Services;
using WTelegram;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserBotService, UserBotService>();

// WTelegramClient 
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new Client(key =>
    {
        return key switch
        {
            "api_id" => configuration["TelegramConfig:ApiId"],
            "api_hash" => configuration["TelegramConfig:ApiHash"],
            "phone_number" => configuration["TelegramConfig:PhoneNumber"],
            _ => null
        };
    });
});

// Logger
builder.Services.AddLogging();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
        