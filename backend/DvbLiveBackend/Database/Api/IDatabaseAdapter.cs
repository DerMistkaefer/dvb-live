using DerMistkaefer.DvbLive.Backend.Database.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DerMistkaefer.DvbLive.Backend.Database.Api
{
    public interface IDatabaseAdapter
    {
        Task ClearDatabase();

        Task InsertStopPoint(StopPoints entity);

        Task<List<StopPoints>> GetAllStopPoints();
    }
}
