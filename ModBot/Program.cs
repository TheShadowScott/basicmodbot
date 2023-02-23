using System;
using static ModBot.DatabaseCommands;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using System.Xml.Serialization;

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
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
        using StringReader reader = new(File.ReadAllText(@".\Settings.xml"));
        LocalSettings = (Settings)xmlSerializer.Deserialize(reader)!;
    }
    internal static List<ulong> ModList => LocalSettings.BotSettings.ModRoles.ModRoleIds;
    static async Task Main()
    {
        LoadSettings();

        var config = new DiscordConfiguration()
        {
            Token = LocalSettings.InitSettings.BotId,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            LogTimestampFormat = "dd MMM yyyy - HH:mm:ss.fff zzz",
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildPresences,
        };

        var client = new DiscordClient(config);

        var slash = client.UseSlashCommands();

        slash.RegisterCommands<NullCommands>();
        slash.RegisterCommands(typeof(DiscordCommands));

        DiscordActivity activity = new()
        {
            Name = "Under Devlopment",
            ActivityType = ActivityType.Playing
        };

        await client.ConnectAsync(activity, UserStatus.DoNotDisturb);

        Thread.Sleep(Timeout.Infinite);

    }
}