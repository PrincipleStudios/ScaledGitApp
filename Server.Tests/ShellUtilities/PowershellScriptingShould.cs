using PrincipleStudios.ScaledGitApp.ShellUtililties;
using System.Collections;
using System.Management.Automation;
using System.Text.Json.Nodes;

namespace PrincipleStudios.ScaledGitApp.ShellUtilities;

public class PowershellScriptingShould
{
	const string relativePathToDemo = @"ShellUtilities/demo.ps1";
	const string defaultRequiredValue = "provided";
	private readonly PowerShellFactory psFactory;
	private readonly string absolutePathToDemoScript;
	private readonly string expectedPSScriptRoot;
	private readonly string expectedTempDirectory;

	public PowershellScriptingShould()
	{
		psFactory = new PowerShellFactory();
		absolutePathToDemoScript = Path.Join(Directory.GetCurrentDirectory(), relativePathToDemo);
		expectedPSScriptRoot = Path.TrimEndingDirectorySeparator(
			Path.GetDirectoryName(absolutePathToDemoScript)
				?? throw new InvalidOperationException("Unexpected directory")
		);
		expectedTempDirectory = Path.TrimEndingDirectorySeparator(Path.GetTempPath());
	}

	[Fact]
	public async Task Run_an_internal_script()
	{
		// The internal script returns an object with the same object JSON Serialized on the "Information" stream.
		using var ps = psFactory.Create();

		var result = await InvokeDemoScript(ps);
		Assert.NotNull(GetReturnedPSBoundParameters(result));
		Assert.Collection(
			result.Results, (resultObject) =>
				Assert.IsType<Hashtable>(resultObject.BaseObject)
		);
	}

	[Fact]
	public async Task Uses_the_specified_working_directory()
	{
		// Sets the working directory to a temp directory and makes sure the script can access it
		using var ps = psFactory.Create();
		ps.SetCurrentWorkingDirectory(expectedTempDirectory);

		var result = await InvokeDemoScript(ps);
		Assert.Collection(
			result.Results, (resultObject) =>
			{
				var data = Assert.IsType<Hashtable>(resultObject.BaseObject);
				var fieldObj = Assert.IsType<PSObject>(data["WorkingDirectory"]);
				var pathInfo = Assert.IsType<PathInfo>(fieldObj.BaseObject);
				Assert.Equal(expectedTempDirectory, Path.TrimEndingDirectorySeparator(pathInfo.Path));
			}
		);
	}

	[Fact]
	public async Task Has_the_correct_PSScriptRoot()
	{
		// Ensures that the script gets the correct $PSScriptRoot - which is the folder where the ps1 script is located.
		using var ps = psFactory.Create();

		var result = await InvokeDemoScript(ps);

		Assert.Collection(
			result.Results, (resultObject) =>
			{
				var data = Assert.IsType<Hashtable>(resultObject.BaseObject);
				var pathInfo = Assert.IsType<string>(data["PSScriptRoot"]);
				Assert.Equal(expectedPSScriptRoot, Path.TrimEndingDirectorySeparator(pathInfo));
			}
		);
	}

	[Fact]
	public async Task Receives_default_parameters()
	{
		// Ensures that the default parameters are received and can be passed back
		using var ps = psFactory.Create();

		var parameters = GetReturnedPSBoundParameters(await InvokeDemoScript(ps));

		var requiredParam = parameters["required"]?.GetValue<string>();
		Assert.Equal(defaultRequiredValue, requiredParam);
		Assert.Single(parameters);
	}

	[Fact]
	public async Task Fails_when_required_parameters_are_missing()
	{
		using var ps = psFactory.Create();

		await Assert.ThrowsAsync<ParameterBindingException>(async () => await InvokeDemoScript(ps, (ps) => { }));
	}

	[Fact]
	public async Task Receives_additional_parameters()
	{
		using var ps = psFactory.Create();

		var parameters = GetReturnedPSBoundParameters(await InvokeDemoScript(ps, (ps) =>
		{
			AddDefaultParameters(ps);
			ps.AddParameter("one", "1");
			ps.AddParameter("two", "2");
		}));

		Assert.Equal("1", parameters["one"]?.GetValue<string>());
		Assert.Equal("2", parameters["two"]?.GetValue<string>());
	}

	[Fact]
	public async Task Coerces_parameter_types()
	{
		using var ps = psFactory.Create();

		var parameters = GetReturnedPSBoundParameters(await InvokeDemoScript(ps, (ps) =>
		{
			AddDefaultParameters(ps);
			ps.AddParameter("one", 1);
		}));

		Assert.Equal("1", parameters["one"]?.GetValue<string>());
	}

	[Fact]
	public async Task Receives_strongly_typed_parameters()
	{
		// When types are left off of the parameter, ensures the same types are passed back to the calling app, at least within JSON.
		using var ps = psFactory.Create();

		var parameters = GetReturnedPSBoundParameters(await InvokeDemoScript(ps, (ps) =>
		{
			AddDefaultParameters(ps);
			ps.AddParameter("anyType", new object[] { "1", 2, "3" });
		}));

		Assert.Equal(defaultRequiredValue, parameters["required"]?.GetValue<string>());
		var targetArg = Assert.IsType<JsonArray>(parameters["anyType"]);
		Assert.Collection(targetArg,
			(node) => Assert.Equal("1", node?.GetValue<string>()),
			(node) => Assert.Equal(2, node?.GetValue<int>()),
			(node) => Assert.Equal("3", node?.GetValue<string>())
		);
		Assert.Equal(2, parameters.Count);
	}

	private static JsonObject GetReturnedPSBoundParameters(PowerShellInvocationResult result)
	{
		var writeHostResult = Assert.IsType<HostInformationMessage>(result.Streams.Information.SingleOrDefault()?.MessageData).Message;
		var node = JsonNode.Parse(writeHostResult);
		var parameters = node?["PSBoundParameters"];
		return Assert.IsType<JsonObject>(parameters);
	}

	private Task<PowerShellInvocationResult> InvokeDemoScript(IPowerShell ps, Action<PowerShell>? addParameters = null)
	{
		return ps.InvokeExternalScriptAsync(absolutePathToDemoScript, addParameters ?? AddDefaultParameters);
	}

	private void AddDefaultParameters(PowerShell shell)
	{
		shell.AddParameter("required", defaultRequiredValue);
	}
}
