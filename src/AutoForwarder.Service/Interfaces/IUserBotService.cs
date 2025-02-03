namespace AutoForwarder.Service.Interfaces;

public interface IUserBotService
{
    Task ForwardMessageAsync(CancellationToken cancellationToken);
}
