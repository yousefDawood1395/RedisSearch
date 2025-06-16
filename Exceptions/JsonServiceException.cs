namespace RedisSearch.Exceptions
{
    [Serializable]
    public class JsonServiceException(string message) : Exception(message)
    {
    }
}
