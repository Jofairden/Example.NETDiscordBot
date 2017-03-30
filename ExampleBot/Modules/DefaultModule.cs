using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Discord.Commands;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace ExampleBot.Modules
{
    public class DefaultModule : ExampleModuleBase<SocketCommandContext>
	{
		[Command("invite")]
		[Summary("Returns the OAuth2 Invite URL of the bot")]
		public async Task Invite()
		{
			var application = await Client.GetApplicationInfoAsync();
			await ReplyAsync(
				$"My invite link: <https://discordapp.com/oauth2/authorize?client_id={application.Id}&scope=bot>");
		}

		[Command("testsanitize")]
		[Summary("Will test our sanitation on our ReplyAsync override")]
		public async Task Testsanitize()
		{
			await ReplyAsync($"Testing @everyone and @here ({Context.Guild.EveryoneRole.Mention})");
		}

		// Say something
		// All examplebots have this for some reason
		[Command("say")]
		[Alias("echo")]
		public async Task Say([Remainder] string content = "") => await ReplyAsync($"```{content}```");

		//[Command("help")]
		//[Summary("Simple help command")]
		//public async Task Help()
		//{
			
		//}
	}
}
