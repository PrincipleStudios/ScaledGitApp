using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PrincipleStudios.ScaledGitApp.Locales;

public class LocaleLoader : ILocaleLoader
{
	private readonly LocalizationOptions options;
	public LocaleLoader(IOptions<LocalizationOptions> options)
	{
		this.options = options.Value;
	}

	public async Task<IEnumerable<LoadedTranslationData>> LoadTranslationData(string[] languages, string[] namespaces)
	{
		return await Task.WhenAll(
			from lng in languages
			from ns in namespaces
			select Load(lng: lng, ns: ns)
		);
		async Task<LoadedTranslationData> Load(string lng, string ns) =>
			new(lng, ns, await LoadNamespace(language: lng, ns: ns));
	}

	private async Task<JsonNode?> LoadNamespace(string language, string ns)
	{
		if (options.StandardPath != null)
		{
			var result = await Load(options.StandardPath
				.Replace("<lang>", language)
				.Replace("<namespace>", ns));
			if (result != null) return result;
		}
		if (options.BundlePath == null) return null;

		var bundle = await Load(options.BundlePath.Replace("<lang>", language));
		return bundle?[ns];
	}

	private static async Task<JsonNode?> Load(string filePath)
	{
		if (!System.IO.File.Exists(filePath)) return null;
		var fileContents = await System.IO.File.ReadAllTextAsync(filePath);
		return JsonSerializer.Deserialize<JsonNode>(fileContents);
	}
}
