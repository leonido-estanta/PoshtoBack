using PoshtoBack.Data.Models;

namespace PoshtoBack.Data;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(PoshtoDbContext context) : base(context)
    {
    }
}