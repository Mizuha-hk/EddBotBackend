namespace EddBotBackend.Shared
{
    public interface IDiscordBotService
    {
        public Task CreateCategory(string categoryName);
    }
}
