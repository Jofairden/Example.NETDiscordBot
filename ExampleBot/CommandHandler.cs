using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace ExampleBot
{
	public class CommandHandler
	{
		public static char CharPrefix = '.';
		public CommandService Service;
		private DiscordSocketClient _client;
		private IDependencyMap _map;

		public async Task Install(IDependencyMap map)
		{
			// Create Command Service, inject it into Dependency Map
			_client = map.Get<DiscordSocketClient>();
			_map = map;
			// Creating a CommandServiceConfig is far from required here, just added it for completion's sake
			Service =
				new CommandService(new CommandServiceConfig
				{
					CaseSensitiveCommands = false,
					DefaultRunMode = RunMode.Async,
					LogLevel = LogSeverity.Verbose
				});

			// Finds modules in our assembly and adds them to our command service
			await Service.AddModulesAsync(Assembly.GetEntryAssembly());

			_client.MessageReceived += HandleCommand;
		}

		public async Task HandleCommand(SocketMessage parameterMessage)
		{
			// Don't handle the command if it is a system message
			var message = parameterMessage as SocketUserMessage;

			// Mark where the prefix ends and the command begins
			int argPos = 0;
			// Determine if the message has a valid prefix, adjust argPos 
			if (message == null
				|| message.IsWebhook
				|| message.Author.IsBot
				|| !(message.HasMentionPrefix(_client.CurrentUser, ref argPos)
				|| message.HasCharPrefix(CharPrefix, ref argPos)))
				return;

			// Create a Socket Command Context
			var context = new SocketCommandContext(_client, message);
			// Execute the Command, store the result
			var result = await Service.ExecuteAsync(context, argPos, _map, MultiMatchHandling.Exception);

			// If the command failed, notify the user
			if (!result.IsSuccess)
				await message.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
		}
	}
}
