namespace ModBot;

public sealed partial class DiscordCommands
{
    private static DiscordEmbedBuilder _QuickBuild(InteractionContext ctx, string name, string desc, List<(string, string)> args, Permissions permissionLevel)
    {
        return new DiscordEmbedBuilder()
        {
            Title = "Help",
            Description = $"`{name}`"
        }
        .AddField("Default Permission", permissionLevel.ToString(), true)
        .AddField("Can Use?", ctx.CheckPermissions(permissionLevel) ? "yes" : "no", true)
        .AddField("Description", desc)
        .AddField("Arguments", string.Join('\n', (from item in args select $"`{item.Item1}` -> {item.Item2}")))
        .WithColor(0xFADADD);

    }
    public enum Commands
    {
        all,
        addrole,
        alterlog,
        ban,
        droplog,
        fetch,
        kick,
        note,
        ping,
        removerole,
        rename,
        repo,
        timeout,
        unban,
        userinfo,
        warn
    }
    [SlashCommand("help", "Get help for bot commands")]
    public async Task Help(InteractionContext ctx, [Option("command", "Command to get information on.")] Commands command = Commands.all)
    {

        switch (command)
        {
            case Commands.all:
                await ctx.CreateResponseAsync("""
                    Supported Commands on this bot are:
                    ```md
                    1. /addrole
                    2. /alterlog
                    3. /ban
                    4. /droplog
                    5. /fetch
                    6. /kick
                    7. /note
                    8. /ping
                    9. /removerole
                    10. /rename
                    11. /repo
                    12. /timeout
                    13. /unban
                    14. /userinfo
                    15. /warn
                    ```
                    For more information on a specific command use `/help <Command>`.
                    """
                    );
                break;
            case Commands.addrole:
                await ctx.CreateResponseAsync(
                    _QuickBuild(
                        ctx,
                        "addrole",
                        "Adds a role to a given user",
                        new() { ("user", "User to give `role`"), ("role", "Role to give `user`") },
                        Permissions.ManageRoles
                        )
                    );
                break;
            case Commands.alterlog:
                await ctx.CreateResponseAsync(
                        new DiscordEmbedBuilder()
                        .WithTitle("Help")
                        .WithDescription("`alterlog`")
                        .AddField("Default Permissions", Program.LocalSettings.BotSettings.LogEditLevel == "Administrator" ? "Administrator"
                            : "Moderator", true)
                        .AddField("Can be used?", ctx.EditCheck() ? "yes" : "no", true)
                        .AddField("Arguments", """
                        `id` -> ID of the log to change
                        `reason` -> Updated reason of the log
                        """
                        )
                        .AddField("Description", "Changes the resoning behind a log.")
                        .WithColor(0xFADADD)
                    );
                break;
            case Commands.ban:
                await ctx.CreateResponseAsync(
                    _QuickBuild(
                        ctx,
                        "ban",
                        "Bans a user",
                        new() { ("user", "User to ban"), ("days", "Timespan of messages to delete"), ("reaon", "reason for the ban") },
                        Permissions.BanMembers
                        )
                    );
                break;
            case Commands.droplog:
                await ctx.CreateResponseAsync(
                        new DiscordEmbedBuilder()
                        .WithTitle("Help")
                        .WithDescription("`droplog`")
                        .AddField("Default Permissions", Program.LocalSettings.BotSettings.LogEditLevel == "Administrator" ? "Administrator"
                            : "Moderator", true)
                        .AddField("Can be used?", ctx.EditCheck() ? "yes" : "no", true)
                        .AddField("Arguments", """
                        `id` -> ID of the log to drop
                        """
                        )
                        .AddField("Description", "Deletes a log from the database")
                        .WithColor(0xFADADD)
                    );
                break;
            case Commands.fetch:
                await ctx.CreateResponseAsync(
                    _QuickBuild(
                        ctx,
                        "fetch",
                        "Fetches the logs for a user",
                        new() { ("user", "The user to fetch logs for") },
                        Permissions.ModerateMembers
                        )
                    );
                break;
            case Commands.kick:
                await ctx.CreateResponseAsync(
                    _QuickBuild(
                        ctx,
                        "kick",
                        "Kicks a user",
                        new() { ("user", "User to kick"), ("reason", "Reason for kicking the user") },
                        Permissions.KickMembers
                        )
                    );
                break;
            case Commands.note:
                await ctx.CreateResponseAsync(
                    _QuickBuild(
                        ctx,
                        "note",
                        "Adds a note to a user",
                        new() { ("user", "User to add a note to"), ("note", "Note to add to `user`") },
                        Permissions.ModerateMembers
                        ));
                break;
            case Commands.ping:
                await ctx.CreateResponseAsync(
                    new DiscordEmbedBuilder()
                    .WithTitle("help")
                    .WithDescription("`ping`")
                    .AddField("Description", "Tests bot latency")
                    .WithColor(0xFADADD)
                    );
                break;
            case Commands.removerole:
                await ctx.CreateResponseAsync(
                    _QuickBuild(
                        ctx,
                        "removerole",
                        "Removes a role from a user",
                        new() { ("user", "User to effect"), ("role", "Role to remove"), ("reason", "Reason for the role removal (optional)") },
                        Permissions.ManageRoles
                        )
                    );
                break;
            case Commands.rename:
                await ctx.CreateResponseAsync(
                    _QuickBuild(
                        ctx,
                        "rename",
                        "Renames a user",
                        new() { ("user", "User to rename"), ("name", "New nickname for the user"), ("reason", "Reason for the name change") },
                        Permissions.ManageNicknames
                        )
                    );
                break;
            case Commands.repo:
                await ctx.CreateResponseAsync(
                    new DiscordEmbedBuilder()
                    .WithTitle("Help")
                    .WithDescription("`repo`")
                    .AddField("Description", "Links the bots GitHub repo")
                    .WithColor(0xFADADD)
                    );
                break;
            case Commands.timeout:
                await ctx.CreateResponseAsync(
                    _QuickBuild(
                        ctx,
                        "timeout",
                        "Timeouts a user",
                        new() { ("user", "User to timeout"), ("length", "Amount of time to timeout a user"), ("reason", "Reason for timing out a user") },
                        Permissions.ModerateMembers
                        )
                    );
                break;
            case Commands.unban:
                await ctx.CreateResponseAsync(
                    _QuickBuild(
                        ctx,
                        "unban",
                        "Unbans a user",
                        new() { ("user", "User to unban"), ("reason", "Reason for unbanning `user`") },
                        Permissions.BanMembers
                        )
                    );
                break;
            case Commands.userinfo:
                await ctx.CreateResponseAsync(
                    new DiscordEmbedBuilder()
                        .WithTitle("Help")
                        .WithDescription("`userinfo`")
                        .AddField("Arguments", "`user` -> User to grab logs for (optional)")
                        .AddField("Description", "Shows information about a user")
                        .WithColor(0xFADADD)
                    );
                break;
            case Commands.warn:
                await ctx.CreateResponseAsync(
                    _QuickBuild(ctx, "warn", "Warns a user",
                    new() { ("user", "User to warn"), ("warning", "Warning to issue") }, Permissions.ModerateMembers)
                    );
                break;
            default:
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Oopsie. Something isn't right"));
                throw new Exception();
        }

    }
}