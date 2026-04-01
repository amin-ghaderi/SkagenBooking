namespace SkagenBooking.Application.Abstractions;

/// <summary>
/// Represents a read-only request that returns a response.
/// </summary>
/// <typeparam name="TResponse">Type of the data returned by the query.</typeparam>
public interface IQuery<TResponse> : IUseCase
{
}
