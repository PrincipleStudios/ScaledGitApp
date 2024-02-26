using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Locales;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PrincipleStudios.ScaledGitApp.Api.Locales;

public class LocalesController(ILocaleLoader localeLoader) : ApiLocalesControllerBase
{
	protected override async Task<GetTranslationDataActionResult> GetTranslationData(string lng, string ns)
	{
		var namespaces = ns.Split(' ');
		var languages = lng.Split(' ');

		var allData = await localeLoader.LoadTranslationData(languages, namespaces);

		return GetTranslationDataActionResult.Ok(
			new(
				from t in allData
				where t.Content != null
				group (t.Namespace, Content: t.Content!) by t.Language into byLanguage
				select new KeyValuePair<string, Dictionary<string, JsonNode>>(
					byLanguage.Key,
					byLanguage.ToDictionary(e => e.Namespace, e => e.Content)
				)
		));
	}

}
