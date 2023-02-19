using System;
using static ModBot.DatabaseCommands;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace ModBot;

class Program
{
    static async Task Main()
    {
        DotNetEnv.Env.TraversePath().Load();

        var config = new DiscordConfiguration()
        {
            Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            LogTimestampFormat = "dd MMM yyyy - HH:mm:ss.fff zzz"
        };

        var client = new DiscordClient(config);

        var slash = client.UseSlashCommands();

        slash.RegisterCommands(typeof(DiscordCommands));

        await client.ConnectAsync();

        foreach (var guild in client.Guilds) Console.WriteLine(guild);

        Thread.Sleep(Timeout.Infinite);

    }
}