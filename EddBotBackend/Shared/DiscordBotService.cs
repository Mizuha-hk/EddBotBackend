using Discord;
using Discord.WebSocket;

namespace EddBotBackend.Shared
{
    public class DiscordBotService : BackgroundService
    {
        private DiscordSocketClient _client;
        private readonly IConfiguration _configuration;

        private string Token => _configuration["DISCORD_BOT_TOKEN"];

        public DiscordBotService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            _client = new DiscordSocketClient(
                new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.Guilds 
                        | GatewayIntents.GuildMessages 
                        | GatewayIntents.GuildMessageReactions
                        | GatewayIntents.MessageContent
                });

            _client.Log += LogAsync;
            _client.MessageReceived += MessageReceivedAsync;
            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();
        }

        public override async Task StopAsync(CancellationToken token)
        {
            await _client.StopAsync();
        }

        private Task LogAsync(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }  

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (!(message is SocketUserMessage m) || m.Author.IsBot)
            {
                return;
            }

            await m.Channel.SendMessageAsync($"output : {m.Content}");
        }
    }
}
