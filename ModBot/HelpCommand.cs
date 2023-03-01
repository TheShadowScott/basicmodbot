using DSharpPlus.SlashCommands;
using DSharpPlus;
using DSharpPlus.Entities;
using DiscordBot;

namespace ModBot;

public sealed partial class DiscordCommands
{
    public enum Commands
    {
        all,
        addrole,
        alterlog,
        ban,
        droplog,
        fetch,
        kick,
        modmail,
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
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        switch(command)
        {
            case Commands.all:
                await ctx.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .WithContent($$"""
                    Supported Commands on this bot are:
                    ```md
                    1. /addrole
                    2. /alterlog
                    3. /ban
                    4. /droplog
                    5. /fetch
                    6. /kick
                    7. /modmail
                    8. /note
                    9. /ping
                    10. /removerole
                    11. /rename
                    12. /repo
                    13. /timeout
                    14. /unban
                    15. /userinfo
                    16. /warn
                    ```
                    For more information on a specific command use `/help <Command>`.
                    """)
                    );
                break;
            case Commands.addrole:
                await ctx.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .AddEmbed(
                        new DiscordEmbedBuilder()
                        .WithTitle("Help")
                        .AddField("Default Permissions", "`ManageRoles`", true)
                        .AddField("Can be used?", ctx.CheckPermissions(Permissions.ManageRoles) ? "yes" : "no", true)
                        .AddField("Arguments", """
                        `user` -> User to give `role`;
                        `role` -> Role to give to `user`
                        """)
                        .AddField("Description", "Adds a role to a given user")
                        .WithColor(0xFADADD)
                        )
                    );
                break;
            case Commands.alterlog:
                await ctx.EditResponseAsync
            default:
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Oopsie. Something isn't right"));
                throw new Exception();
        }

    }
}