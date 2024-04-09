using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Runtime.CompilerServices;

namespace PrincipleStudios.ScaledGitApp.Commands;

public sealed class CommandCache : ICommandCache, IDisposable
{
	private readonly IMemoryCache memoryCache;
	private readonly CommandCacheOptions options;
	private IChangeToken changeToken;
	private CancellationTokenSource cancellationSource;

	public CommandCache(IMemoryCache memoryCache, IOptions<CommandCacheOptions> options)
	{
		(cancellationSource, changeToken) = CreateChangeToken();
		this.memoryCache = memoryCache;
		this.options = options.Value;
	}

	private static (CancellationTokenSource, IChangeToken) CreateChangeToken()
	{
		var source = new CancellationTokenSource();
		return (source, new CancellationChangeToken(source.Token));
	}

	public void ClearAll()
	{
		var oldCancellationSource = cancellationSource;
		(cancellationSource, changeToken) = CreateChangeToken();
		oldCancellationSource.Cancel();
		oldCancellationSource.Dispose();
	}

	public async Task<T> GetCommandResultOrInvoke<T, TContext>(ICommand<Task<T>, TContext> command, Func<Task<T>> invoke)
	{
		if (!IsEquatable(command)) return await invoke();
		if (!IsCacheable(command)) return await invoke();

		var result = await memoryCache.GetOrCreateAsync(command, (entry) =>
		{
			entry.AddExpirationToken(changeToken);
			return invoke();
		});
		// Nullability seems incorrect on GetOrCreate; should use the same result as the invoke
		return result!;
	}

	private bool IsCacheable<T, TContext>(ICommand<Task<T>, TContext> command)
	{
		var result = options.DefaultEnabled;
		if (options.TypeSettings?.GetValue<bool?>(command.GetType().Name) is bool typeOverride)
			result = typeOverride;
		return result;
	}

	private static bool IsEquatable(object target)
	{
		return target.GetType().FindInterfaces(EquatableFilter, target.GetType()).Length > 0;
	}

	private static bool EquatableFilter(Type targetType, object? source)
	{
		if (source is not Type sourceType) return false;

		if (!targetType.IsGenericType) return false;
		return targetType.GetGenericTypeDefinition() == typeof(IEquatable<>) && targetType.GetGenericArguments()[0].IsAssignableFrom(sourceType);
	}

	public void Dispose()
	{
		cancellationSource?.Dispose();
	}
}
