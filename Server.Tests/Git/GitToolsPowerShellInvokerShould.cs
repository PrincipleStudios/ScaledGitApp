using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PrincipleStudios.ScaledGitApp.Commands;

namespace PrincipleStudios.ScaledGitApp.Git;

public partial class GitToolsPowerShellInvokerShould
{
	private readonly GitToolsPowerShellInvokerFixture fixture = new GitToolsPowerShellInvokerFixture();

	[Fact]
	public async Task Instantiate_without_immediately_invoking()
	{
		var calledFactory = false;
		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns((IPowerShell)null!);

		var target = new GitToolsCommandInvoker(
			Options.Create(fixture.GitOptions),
			fixture.MockPowerShellFactory.Object,
			() =>
			{
				calledFactory = true;
				return Task.FromResult(fixture.CloneConfiguration);
			},
			Mock.Of<ILogger<GitToolsCommandInvoker>>(),
			Mock.Of<StubCommandCache>()
		);

		await Task.Yield();

		fixture.MockPowerShellFactory.Verify(ps => ps.Create(null), Times.Never());
		Assert.False(calledFactory);
	}

	/// <summary>
	/// Verifies that the actual command with the mock that was passed
	/// </summary>
	[Fact]
	public async Task Executes_the_target_command()
	{
		var expectedResult = "foo";
		var mockFinal = new Mock<IPowerShell>();
		fixture.MockPowerShellFactory.Setup(ps => ps.Create(null)).Returns(mockFinal.Object);

		var mockCommand = new Mock<IGitToolsCommand<Task<string>>>();
		var verifiableCommand = mockCommand.Verifiable(cmd => cmd.RunCommand(It.IsAny<IGitToolsCommandContext>()), s => s.ReturnsAsync(expectedResult));

		// By mocking the factory directly, we skip working directory detection
		var target = fixture.CreateTarget();

		var result = await target.RunCommand(mockCommand.Object);

		// Assert that we got the expected result from the command because the value was passed through
		Assert.Equal(expectedResult, result);
		verifiableCommand.Verify(Times.Once);
	}
}
