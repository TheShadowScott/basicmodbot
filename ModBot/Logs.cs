using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModBot.Program;
using static ModBot.GMethods;
namespace ModBot;
internal static class Logging
{
    private static ulong MainServerId => LocalSettings.BotSettings.MainServerId;
    private static ulong LogChannel => LocalSettings.BotSettings.LogChannelId;
    private static DateTimeOffset Now => DateTimeOffset.UtcNow;
    const string DateFormat = @"dd MMM. yyyy, HH:mm:ss (UTC)"; // Update to preffered date format.
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
            ?.EnableModerationLogging();
    }
    /// <summary>
    /// Adds a logging method for message deletion. Requires <c>DiscordIntents.MessageContents</c>.
    /// </summary>
    /// <param name="client">Discord Client to add the logger to.</param>
    /// <returns><c>client</c> if successful, otherwise <c>null</c></returns>
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
                         name: @event.Message.Author.Username + '#' + @event.Message.Author.Discriminator,
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
    /// <returns><c>client</c> if successful, otherwise <c>null</c></returns>
    internal static DiscordClient? EnableMessageAlterLogging(this DiscordClient client)
    {
        try
        {
            client.MessageUpdated += async (sender, @event) =>
            {
                if (@event.Guild is null || @event.Guild.Id != MainServerId || @event.Author.IsBot)
                    return;
                await sender.SendMessageAsync(await sender.GetChannelAsync(LocalSettings.BotSettings.LogChannelId), new DiscordEmbedBuilder()
                     .WithTitle($"Message Edited in <#{@event.Channel.Id}>")
                     .WithDescription($"***Old Content:***\n{@event.MessageBefore.Content}\n***New Content:***\n{@event.Message.Content}")
                     .WithColor(new DiscordColor(0xBD10E0))
                     .WithTimestamp(Now)
                     .WithAuthor(
                         name: @event.Message.Author.Username + '#' + @event.Message.Author.Discriminator,
                         iconUrl: @event.Message.Author.AvatarUrl
                     )
                     .AddField("User ID", @event.Message.Author.Id.ToString(), true)
                     .AddField("Message ID", @event.Message.Id.ToString(), true));
            };
            return client;
        } catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
    /// <summary>
    /// Adds a logging method on member join. equires <c>DiscordIntents.GuildMembers</c>.
    /// </summary>
    /// <param name="client">Discord client to add the logger to.</param>
    /// <returns><c>client</c> if successful, otherwise <c>null</c></</returns>
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
                     .WithDescription($"{@event.Member.DisplayName} ({@event.Member.Id})")
                     .WithColor(new DiscordColor(0xBD10E0))
                     .WithTimestamp(Now)
                     .WithThumbnail(url: @event.Member.AvatarUrl)
                     .AddField("Account Created", @event.Member.CreationTimestamp.ToString(DateFormat)));
            };
            return client;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
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
                    .WithDescription(@$"{@event.Member.DisplayName}" + '#' + $"{@event.Member.Discriminator} ({@event.Member.Id})")
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
                    if (!@event.RolesAfter.SequenceEqual(@event.RolesBefore))
                    {
                        await sender.SendMessageAsync(await sender.GetChannelAsync(LogChannel), new DiscordEmbedBuilder()
                            .WithTitle("Member Roles Updated")
                            .WithAuthor(name: @event.Member.Username + '#' + @event.Member.Discriminator, iconUrl: @event.Member.AvatarUrl)
                            .WithDescription($"***Roles Added:*** {Enumerable.Except(@event.RolesBefore, @event.RolesAfter).ToDiscordRoleDisplayString()}" +
                                $"\n***Roles Removed:*** {Enumerable.Except(@event.RolesAfter, @event.RolesBefore).ToDiscordRoleDisplayString()}")
                            .WithColor(new DiscordColor(0xBD10E0))
                            .WithTimestamp(Now));
                    }
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
                            .WithAuthor(name: @event.Member.Username + '#' + @event.Member.Discriminator, iconUrl: @event.Member.AvatarUrl)
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
    internal static DiscordClient? EnableModerationLogging(this DiscordClient client, bool enableBanLogging = true, bool enableUnbanLogging = true)
    {
        try
        {
            if (enableBanLogging)
            {
                client.GuildBanAdded += async (sender, @event) => { };
            }
            if (enableUnbanLogging) 
            {
                client.GuildBanRemoved += async (sender, @event) => { };
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
