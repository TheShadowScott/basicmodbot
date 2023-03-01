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
using DSharpPlus.CommandsNext.Attributes;
using System.Diagnostics.Metrics;

namespace ModBot;
public sealed partial class DiscordCommands : ApplicationCommandModule
{
    public enum UrgencyLevel
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Urgent = 4
    }
    [SlashCommand("modmail", "Sends a message to the server moderators.")]
    public static async Task ModMail(InteractionContext ctx, [Option("urgency", "Urgency of message")] UrgencyLevel urgency,
        [Option("message", "Message to send to the moderators")] string message)
    {
        var channel = await ctx.Client.GetChannelAsync((ulong)Convert.ToInt64(GetEnvironmentVariable("MODMAIL_CHANNEL_ID")));

        var embed = new DiscordEmbedBuilder()
            .WithAuthor('@' + ctx.User.Username)
            .WithTimestamp(DateTimeOffset.UtcNow)
            .WithDescription(message)
            .WithColor(
                (int)urgency switch
                {
                    4 => 0x72286f,
                    3 => 0xb80f0a,
                    2 => 0x933b17,
                    1 => 0xffdf00,
                    _ => 0xffffff
                }
            );

        var msg = new DiscordMessageBuilder()
            .WithEmbed(embed);
        if ((int)urgency >= Convert.ToInt32(GetEnvironmentVariable("URGENCY_PING_LEVEL")))
            msg.WithContent($"<@&{GetEnvironmentVariable("MOD_ROLE_ID")}>");

        await channel.SendMessageAsync(msg);
        await ctx.CreateResponseAsync("Message sent!");
    }
    [SlashCommand("addrole", "Adds a role to a user.")]
    public static async Task AddRole(InteractionContext ctx, [Option("user", "User to give the role to")] DiscordUser user,
        [Option("role", "Role to give the user")] DiscordRole role,
        [Option("log", "Log this to the database?")] bool log = false)
    {
        if (!ctx.CheckPermissions(Permissions.ManageRoles))
        {
            await ctx.CreateResponseAsync("You need the `Manage Roles` permission to use this command.");
            return;
        }

        if ((user as DiscordMember)!.Roles.Contains(role))
        {
            await ctx.CreateResponseAsync("This user already has this role!");
            return;
        }

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        await (user as DiscordMember)!.GrantRoleAsync(role);

        if (log) 
            await ExecCommand("AROLE", user.Id, role.Id.ToString(), ctx.User);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully gave <@{user.Id}> the <@&{role.Id}> role!"));
    }
    [SlashCommand("removerole", "Adds a role to a user.")]
    public static async Task Remove(InteractionContext ctx, [Option("user", "User to give the role to")] DiscordUser user,
        [Option("role", "Role to give the user")] DiscordRole role,
        [Option("log", "Log this to the database?")] bool log = false)
    {
        if (!ctx.CheckPermissions(Permissions.ManageRoles))
        {
            await ctx.CreateResponseAsync("You need the `Manage Roles` permission to use this command.");
            return;
        }

        if (!((user as DiscordMember)!.Roles.Contains(role)))
        {
            await ctx.CreateResponseAsync("This user doesn't have this role!");
            return;
        }

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        try
        {
            await (user as DiscordMember)!.GrantRoleAsync(role);

            if (log)
                await ExecCommand("RROLE", user.Id, role.Id.ToString(), ctx.User);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully removed the <@&{role.Id}> role from <@{user.Id}>!"));
        }
        catch (Exception e)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Sorry I couldn't complete that operation."));
            Console.WriteLine(e);
        }
    }
    [SlashCommand("rename", "Sets the nickname of a member")]
    public static async Task Nick(InteractionContext ctx, [Option("user", "User to rename.")] DiscordUser user, [Option("name", "Name to change to")] string name,
        [Option("reason", "Reason for the name change")] string reason)
    {
        if (!ctx.CheckPermissions(Permissions.ManageNicknames))
        {
            await ctx.CreateResponseAsync("You need the `Manage Nicknames` permission");
            return;
        }
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        try
        {
            await (user as DiscordMember)!.ModifyAsync(x =>
            {
                x.Nickname = name;
                x.AuditLogReason = reason;
            });

            await ExecCommand("NICK", user.Id, reason + $" (Name changed to {name} from {user.Username}.)", ctx.User);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(
                QuickCreate.QuickEmbed(@$"Changed the nickname for <@{user.Id}> ({user.Username}) to {name} for: `{reason.Replace("`", "\\`")}`.", 0x555555)
                ));
        }
        catch (Exception e)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Something went wrong."));
        }
    }
}