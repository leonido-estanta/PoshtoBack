using PoshtoBack.Data.Models;

namespace PoshtoBack.Data;

public class VoiceRoomRepository : Repository<VoiceRoom>, IVoiceRoomRepository
{
    public VoiceRoomRepository(PoshtoDbContext context) : base(context)
    {
    }
}