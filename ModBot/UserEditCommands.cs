using DiscordBot;
using System.Globalization;
using static ModBot.DatabaseCommands;

namespace ModBot;
public sealed partial class DiscordCommands : ApplicationCommandModule
{
    [SlashRequireGuild()]
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
                    foreach ((string id, string type, _, long perp, string reason, string timestamp) in item)
                    {
                        warnings.AddField(type, $"**Actor**: <@{perp}> -  {perp}\n**Reason**: {reason}\n**Time**: <t:{((DateTimeOffset)DateTime.Parse(timestamp)).ToUnixTimeSeconds()}:F>\n**Id No**: {id}");
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
    [SlashRequireGuild()]
    [SlashCommand("droplog", "Drops a discipline log.")]
    public async Task DropData(InteractionContext ctx, [Option("log", "Id of the log to drop (should be exactly 36 characters).")] string id)
    {
        if (!ctx.EditCheck())
        {
            await ctx.CreateResponseAsync("You don't have the required permissions to use this command.", true);
            return;
        }
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        await DropAsync(id);
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Log with id `{id}` has been DROPPED and is no longer accessable."));
    }
    [SlashRequireGuild()]
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

        await ExecCommandAsync("NOTE", user.Id, note, ctx.User, time);

        var embed = QuickCreate.QuickEmbed(@$"Added note to <@{user.Id}>: {note}", 0x49B6DF);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));

    }
    [SlashRequireGuild()]
    [SlashCommand("alterlog", "Alters an existing log.")]
    public async Task AlterLog(InteractionContext ctx, [Option("id", "ID of the log to change")] string id, 
        [Option("reason", "The new reasoning for the warning")] string reason)
    {
        if (!ctx.EditCheck())
        {
            await ctx.CreateResponseAsync("You don't have the required permissions to use this command.", true);
            return;
        }

        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        await AlterAsync(id, reason);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(
                new DiscordEmbedBuilder()
                {
                    Title = "Altered Log",
                    Description = $"Altered the log `{id}` to have reason `{reason.Replace("`", "\\`")}`. The old reason has been updated and is no longer recoverable.",
                    Color = (DiscordColor)0xff007f
                }
            ));
    }
    [SlashCommand("userinfo", "Grabs information about a user.")]
    public async Task UserInfo(InteractionContext ctx, [Option("user", "User to grab the info for.")] DiscordUser user = null!)
    {
        user ??= ctx.User;
        var member = user as DiscordMember;
        var embed = new DiscordEmbedBuilder()
            .WithTitle("User Information")
            .WithDescription($"<@{user.Id}> - {user.Id}")
            .AddField("Discord Join Date", $"<t:{user?.CreationTimestamp.ToUnixTimeSeconds()}:F>", true)
            .WithThumbnail((member ?? user)?.AvatarUrl);
        if (member is not null)
            embed.AddField("Server Join Date", $"<t:{member?.JoinedAt.ToUnixTimeSeconds()}:F>", true)
                .AddField("Roles", string.Join(' ', member?.Roles.OrderBy(i => i.Name).Select(x => $"<@&{x.Id}>").Take(20).ToList() ?? new() { "No roles" }));

        await ctx.CreateResponseAsync(embed);
    }
    [SlashCommand("ping", "Pings the bot")]
    public async Task Ping(InteractionContext ctx) =>
        await ctx.CreateResponseAsync($"Pong! `{ctx.Client.Ping} ms`");
}
