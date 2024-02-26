
namespace PrincipleStudios.ScaledGitApp.Locales
{
	public interface ILocaleLoader
	{
		Task<IEnumerable<LoadedTranslationData>> LoadTranslationData(string[] languages, string[] namespaces);
	}
}