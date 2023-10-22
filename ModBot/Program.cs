global using System;
global using DSharpPlus;
global using DSharpPlus.SlashCommands;
global using DSharpPlus.Entities;
global using DSharpPlus.SlashCommands.Attributes;
using System.Xml.Serialization;
using DSharpPlus.CommandsNext;

namespace ModBot;

public sealed partial class DiscordCommands : ApplicationCommandModule
{
    [SlashCommand("repo", "Links the github repo")]
    public async Task Repo(InteractionContext ctx) => await ctx.CreateResponseAsync("https://github.com/TheShadowScott/basicmodbot");
}
class Program
{
    private static async Task UpdateStatus(DiscordClient client, DiscordActivity activity)
        => await client.UpdateStatusAsync(activity);
    internal sealed class NullCommands : ApplicationCommandModule { }
    internal static Settings LocalSettings = new();
    internal static void LoadSettings()
    {
        #pragma warning disable IL2026
        var xmlSerializer = new XmlSerializer(typeof(Settings));
        using StringReader reader = new(File.ReadAllText(@".\Settings.xml"));
        LocalSettings = (Settings)xmlSerializer.Deserialize(reader)!;
        #pragma warning restore IL2026
    }
    internal static List<ulong> ModList => LocalSettings.BotSettings.ModRoles.ModRoleId;
    static async Task Main()
    {
        LoadSettings();
        LocalSettings.InitDeveloperSettings();

        var config = new DiscordConfiguration()
        {
            Token = LocalSettings.InitSettings.BotId,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            LogTimestampFormat = "dd MMM yyyy - HH:mm:ss.fff zzz",
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildPresences | DiscordIntents.MessageContents | DiscordIntents.GuildMembers,
        };

        var client = new DiscordClient(config);

        var slash = client.UseSlashCommands();

        // Use when commands should be refreshed
        // slash.RegisterCommands<NullCommands>();

        slash.RegisterCommands(typeof(DiscordCommands));

        var commands = client.UseCommandsNext(new CommandsNextConfiguration()
        {
            StringPrefixes = new[] { "gf!" }
        });

        commands.RegisterCommands(typeof(OwnerCommands));

        DiscordActivity activity = new()
        {
            Name = "/help",
            ActivityType = ActivityType.ListeningTo
        };


        await client.ConnectAsync(activity, UserStatus.Online);

        // Use this space to make client method calls to enable different logging methods. See ./Logs.cs for available methods.
        client.EnableAllLoggingMethods();

        // Client error handling. Update to completeness later
        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously; ClientError requires async, warning is not needed
        client.ClientErrored += async (s, e) => Console.WriteLine(e.Exception.ToString());
        #pragma warning restore CS1998

        await Task.Delay(Timeout.Infinite);
    }
}