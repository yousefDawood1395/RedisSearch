using StackExchange.Redis.MultiplexerPool;

namespace RedisSearch.Helper
{
    public class RedisOptions
    {
        public string ConnectionString { get; set; } = "";
        public int PoolSize { get; set; } = Environment.ProcessorCount;
        public ConnectionSelectionStrategy Strategy { get; set; } = ConnectionSelectionStrategy.LeastLoaded;
    }
}
