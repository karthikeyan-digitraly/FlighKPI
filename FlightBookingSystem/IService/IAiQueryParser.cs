namespace FlightBookingSystem.IService
{
    public interface IAiQueryParser
    {
        Task<string> GenerateSqlQueryAsync(string userPrompt);
    }
}
