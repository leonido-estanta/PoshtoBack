using PoshtoBack.Data.Models;

namespace PoshtoBack.Data;

public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(PoshtoDbContext context) : base(context)
    {
    }
}