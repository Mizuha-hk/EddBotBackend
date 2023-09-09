using Discord;
using Discord.WebSocket;

namespace EddBotBackend.Shared
{
    public class DiscordBotService : BackgroundService, IDiscordBotService
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
                        | GatewayIntents.GuildMessageTyping
                        | GatewayIntents.GuildEmojis
                        | GatewayIntents.GuildMembers
                        
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

            await m.Channel.SendMessageAsync($"Made new Channel : {m.Content}");
        }

        public async Task CreateCategory(string categoryName)
        {             
            foreach(var guild in _client.Guilds)
            {
                var restCategoryChannel = await guild.CreateCategoryChannelAsync(categoryName);
                await restCategoryChannel.AddPermissionOverwriteAsync(guild.EveryoneRole, new OverwritePermissions(viewChannel: PermValue.Allow));
                await guild.CreateTextChannelAsync(categoryName, x => x.CategoryId = restCategoryChannel.Id);
            }
        }
    }
}
