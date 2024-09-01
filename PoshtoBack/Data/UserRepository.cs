using PoshtoBack.Data.Models;

namespace PoshtoBack.Data;

public class UserRepository(PoshtoDbContext context) : Repository<User>(context), IUserRepository
{
}