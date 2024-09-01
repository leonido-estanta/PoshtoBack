namespace PoshtoBack.Data;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IMessageRepository Messages { get; }
    IVoiceRoomRepository VoiceRooms { get; }
    int Save();
}