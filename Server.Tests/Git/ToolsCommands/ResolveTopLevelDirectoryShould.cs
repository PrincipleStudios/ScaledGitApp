
using Moq;
using PrincipleStudios.ScaledGitApp.ShellUtilities;

namespace PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

public class ResolveTopLevelDirectoryShould
{
	private readonly PowerShellFixture fixture = new PowerShellFixture();

	[Fact]
	public async Task Resolve_the_top_level_directoryAsync()
	{
		// These "real" directories are used to ensure file path separators, etc. do not break the test
		var baseWorkingDirectory = Directory.GetCurrentDirectory();
		var expectedWorkingDirectory = Path.TrimEndingDirectorySeparator(Path.GetTempPath());
		fixture.MockPowerShell.Setup(ps => ps.PowerShellInvoker.SetCurrentWorkingDirectory(baseWorkingDirectory));
		var verifiable = fixture.MockPowerShell.Verifiable(
			ps => ps.PowerShellInvoker.InvokeCliAsync("git", "rev-parse", "--show-toplevel"),
			s => s.ReturnsAsync(PowerShellInvocationResultStubs.WithResults(expectedWorkingDirectory))
		);
		var target = new ResolveTopLevelDirectory(baseWorkingDirectory);

		var actual = await target.RunCommand(fixture.Create());

		Assert.Equal(expectedWorkingDirectory, actual);
		verifiable.Verify(Times.Once);
	}
}
