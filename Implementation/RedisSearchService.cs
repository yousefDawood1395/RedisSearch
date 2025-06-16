using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RedisSearch.Interfaces;
using StackExchange.Redis;
using StackExchange.Redis.MultiplexerPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using RedisSearch.Helper;
using RedisSearch.Exceptions;

namespace RedisSearch.Implementation
{
    public class RedisSearchService(IConfiguration _configuration,
                                    IConnectionMultiplexerPool _connectionPool,
                                    ILogger<RedisSearchService> _logger) : IRedisSearchService
    {

        public async Task<(int totalCount, List<TOUT> results)> Search<TOUT>(string indexName, string query, int offset = 0, int limit = 10, string sortBy = "", string order = "ASC") where TOUT : class
        {
            try
            {
                string crIndexName = _configuration[$"Redis:{indexName}"] ?? string.Empty;

                if (string.IsNullOrWhiteSpace(crIndexName))
                {
                    throw new Exception("Missing redis Index In Setting");
                }

                var connection = await _connectionPool.GetAsync();
                var _redisDB = connection.Connection.GetDatabase();

                int adjustedOffset = offset * limit;
                bool applySorting = !string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(order);
                RedisResult result;

                if (applySorting)
                    result = await _redisDB.ExecuteAsync(ResisSearchConstant.RedisSearch, crIndexName, query, ResisSearchConstant.SORTBY, sortBy, order, ResisSearchConstant.RedisSearchLimit, adjustedOffset, limit);
                else
                    result = await _redisDB.ExecuteAsync(ResisSearchConstant.RedisSearch, crIndexName, query, ResisSearchConstant.RedisSearchLimit, adjustedOffset, limit);

                var rawResult = (RedisResult[])result!;
                var totalCount = (int)rawResult[0]!;

                var models = new List<TOUT>();

                for (int i = 1; i < rawResult.Length; i += 2)
                {
                    var key = (string)rawResult[i]!; // Document key

                    RedisResult[] documentData = (RedisResult[])rawResult[i + 1]!;

                    var json = documentData.Length == 2
                                            ? (string)(documentData)[1]!
                                            : (string)(documentData)[3]!;

                    _logger.LogInformation("Date: {@DateNow}\n Key: {@key}", DateTime.Now, key);

                    // Deserialize the JSON to your model
                    models.Add(JsonSerializer.Deserialize<TOUT>(json, new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.Never
                    })!);
                }
                return (totalCount, models!);
            }
            catch (JsonException ex)
            {
                //_logger.LogError($"Error in field: {ex.Path} , Message: {ex.Message}");
                throw new JsonServiceException($"Error in field: {ex.Path} , Message: {ex.Message}");
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Exception Message: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        public async Task<Dictionary<string, long>> Aggregate(string indexName, string query, string fieldToGroupBy)
        {
            try
            {
                var kvp = new Dictionary<string, long>();
                string crIndexName = _configuration[$"Redis:{indexName}"] ?? string.Empty;
                if (string.IsNullOrWhiteSpace(crIndexName))
                {
                    throw new Exception("Missing redis Index In Setting");
                }
                var connection = await _connectionPool.GetAsync();
                var _redisDB = connection.Connection.GetDatabase();

                var result = await _redisDB.ExecuteAsync(ResisSearchConstant.RedisAggregate, crIndexName, query, "GROUPBY", "1", $"@{fieldToGroupBy}", "REDUCE", "COUNT", "0", "AS", "count");

                RedisResult[] rawResult = (RedisResult[])result!;
                if (rawResult.Length > 0 && rawResult[0].ToString() != "0")
                {
                    for (int i = 1; i < rawResult.Length; i++)
                    {
                        var entry = (RedisResult[])rawResult[i]!; // Extract the nested array

                        string key = ((RedisValue)entry[1]).ToString()!;
                        long value = long.Parse(entry[3].ToString()!);
                        kvp![key] = value;
                    }
                    //totalCount = (int)((RedisResult[])rawResult[1]!)[^1]!;
                }

                return kvp;
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Exception Message: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
    }
}
