using PoshtoBack.Data.Models;

namespace PoshtoBack.Data;

public class VoiceChannelRepository : Repository<VoiceChannel>, IVoiceChannelRepository
{
    public VoiceChannelRepository(PoshtoDbContext context) : base(context)
    {
    }
}