using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModBot.Program;
using static ModBot.GMethods;
using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace ModBot;
internal static partial class Logging
{
    private static ulong MainServerId => LocalSettings.BotSettings.MainServerId;
    private static ulong LogChannel => LocalSettings.BotSettings.LogChannelId;
    private static DateTimeOffset Now => DateTimeOffset.UtcNow;
    const string DateFormat = @"dddd, d MMMM yyyy, HH:mm:ss (UTC)"; // Update to preffered date format.
    /// <summary>
    /// Adds all logging methods. Requires all Discord intents.
    /// </summary>
    /// <param name="client"></param>
    /// <exception cref="ArgumentNullException"></exception>
    internal static void EnableAllLoggingMethods(this DiscordClient client)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        client.EnableMemberJoinLogging()
            ?.EnableMemberLeaveLogging()
            ?.EnableMessageAlterLogging()
            ?.EnableMessageDeletionLogging()
            ?.EnableMemberUpdateLogging()
            ?.EnableModerationLogging()
            ?.EnableVoiceChannelLogging();
    }
    /// <summary>
    /// Adds a logging method for message deletion. Requires <c>DiscordIntents.MessageContents</c>.
    /// </summary>
    /// <param name="client">Discord Client to add the logger to.</param>
    /// <returns><paramref name="client"></paramref> if successful, otherwise <c>null</c></returns>
    internal static DiscordClient? EnableMessageDeletionLogging(this DiscordClient client)
    {
        try
        {
            client.MessageDeleted += async (sender, @event) =>
            {
                if (@event.Guild is null || @event.Guild.Id != MainServerId)
                    return;
                await sender.SendMessageAsync(await sender.GetChannelAsync(LocalSettings.BotSettings.LogChannelId), new DiscordEmbedBuilder()
                     .WithTitle($"Message Deleted in <#{@event.Channel.Id}>")
                     .WithDescription(@event.Message.Content)
                     .WithColor(new DiscordColor(0xBD10E0))
                     .WithTimestamp(DateTimeOffset.UtcNow)
                     .WithAuthor(
                         name: $"{@event.Message.Author.Username}{(@event.Message.Author.Discriminator is not "0" or null ?
                         '#' + @event.Message.Author.Discriminator : "")}",
                         iconUrl: @event.Message.Author.AvatarUrl
                     )
                     .AddField("User ID", $"{@event.Message.Author.Id}", true)
                     .AddField("Message ID", @event.Message.Id.ToString(), true));
            };
            return client;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
    /// <summary>
    /// Adds a logging method for message updating. Requires <c>DiscordIntents.MessageContents</c>.
    /// </summary>
    /// <param name="client">Discord client to add the logger to.</param>
    /// <returns><paramref name="client"></paramref> if successful, otherwise <c>null</c></returns>
    internal static DiscordClient? EnableMessageAlterLogging(this DiscordClient client)
    {
        try
        {
            client.MessageUpdated += async (sender, @event) =>
            {
                if (@event.Guild is null || @event.Guild.Id != MainServerId || @event.Author.IsBot || @event.Message.Content == @event.MessageBefore.Content)
                    return;
                await sender.SendMessageAsync(await sender.GetChannelAsync(LocalSettings.BotSettings.LogChannelId), new DiscordEmbedBuilder()
                     .WithTitle($"Message Edited in <#{@event.Channel.Id}>")
                     .WithDescription($"***Old Content:***\n{@event.MessageBefore.Content}\n***New Content:***\n{@event.Message.Content}")
                     .WithColor(new DiscordColor(0xBD10E0))
                     .WithTimestamp(Now)
                     .WithAuthor(
                         name: $"{@event.Message.Author.Username}{(@event.Message.Author.Discriminator is not "0" or null ?
                    '#' + @event.Message.Author.Discriminator : "")}",
                         iconUrl: @event.Message.Author.AvatarUrl
                     )
                     .AddField("User ID", @event.Message.Author.Id.ToString(), true)
                     .AddField("Message ID", @event.Message.Id.ToString(), true));
            };
            return client;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
    /// <summary>
    /// Adds a logging method on member join. equires <c>DiscordIntents.GuildMembers</c>.
    /// </summary>
    /// <param name="client">Discord client to add the logger to.</param>
    /// <returns><paramref name="client"></paramref> if successful, otherwise <c>null</c></returns>
    internal static DiscordClient? EnableMemberJoinLogging(this DiscordClient client)
    {
        try
        {
            client.GuildMemberAdded += async (sender, @event) =>
            {
                if (@event.Guild is null || @event.Guild.Id != MainServerId)
                    return;
                await sender.SendMessageAsync(await sender.GetChannelAsync(LogChannel), new DiscordEmbedBuilder()
                     .WithTitle("Member Join")
                     .WithDescription($"{@event.Member.DisplayName}{(@event.Member.Discriminator is not "0" or null ?
                    '#' + @event.Member.Discriminator : "")} ({@event.Member.Id})")
                     .WithColor(new DiscordColor(0xBD10E0))
                     .WithTimestamp(Now)
                     .WithThumbnail(url: @event.Member.AvatarUrl)
                     // Remove `AddOrdinalToDateObject` if you don't want Ordinal Dates. (i.e.: 12th, 21st &c.)
                     .AddField("Account Created", AddOrdinalToDateObject(@event.Member.CreationTimestamp, DateFormat)));
            };
            return client;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
    /// <summary>
    /// Enables logging when a member leaves the server.
    /// </summary>
    /// <param name="client">The Discord client to enable logging for.</param>
    /// <returns><paramref name="client"></paramref> if successful, otherwise <c>null</c></returns>
    internal static DiscordClient? EnableMemberLeaveLogging(this DiscordClient client)
    {
        try
        {
            client.GuildMemberRemoved += async (sender, @event) =>
            {
                if (@event.Guild is null || @event.Guild.Id != MainServerId)
                    return;
                await sender.SendMessageAsync(await sender.GetChannelAsync(LogChannel), new DiscordEmbedBuilder()
                    .WithTitle("Member Left")
                    .WithDescription(@$"{@event.Member.DisplayName}{(@event.Member.Discriminator is not "0" or null ?
                    '#' + @event.Member.Discriminator : "")} ({@event.Member.Id})")
                    .WithColor(new DiscordColor(0xBD10E0))
                    .WithTimestamp(Now)
                    .WithThumbnail(url: @event.Member.AvatarUrl)
                    .AddField("Was Banned?", (await @event.Guild.GetBansAsync()).Any(ban => ban.User.Id == @event.Member.Id) ? "Yes" : "No"));
            };
            return client;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
    /// <summary>
    /// Enables logging when a guild role is created, deleted, or updated.
    /// </summary>
    /// <param name="client">The Discord client to enable logging for.</param>
    /// <returns><paramref name="client"></paramref> if successful, otherwise <c>null</c></returns>
    internal static DiscordClient? EnableGuildRoleChangeLogging(this DiscordClient client)
    {
        try
        {
            client.GuildRoleCreated += async (sender, @event) =>
            {
                if (@event.Guild is null || @event.Guild.Id != MainServerId)
                    return;
                await sender.SendMessageAsync(await sender.GetChannelAsync(LogChannel), new DiscordEmbedBuilder()
                            .WithTitle("Role Created")
                            .WithDescription(@event.Role.Name)
                            .WithColor(new DiscordColor(0xBD10E0))
                            .WithTimestamp(Now)
                            .AddField("Role ID", @event.Role.Id.ToString(), true)
                            .AddField("Role Color", @event.Role.Color.ToString(), true));
            };

            client.GuildRoleDeleted += async (sender, @event) =>
            {
                if (@event.Guild is null || @event.Guild.Id != MainServerId)
                    return;
                await sender.SendMessageAsync(await sender.GetChannelAsync(LogChannel), new DiscordEmbedBuilder()
                            .WithTitle("Role Deleted")
                            .WithDescription(@event.Role.Name)
                            .WithColor(new DiscordColor(0xBD10E0))
                            .WithTimestamp(Now)
                            .AddField("Role ID", @event.Role.Id.ToString(), true));
            };

            client.GuildRoleUpdated += async (sender, @event) =>
            {
                if (@event.Guild is null || @event.Guild.Id != MainServerId)
                    return;
                // This gets the permissions of the role before and after the update and displays the changes accordingly
                var permissionsBefore = @event.RoleBefore.Permissions;
                var permissionsAfter = @event.RoleAfter.Permissions;
                var permissionsNotBefore = permissionsAfter & ~permissionsBefore;
                var permissionsNotAfter = permissionsBefore & ~permissionsAfter;

                var desc = new StringBuilder();

                if (permissionsNotBefore != 0)
                    desc.Append($"***Permissions Added:*** {permissionsNotBefore.ToPermissionString()}\n");
                if (permissionsNotAfter != 0)
                    desc.Append($"***Permissions Removed:*** {permissionsNotAfter.ToPermissionString()}\n");

                if (@event.RoleBefore.Color.Value == @event.RoleAfter.Color.Value)
                    desc.Append($"***Color Changed:*** {@event.RoleBefore.Color.Value} -> {@event.RoleAfter.Color.Value}\n");

                if (@event.RoleBefore.Name != @event.RoleAfter.Name)
                    desc.Append($"***Name Changed:*** {@event.RoleBefore.Name} -> {@event.RoleAfter.Name}\n");

                if (@event.RoleBefore.IsHoisted != @event.RoleAfter.IsHoisted)
                    desc.Append($"***Hoist Changed:*** {@event.RoleBefore.IsHoisted} -> {@event.RoleAfter.IsHoisted}\n");

                if (@event.RoleBefore.IsMentionable != @event.RoleAfter.IsMentionable)
                    desc.Append($"***Mentionable Changed:*** {@event.RoleBefore.IsMentionable} -> {@event.RoleAfter.IsMentionable}\n");

                if (desc.Length == 0)
                    return;

                await sender.SendMessageAsync(await sender.GetChannelAsync(LogChannel), new DiscordEmbedBuilder()
                            .WithTitle("Role Updated")
                            .WithDescription(@event.RoleAfter.Name)
                            .WithColor(new DiscordColor(0xBD10E0))
                            .WithTimestamp(Now)
                            .AddField("Role ID", @event.RoleBefore.Id.ToString(), true)
                            .AddField("Changes", desc.ToString()));
            };

            return client;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
    /// <summary>
    /// Enables logging of member updates, such as role changes and nickname changes.
    /// </summary>
    /// <param name="client">The DiscordClient object.</param>
    /// <param name="enableRoleUpdates">Whether to enable logging of role changes. True by default.</param>
    /// <param name="enableDisplayNameUpdates">Whether to enable logging of nickname changes. True by default.</param>
    /// <returns><paramref name="client"></paramref> if successful, otherwise <see langword="null"/></returns>
    internal static DiscordClient? EnableMemberUpdateLogging(this DiscordClient client, bool enableRoleUpdates = true,
            bool enableDisplayNameUpdates = true)
    {
        try
        {
            if (enableRoleUpdates)
            {
                client.GuildMemberUpdated += async (sender, @event) =>
                {
                    if (@event.Guild.Id != MainServerId)
                        return;

                    IEnumerable<DiscordRole> rBefore = @event.MemberBefore.Roles,
                                             rAfter = @event.MemberAfter.Roles;

                    var rNotAfter = rBefore.Except(rAfter);
                    var rNotBefore = rAfter.Except(rBefore);

                    if (!rNotAfter.Any() && !rNotBefore.Any())
                        return;

                    var descriptor = new StringBuilder();

                    if (rNotAfter.Any())
                        descriptor.Append($"***Roles Removed:*** {rNotAfter.ToDiscordRoleDisplayString()}\n");
                    if (rNotBefore.Any())
                        descriptor.Append($"***Roles Added:*** {rNotBefore.ToDiscordRoleDisplayString()}\n");


                    await sender.SendMessageAsync(await sender.GetChannelAsync(LogChannel), new DiscordEmbedBuilder()
                        .WithTitle("Member Roles Updated")
                        .WithAuthor(name: @$"@{@event.Member.Username}{(@event.Member.Discriminator is not "0" or null ?
                            '#' + @event.Member.Discriminator : "")} ({@event.Member.Id})",
                            iconUrl: @event.Member.AvatarUrl)
                        .WithDescription(descriptor.ToString())
                        .WithColor(new DiscordColor(0xBD10E0))
                        .WithTimestamp(Now));
                };
            }
            if (enableDisplayNameUpdates)
            {
                client.GuildMemberUpdated += async (sender, @event) =>
                {
                    if (@event.Guild.Id != MainServerId)
                        return;
                    if (@event.MemberBefore.Nickname != @event.MemberAfter.Nickname)
                    {
                        await sender.SendMessageAsync(await sender.GetChannelAsync(LogChannel), new DiscordEmbedBuilder()
                            .WithTitle("Changed Nickname")
                            .WithAuthor(name: @$"{@event.Member.Username}{(@event.Member.Discriminator is not "0" or null ?
                                '#' + @event.Member.Discriminator : "")} ({@event.Member.Id})",
                                iconUrl: @event.Member.AvatarUrl)
                            .WithDescription($"***Old Nickname:*** {@event.MemberBefore.Nickname}\n***New Nickname:*** {@event.MemberAfter.Nickname}")
                            .WithColor(new DiscordColor(0xBD10E0))
                            .WithTimestamp(Now));
                    }
                };
            }
            return client;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    /// <summary>
    /// Enables logging of voice channel events.
    /// </summary>
    /// <param name="client">Discord client</param>
    /// <returns><paramref name="client"></paramref> if successful, otherwise <see langword="null"/></returns>
    internal static DiscordClient? EnableVoiceChannelLogging(this DiscordClient client)
    {
        try
        {
            client.VoiceStateUpdated += async (sender, @event) =>
            {
                var channel = await sender.GetChannelAsync(LogChannel);
                if (@event.Guild.Id != MainServerId)
                    return;
                if (@event.Before?.Channel?.Id != @event.After?.Channel?.Id)
                {
                    if (@event.Before?.Channel is not null)
                    {
                        await sender.SendMessageAsync(channel, new DiscordEmbedBuilder()
                        .WithTitle("Member Left Voice Channel")
                        .WithAuthor(name: @$"{@event.User.Username}{(@event.User.Discriminator is not "0" or null ?
                            '#' + @event.User.Discriminator : "")}",
                            iconUrl: @event.User.AvatarUrl)
                        .WithDescription($"***Channel:*** {@event.Before.Channel.Name}")
                        .WithColor(new DiscordColor(0xBD10E0))
                        .WithTimestamp(Now));
                    }
                    else if (@event.After?.Channel is not null)
                    {
                        await sender.SendMessageAsync(channel, new DiscordEmbedBuilder()
                        .WithTitle("Member Joined Voice Channel")
                        .WithAuthor(name: @$"{@event.User.Username}{(@event.User.Discriminator is not "0" or null ?
                            '#' + @event.User.Discriminator : "")}",
                            iconUrl: @event.User.AvatarUrl)
                        .WithDescription($"***Channel:*** {@event.After.Channel.Name}")
                        .WithColor(new DiscordColor(0xBD10E0))
                        .WithTimestamp(Now));
                    }
                    else
                    {
                        await sender.SendMessageAsync(channel, new DiscordEmbedBuilder()
                        .WithTitle("Member Changed Voice Channels")
                        .WithAuthor(name: @$"{@event.User.Username}{(@event.User.Discriminator is not "0" or null ?
                            '#' + @event.User.Discriminator : "")}",
                            iconUrl: @event.User.AvatarUrl)
                        .WithDescription($"{@event.Before?.Channel.Name} -> {@event?.After?.Channel.Name}")
                        .WithColor(new DiscordColor(0xBD10E0))
                        .WithTimestamp(Now));
                    }
                }
            };
            return client;
        }
        catch
        {
            return null;
        }
    }
    /// <summary>
    /// Enables logging of moderation actions, such as bans and unbans.
    /// </summary>
    /// <param name="client">Discord Client</param>
    /// <param name="enableBanLogging">Enables the logging of bans</param>
    /// <param name="enableUnbanLogging">Enables the logging of unbans</param>
    /// <returns><paramref name="client"/> if successful, otherwise <c>null</c></returns>
    internal static DiscordClient? EnableModerationLogging(this DiscordClient client, bool enableBanLogging = true, bool enableUnbanLogging = true)
    {
        try
        {
            if (enableBanLogging)
            {
                client.GuildBanAdded += async (sender, @event) =>
                {
                    await sender.SendMessageAsync(await sender.GetChannelAsync(LogChannel), new DiscordEmbedBuilder()
                                .WithTitle("User Banned")
                                .WithAuthor(name: @$"{@event.Member.DisplayName}{(@event.Member.Discriminator is not "0" or null ?
                                    '#' + @event.Member.Discriminator : "")}",
                                    iconUrl: @event.Member.AvatarUrl)
                                .WithDescription(@event.Member.Mention)
                                .WithColor(new DiscordColor(0xBD10E0))
                                .WithTimestamp(Now)
                                .AddField("User ID", @event.Member.Id.ToString(), true)
                                .AddField("Reason", @event.Guild.GetBanAsync(@event.Member).Result.Reason ?? "No reason provided", true));
                };
            }
            if (enableUnbanLogging)
            {
                client.GuildBanRemoved += async (sender, @event) =>
                {
                    await sender.SendMessageAsync(await sender.GetChannelAsync(LogChannel), new DiscordEmbedBuilder()
                        .WithTitle("User Unbanned")
                        .WithAuthor(name: @$"{@event.Member.DisplayName}{(@event.Member.Discriminator is not "0" or null ? '#' + @event.Member.Discriminator : "")}",
                            iconUrl: @event.Member.AvatarUrl)
                        .WithDescription(@event.Member.Mention)
                        .WithColor(new DiscordColor(0xBD10E0))
                        .WithTimestamp(Now)
                        .AddField("User ID", @event.Member.Id.ToString(), true));
                };
            }
            return client;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
}
