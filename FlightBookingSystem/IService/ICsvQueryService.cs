using System.Data;

namespace FlightBookingSystem.IService
{
    public interface ICsvQueryService
    {
        Task<DataTable> ExecuteSqlOnCsvAsync(string sqlQuery);
    }
}
