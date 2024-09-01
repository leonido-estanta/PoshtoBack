using PoshtoBack.Data.Models;

namespace PoshtoBack.Data;

public class MessageRepository(PoshtoDbContext context) : Repository<Message>(context), IMessageRepository;