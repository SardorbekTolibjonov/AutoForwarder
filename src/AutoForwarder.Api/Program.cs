using AutoForwarder.Service.Interfaces;
using AutoForwarder.Service.Services;
using WTelegram;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IUserBotService, UserBotService>();

// WTelegramClient 
builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    string sessionPath = Path.Combine(Directory.GetCurrentDirectory(), "wtelegram.session");

    return new Client(key =>
    {
        return key switch
        {
            "api_id" => configuration["TelegramConfig:ApiId"],
            "api_hash" => configuration["TelegramConfig:ApiHash"],
            "phone_number" => configuration["TelegramConfig:PhoneNumber"],
            "session_pathname" => sessionPath, // **SESSION FAYL UCHUN YO‘L**
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

var userBotService = app.Services.GetRequiredService<IUserBotService>();
userBotService.ForwardMessageAsync(default);

app.Run();
        