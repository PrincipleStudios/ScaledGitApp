using Moq;
using Moq.Language;
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
	/// <summary>
	/// Creates a verifiable setup so the setup expression does not get repeated
	/// </summary>
	/// <typeparam name="T">The type of the mock</typeparam>
	/// <param name="mock">The mock instance</param>
	/// <param name="setup">The setup expression</param>
	/// <param name="moreSetup">Additional setup, including return value</param>
	/// <returns>A record to invoke `Verify` on the existing mock</returns>
	public static VerifiableMock<T> Verifiable<T>(this Mock<T> mock, Expression<Action<T>> setup, Action<ISetup<T>>? moreSetup = null)
		where T : class
	{
		var invocation = mock.Setup(setup);
		moreSetup?.Invoke(invocation);

		return new(mock, setup);
	}

	/// <summary>
	/// Creates a verifiable setup so the setup expression does not get repeated
	/// </summary>
	/// <typeparam name="T">The type of the mock</typeparam>
	/// <typeparam name="TResult">Return result type of the action</typeparam>
	/// <param name="mock">The mock instance</param>
	/// <param name="setup">The setup expression</param>
	/// <param name="moreSetup">Additional setup, including return value</param>
	/// <returns>A record to invoke `Verify` on the existing mock</returns>
	public static VerifiableMock<T, TResult> Verifiable<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> setup, Action<ISetup<T, TResult>> moreSetup)
		where T : class
	{
		var invocation = mock.Setup(setup);
		moreSetup?.Invoke(invocation);

		return new(mock, setup);
	}

	/// <summary>
	/// Creates a verifiable setup for a sequence of calls so the setup expression does not get repeated
	/// </summary>
	/// <typeparam name="T">The type of the mock</typeparam>
	/// <param name="mock">The mock instance</param>
	/// <param name="setup">The setup expression</param>
	/// <param name="moreSetup">Additional setup, including return value</param>
	/// <returns>A record to invoke `Verify` on the existing mock</returns>
	public static VerifiableMock<T> VerifiableSequence<T>(this Mock<T> mock, Expression<Action<T>> setup, Action<ISetupSequentialAction>? moreSetup = null)
		where T : class
	{
		var invocation = mock.SetupSequence(setup);
		moreSetup?.Invoke(invocation);

		return new(mock, setup);
	}

	/// <summary>
	/// Creates a verifiable setup for a sequence of calls so the setup expression does not get repeated
	/// </summary>
	/// <typeparam name="T">The type of the mock</typeparam>
	/// <typeparam name="TResult">Return result type of the action</typeparam>
	/// <param name="mock">The mock instance</param>
	/// <param name="setup">The setup expression</param>
	/// <param name="moreSetup">Additional setup, including return value</param>
	/// <returns>A record to invoke `Verify` on the existing mock</returns>
	public static VerifiableMock<T, TResult> VerifiableSequence<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> setup, Action<ISetupSequentialResult<TResult>> moreSetup)
		where T : class
	{
		var invocation = mock.SetupSequence(setup);
		moreSetup?.Invoke(invocation);

		return new(mock, setup);
	}
}