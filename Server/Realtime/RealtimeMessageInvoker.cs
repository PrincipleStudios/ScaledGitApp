using Microsoft.AspNetCore.SignalR;
using PrincipleStudios.ScaledGitApp.Commands;

namespace PrincipleStudios.ScaledGitApp.Realtime;

public class RealtimeMessageInvoker : IRealtimeMessageInvoker
{
	private readonly InstanceCommandInvoker<IRealtimeMessageContext> invoker;

	public RealtimeMessageInvoker(IHubContext<FullHub> hubContext, ILogger<RealtimeMessageInvoker> logger)
	{
		invoker = new InstanceCommandInvoker<IRealtimeMessageContext>(
			new RealtimeMessageContext(hubContext),
			logger
		);
	}

	public Task RunCommand(ICommand<Task, IRealtimeMessageContext> command) =>
		invoker.RunCommand(command);

	public Task<T> RunCommand<T>(ICommand<Task<T>, IRealtimeMessageContext> command) =>
		invoker.RunCommand(command);
}

public class RealtimeMessageContext(IHubContext<FullHub> hubContext) : IRealtimeMessageContext
{
	public IHubContext<FullHub> HubContext => hubContext;
}

public interface IRealtimeMessageContext
{
	IHubContext<FullHub> HubContext { get; }
}