using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace ModBot;

public sealed class OwnerCommands : BaseCommandModule
{
    private bool IsOwner(ulong Id) =>
        Program.LocalSettings.InitSettings.OwnerId.Equals(Id);
    [Command("status")]
    public async Task SetBotStatus(CommandContext ctx, int statusLevel)
    {
        if (!IsOwner(ctx.User.Id))
            return;
        await ctx.Client.UpdateStatusAsync(userStatus: statusLevel switch
        {
            0 => UserStatus.Invisible,
            1 => UserStatus.DoNotDisturb,
            2 => UserStatus.Idle,
            _ => UserStatus.Online
        });
    }
    [Command("activity")]
    public async Task SetBotActivity(CommandContext ctx, int activityType, [RemainingText] string activityName)
    {
        if (!IsOwner(ctx.User.Id))
            return;
        DiscordActivity activity = new()
        {
            ActivityType = (ActivityType)activityType,
            Name = activityName
        };

        await ctx.Client.UpdateStatusAsync(activity: activity);
    }
    [Command("statact")]
    public async Task ChangeBotStatAct(CommandContext ctx, int statusLevel, int activityType,
        [RemainingText] string activityName)
    {
        if (!IsOwner(ctx.User.Id))
            return;
        DiscordActivity activity = new()
        {
            ActivityType = (ActivityType)activityType,
            Name = activityName
        };
        await ctx.Client.UpdateStatusAsync(userStatus: statusLevel switch
        {
            0 => UserStatus.Invisible,
            1 => UserStatus.DoNotDisturb,
            2 => UserStatus.Idle,
            _ => UserStatus.Online
        },
        activity: activity
        );
    }
    [Command("emergency-shutdown")]
    public async Task Stop(CommandContext ctx)
    {
        if (!IsOwner(ctx.User.Id))
            return;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine($"Emergency shutdown performed at {DateTimeOffset.UtcNow}\nDisconnecting and disposing");
        Console.ResetColor();
        await ctx.Client.DisconnectAsync();
        ctx.Client.Dispose();
    }
    [Command("restart")]
    public async Task Restart(CommandContext ctx)
    {
        if (!IsOwner(ctx.User.Id))
            return;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine($"Restart performed at {DateTimeOffset.UtcNow}\nDisconnecting and disposing");
        Console.ResetColor();
        await ctx.Client.DisconnectAsync();
        ctx.Client.Dispose();
        await Program.Main();
    }
    [Command("list-mods")]
    public async Task ListMods(CommandContext ctx)
    {
        if (!IsOwner(ctx.User.Id))
            return;
        var sb = new StringBuilder();
        foreach (var mod in Program.ModList)
            sb.Append($"<@&{mod}> ");
        sb.Length--;
        await ctx.RespondAsync(sb.ToString());
    }
}