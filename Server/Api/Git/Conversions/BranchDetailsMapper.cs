using PrincipleStudios.ScaledGitApp.BranchingStrategy;
using PrincipleStudios.ScaledGitApp.Git.ToolsCommands;

namespace PrincipleStudios.ScaledGitApp.Api.Git.Conversions;

public interface IBranchDetailsMapper
{
	BranchDetails ToBranchDetails(UpstreamBranchDetailedState state);
}

public class BranchDetailsMapper(IBranchTypeLookup branchTypeLookup) : IBranchDetailsMapper
{
	public BranchDetails ToBranchDetails(UpstreamBranchDetailedState state)
	{
		var type = branchTypeLookup.DetermineBranchType(state.Name);

		return new BranchDetails(
			state.Name,
			Type: type.BranchType,
			Color: type.Color,
			Exists: state.Exists,
			NonMergeCommitCount: state.NonMergeCommitCount,
		Upstream: from upstream in state.Upstreams
					  let upstreamType = branchTypeLookup.DetermineBranchType(upstream.Name)
					  select new DetailedUpstreamBranch(
						  Name: upstream.Name,
						  Type: upstreamType.BranchType,
						  Color: upstreamType.Color,
						  Exists: upstream.Exists,
						  BehindCount: upstream.BehindCount,
						  HasConflict: upstream.HasConflict
					  ),
			Downstream: from downstream in state.DownstreamNames
						let downstreamType = branchTypeLookup.DetermineBranchType(downstream)
						select new Branch(Name: downstream, Color: downstreamType.Color, Type: downstreamType.BranchType)
		);
	}
}
