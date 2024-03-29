﻿using DiscordBot;
using static ModBot.DatabaseCommands;

namespace ModBot;
public sealed partial class DiscordCommands : ApplicationCommandModule
{
    [SlashRequireGuild]
    [SlashCommand("addrole", "Adds a role to a user.")]
    public static async Task AddRole(InteractionContext ctx, [Option("user", "User to give a role")] DiscordUser user,
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
            await ExecCommandAsync("AROLE", user.Id, role.Id.ToString(), ctx.User);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully gave <@{user.Id}> the <@&{role.Id}> role!"));
    }
    [SlashRequireGuild]
    [SlashCommand("removerole", "Adds a role to a user.")]
    public static async Task Remove(InteractionContext ctx, [Option("user", "User to take a role from")] DiscordUser user,
        [Option("role", "Role to remove from the user")] DiscordRole role,
        [Option("log", "Log this to the database?")] bool log = false)
    {
        if (!ctx.CheckPermissions(Permissions.ManageRoles))
        {
            await ctx.CreateResponseAsync("You need the `Manage Roles` permission to use this command.");
            return;
        }

        if (!(user as DiscordMember)!.Roles.Contains(role))
        {
            await ctx.CreateResponseAsync("This user doesn't have this role!");
            return;
        }

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        try
        {
            await (user as DiscordMember)!.RevokeRoleAsync(role);

            if (log)
                await ExecCommandAsync("RROLE", user.Id, role.Id.ToString(), ctx.User);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully removed the <@&{role.Id}> role from <@{user.Id}>!"));
        }
        catch (Exception e)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Sorry I couldn't complete that operation."));
            Console.WriteLine(e);
        }
    }
    [SlashRequireGuild]
    [SlashCommand("setnick", "Sets the nickname of a member")]
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

            await ExecCommandAsync("NICK", user.Id, reason + $" (Name changed to {name} from {user.Username}.)", ctx.User);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(
                QuickCreate.QuickEmbed(@$"Changed the nickname for <@{user.Id}> ({user.Username}) to {name} for: `{reason.Replace("`", "\\`")}`.", 0x555555)
                ));
        }
        catch
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Something went wrong."));
        }
    }
}