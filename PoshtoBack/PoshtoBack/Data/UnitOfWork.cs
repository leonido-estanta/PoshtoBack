namespace PoshtoBack.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly PoshtoDbContext _context;

    public UnitOfWork(PoshtoDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Messages = new MessageRepository(_context);
        VoiceRooms = new VoiceRoomRepository(_context);
    }

    public IUserRepository Users { get; private set; }
    public IMessageRepository Messages { get; private set; }
    public IVoiceRoomRepository VoiceRooms { get; private set; }

    public int Save()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}