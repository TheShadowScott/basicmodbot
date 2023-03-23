using static ModBot.Program;
namespace ModBot;

public sealed partial class DiscordCommands : ApplicationCommandModule
{
    [SlashCommand("modmail", "Starts a new modmail forum channel.")]
    public static async Task NewModMail(InteractionContext ctx, string name, string message)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        try
        {
            var a = await ctx.Client.GetChannelAsync(LocalSettings.ModMailSettings.ChannelId);
            var msgBuilder = new DiscordMessageBuilder()
                .WithContent($"Post Created by <@{ctx.User.Id}>")
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithDescription(message)
                    .WithAuthor(ctx.User.Username + '#' + ctx.User.Discriminator, iconUrl: ctx.User.AvatarUrl)
                    .WithFooter("ModMail")
                    .WithColor(DiscordColor.Green)
                    .WithTimestamp(DateTime.Now)
                    .Build());
            DiscordForumPostStarter? channel = await (a as DiscordForumChannel)?.CreateForumPostAsync(new ForumPostBuilder()
                .WithName(name)
                .WithMessage(msgBuilder)
                )!;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("An error occurred while creating the modmail."));
        }
    }
}