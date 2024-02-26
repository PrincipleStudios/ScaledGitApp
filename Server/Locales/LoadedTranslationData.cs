using System.Text.Json.Nodes;

namespace PrincipleStudios.ScaledGitApp.Locales;

public record struct LoadedTranslationData(string Language, string Namespace, JsonNode? Content);