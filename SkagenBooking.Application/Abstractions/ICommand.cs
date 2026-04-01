namespace SkagenBooking.Application.Abstractions;

/// <summary>
/// Represents a request that changes application state and returns a response.
/// </summary>
/// <typeparam name="TResponse">Type of the response produced by the command.</typeparam>
public interface ICommand<TResponse> : IUseCase
{
}
