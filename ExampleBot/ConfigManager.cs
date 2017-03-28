using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ExampleBot
{
	// Our ConfigProperties class
	// Opt in is important, it makes us have to define which properties we want to include
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	sealed class ConfigProperties
	{
		// A token is always required
		[JsonProperty(Required = Required.Always, PropertyName = "token")]
		public string Token { get; set; }
		// A version is not always required, but shouldn't be null either
		[JsonProperty(Required = Required.DisallowNull, PropertyName = "version")]
		public string Version { get; set; }

		private string _configPath { get; set; }
		public string ConfigPath => Path.Combine(AppContext.BaseDirectory, _configPath);

		[JsonConstructor]
		public ConfigProperties(string token = null, string version = null, string configName = null)
		{
			Token = token ?? string.Empty;
			Version = version ?? string.Empty;
			_configPath = configName ?? "config.json";
		}

		// Quick method for parsing
		public static ConfigProperties Parse(string value) =>
			JsonConvert.DeserializeObject<ConfigProperties>(value);
		// Quick method for serializing
		public string Serialize() =>
			JsonConvert.SerializeObject(this, Formatting.Indented);
	}

    internal static class ConfigManager
    {
		internal static ConfigProperties properties = new ConfigProperties();

		public static Task Read()
		{
			// Throw an exception if our config file does not exist
			if (!File.Exists(properties.ConfigPath))
				throw new FileNotFoundException($"File {properties.ConfigPath} not found.");

			// Read our config json
			var json = File.ReadAllText(properties.ConfigPath);
			// Parse read json
			var data = ConfigProperties.Parse(json);

			// Set values
			properties.Token = data.Token;
			properties.Version = data.Version;

			return Task.CompletedTask;
		}

		// Try to write new config data, you shouldn't really use this, just here for completion's sake
		public static Task Write()
		{
			var data = properties.Serialize();
			File.WriteAllText(properties.ConfigPath, data);
			return Task.CompletedTask;
		}
    }
}
