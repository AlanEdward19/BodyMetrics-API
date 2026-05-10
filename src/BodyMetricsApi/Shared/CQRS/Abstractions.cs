namespace BodyMetricsApi.Shared.CQRS;

public interface ICommand;

public interface ICommand<out TResponse> : ICommand;

public interface IQuery<out TResponse>;

