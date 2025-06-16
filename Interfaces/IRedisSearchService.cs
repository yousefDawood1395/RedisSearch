namespace RedisSearch.Interfaces
{
    public interface IRedisSearchService
    {
        Task<(int totalCount, List<TOUT> results)> Search<TOUT>(string indexName, string query, int offset = 0, int limit = 10, string sortBy = "", string order = "ASC") where TOUT : class;
        Task<Dictionary<string, long>> Aggregate(string indexName, string query, string fieldToGroupBy);
    }
}
