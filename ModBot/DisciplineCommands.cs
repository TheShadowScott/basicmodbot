using DSharpPlus;
using static System.Environment;
using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using System.Diagnostics;
using System.Globalization;
using static ModBot.DatabaseCommands;
using DiscordBot;
using Microsoft.Win32.SafeHandles;
using System.Security.Cryptography.X509Certificates;
using static System.Globalization.TextInfo;
using static ModBot.GMethods;

namespace ModBot;
public sealed partial class DiscordCommands : ApplicationCommandModule
{
    [SlashCommand("warn", "Warns a user.")]
    public async Task WarnUser(InteractionContext ctx, [Option("user", "User to issue the warning to.")] DiscordUser user,
        [Option("reason", "Reason for warning the user.")] string reason = "No reason provided.")
    {
        if (!ctx.CheckPermissions(Permissions.ModerateMembers))
        {
            await ctx.CreateResponseAsync("You must have the `ManageRoles` permission to use this command", true);
            return;
        }
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var time = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);

        await ExecCommand("WARN", user.Id, reason, ctx.User, time);

        var embed = QuickCreate.QuickEmbed(@$"<@{user.Id}> was warned for {reason}", 0xFFEA00);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));

        await (user as DiscordMember)!.SendMessageAsync(QuickCreate.QuickEmbed(@$"You have been warned in {ctx.Guild.Name} for {reason}", 0xFFEA00));
    }

    [SlashCommand("fetch", "Fetches a user's discipline logs.")]
    public async Task FetchUser(InteractionContext ctx, [Option("user", "User to access")] DiscordUser user)
    {
        try
        {
            if (!ctx.CheckPermissions(Permissions.ModerateMembers))
            {
                await ctx.CreateResponseAsync("You must have the `ManageRoles` permission to use this command", true);
                return;
            }

            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            var results = FetchData(user.Id);
            results = results.OrderBy(i => i.Item6).ToList();
            results.Reverse();
            var splitResult = results.ChunkBy(6);

            if (results.Count == 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No Logs Found!"));
                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Found Results for <@{user.Id}>:"));
            int i = 0;

            foreach (var item in splitResult)
            {
                var warnings = new DiscordEmbedBuilder();
                try
                {
                    foreach ((string id, string type, long _, long perp, string reason, string timestamp) in item)
                    {
                        warnings.AddField(type, $"**Perpetrator**: <@{perp}> -  {perp}\n**Reason**: {reason}\n**Time**: <t:{((DateTimeOffset)DateTime.Parse(timestamp)).ToUnixTimeSeconds()}:F>\n**Id No**: {id}");
                    }
                    warnings.WithTitle($"Dicipline Logs for {item[0].Item3}")
                        .WithColor(0x20afff)
                        .WithTimestamp(DateTimeOffset.UtcNow)
                        .WithFooter($"Page {++i}")
                        .WithDescription("");

                    await ctx.Channel.SendMessageAsync(warnings);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await ctx.CreateResponseAsync("Something Went Wrong");
        }
    }

    [SlashCommand("kick", "Kicks a user")]
    public async Task KickUser(InteractionContext ctx, [Option("user", "User to kick")] DiscordUser user,
        [Option("reason", "Reason to kick")] string reason = "No Reason Provided")
    {
        if (!ctx.CheckPermissions(Permissions.KickMembers))
        {
            await ctx.CreateResponseAsync("You must have the `KickMembers` permission to use this command", true);
            return;
        }
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        await (user as DiscordMember)!.SendMessageAsync(QuickCreate.QuickEmbed(@$"You have been kicked from {ctx.Guild.Name} for {reason}", 0xDf9449));
        await (user as DiscordMember)!.RemoveAsync(reason);
        await ExecCommand("KICK", user.Id, reason, ctx.User);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(QuickCreate.QuickEmbed(@$"<@{user.Id}> has been kicked for {reason}", 0xDf9449)));
    }
    [SlashCommand("ban", "Bans a User")]
    public async Task BanUser(InteractionContext ctx, [Option("user", "User to ban.")] DiscordUser user, 
        [Option("days", "Days of messges to delete")] long days = 0,
        [Option("reason", "Reason to ban")] string reason = "No reason provided")
    {
        if (!ctx.CheckPermissions(Permissions.BanMembers))
        {
            await ctx.CreateResponseAsync("You must have the `BanMembers` permission to use this command", true);
            return;
        }
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        await (user as DiscordMember)!.SendMessageAsync(QuickCreate.QuickEmbed(@$"You have been banned from {ctx.Guild.Name} for {reason}", 0xC70039));
        await (user as DiscordMember)!.BanAsync((int)days, reason);
        await ExecCommand("BAN", user.Id, reason, ctx.User);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(QuickCreate.QuickEmbed($"<@{user.Id}> has been banned for {reason}.\n\nAdditionally I deleted {days} days of messages.", 0xC70039)));
    }
    [SlashCommand("unban", "Unbans a user")]
    public async Task UnbanUser(InteractionContext ctx, [Option("user", "User to unban")] DiscordUser user, 
        [Option("reason", "Reason to unban")] string reason = "No reason provided")
    {
        if (!ctx.CheckPermissions(Permissions.BanMembers))
        {
            await ctx.CreateResponseAsync("You must have the `BanMembers` permission to use this command", true);
            return;
        }
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        await user.UnbanAsync(ctx.Guild, reason);
        await ExecCommand("UNBAN", user.Id, reason, ctx.User);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(QuickCreate.QuickEmbed(@$"<@{user.Id}> has been unbanned for {reason}.", 0x39845F)));
    }
    [SlashCommand("droplog", "Drops a discipline log.")]
    public async Task DropData(InteractionContext ctx, [Option("log", "Id of the log to drop (should be exactly 36 characters).")] string id)
    {
        if (!ctx.CheckPermissions(Permissions.ModerateMembers))
        {
            await ctx.CreateResponseAsync("You must have the `ManageRoles` permission to use this command", true);
            return;
        }
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        await Drop(id);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Log with id `{id}` has been DROPPED and is no longer accessable."));
    }
    [SlashCommand("note", "Adds a note to a user.")]
    public async Task Note(InteractionContext ctx, [Option("user", "User to issue the warning to.")] DiscordUser user,
        [Option("note", "Note to add to the user.")] string note)
    {
        if (!ctx.CheckPermissions(Permissions.ModerateMembers))
        {
            await ctx.CreateResponseAsync("You must have the `ManageRoles` permission to use this command", true);
            return;
        }
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        var time = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);

        await ExecCommand("NOTE", user.Id, note, ctx.User, time);

        var embed = QuickCreate.QuickEmbed(@$"Added note to <@{user.Id}>: {note}", 0x49B6DF);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));

    }
    [SlashCommand("timeout", "Time outs the specified user")]
    public async Task TimeOutUser(InteractionContext ctx, [Option("user", "User to timeout")] DiscordUser user,
        [Choice("Remove Timeout", -1)]
        [Choice("60 Seconds", 1)]
        [Choice("5 Minutes", 5)]
        [Choice("10 Minutes", 10)]
        [Choice("1 Hour", 60)]
        [Choice("1 Day", 1440)]
        [Choice("1 Week", 10_080)]
        [Option("length", "Time to timeout a userfor")] double length,
        [Option("reason", "Reason to time out")] string reason = "No reason provided")
    {
        if (!ctx.CheckPermissions(Permissions.ModerateMembers))
        {
            await ctx.CreateResponseAsync("You must have the `Time out members` permission to use this command.");
            return;
        }

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        try
        {
            await ExecCommand("MUTE", user.Id, $"{(length != -1 ? $"{length} min" : "REMOVED")}; {reason}", ctx.User);
            await (user as DiscordMember)!.TimeoutAsync(DateTimeOffset.UtcNow.AddMinutes(length), reason);
            await ctx.EditResponseAsync(
                new DiscordWebhookBuilder().AddEmbed(
                    QuickCreate.QuickEmbed(
                        length != -1 ?
                        $"I timed out <@{user.Id}> for {length} minutes for {reason}"
                        : $"I removed the time out from <@{user.Id}> for {reason}", 0xfd5da8)
                    )
                );
            if (length > 0)
                await (user as DiscordMember)!.SendMessageAsync($"You were timed out in {ctx.Guild.Name} for {length} mintues for {reason}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Something went wrong."));
        }
    }
}