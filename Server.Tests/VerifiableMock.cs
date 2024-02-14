using Moq;
using Moq.Language.Flow;
using System.Linq.Expressions;

namespace PrincipleStudios.ScaledGitApp;

public record VerifiableMock<T>(Mock<T> Mock, Expression<Action<T>> SetupExpression)
		where T : class
{
	public void Verify(Times times)
	{
		Mock.Verify(SetupExpression, times);
	}
	public void Verify(Func<Times> times)
	{
		Mock.Verify(SetupExpression, times);
	}
}
public record VerifiableMock<T, TResult>(Mock<T> Mock, Expression<Func<T, TResult>> SetupExpression)
		where T : class
{
	public void Verify(Times times)
	{
		Mock.Verify(SetupExpression, times);
	}
	public void Verify(Func<Times> times)
	{
		Mock.Verify(SetupExpression, times);
	}
}

public static class VerifiableMock
{
	public static VerifiableMock<T> Verifiable<T>(this Mock<T> mock, Expression<Action<T>> setup, Action<ISetup<T>>? moreSetup = null)
		where T : class
	{
		var invocation = mock.Setup(setup);
		moreSetup?.Invoke(invocation);

		return new(mock, setup);
	}

	public static VerifiableMock<T, TResult> Verifiable<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> setup, Action<ISetup<T, TResult>> moreSetup)
		where T : class
	{
		var invocation = mock.Setup(setup);
		moreSetup?.Invoke(invocation);

		return new(mock, setup);
	}
}