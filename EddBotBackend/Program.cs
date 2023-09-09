using EddBotBackend.Shared;
using EddBotBackend.EndPoints;

namespace EddBotBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            DiscordBotService discordBotService = new DiscordBotService(builder.Configuration);

            builder.Services.AddHostedService<DiscordBotService>(x => discordBotService);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapMkCatReqModelEndpoints(discordBotService);

            app.Run();
        }
    }
}
