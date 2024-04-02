using Microsoft.AspNetCore.SignalR;
using PrincipleStudios.ScaledGitApp.Commands;

namespace PrincipleStudios.ScaledGitApp.Realtime;

public class RealtimeMessageInvoker(IHubContext<FullHub> hubContext, ILogger<RealtimeMessageInvoker> logger)
	: CommandInvoker<IRealtimeMessageContext>(logger)
{
	protected override Task<T> RunGenericCommand<T>(ICommand<T, IRealtimeMessageContext> command) =>
		RunGenericCommand(command, new RealtimeMessageContext(hubContext));
}

public class RealtimeMessageContext(IHubContext<FullHub> hubContext) : IRealtimeMessageContext
{
	public IHubContext<FullHub> HubContext => hubContext;
}

public interface IRealtimeMessageContext
{
	IHubContext<FullHub> HubContext { get; }
}