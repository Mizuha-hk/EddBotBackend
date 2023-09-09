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
                var channel = await guild.CreateTextChannelAsync(categoryName, x => x.CategoryId = restCategoryChannel.Id);
                var message = await channel.SendMessageAsync($"Welcome to **{categoryName}**.\r\n Adding reaction below, you can add this event role.\r\n");
                //await message.AddReactionAsync(new Emoji(""));
                
            }
        }

        public async Task<CategoryStatus> CreateThreads(string categoryName, string channelName, List<string> threads)
        {
            foreach(var guild in _client.Guilds)
            {
                var category = guild.CategoryChannels.FirstOrDefault(x => x.Name == categoryName);
                if(category == null)
                {
                    return CategoryStatus.CategoryNotFound;
                }

                await guild.CreateTextChannelAsync(channelName, x => x.CategoryId = category.Id);
                var channel = guild.TextChannels.FirstOrDefault(x => x.Name == channelName);

                if(channel == null)
                {
                    return CategoryStatus.CreateChannelFailed;
                }

                foreach(var thread in threads)
                {  
                    await channel.CreateThreadAsync(thread); 
                }
            }

            return CategoryStatus.Success;
        }

        public enum CategoryStatus
        {
            Success,
            CategoryNotFound,
            CreateChannelFailed,
        }
    }
}
