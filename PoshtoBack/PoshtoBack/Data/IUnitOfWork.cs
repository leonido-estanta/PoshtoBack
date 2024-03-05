namespace PoshtoBack.Data;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IMessageRepository Messages { get; }
    int Save();
}