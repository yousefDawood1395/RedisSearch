# 🔍 RedisSearchService

A .NET class library that provides Redis-based full-text search capabilities using [RediSearch](https://redis.io/docs/interact/search/) with RedisJSON.

## 🧰 Features

- 🌐 Full-text search using RediSearch
- ⚡ High-speed in-memory indexing with Redis
- 🧩 Flexible filters using tags, ranges, and full-text
- 📦 Supports RedisJSON documents
- ✅ Built with dependency injection in mind (ILogger, IConfiguration)

---

## 🚀 Getting Started

### 1. Install the NuGet Package

🧱 Dependencies
StackExchange.Redis

RedisJSON / RediSearch

Microsoft.Extensions.Logging

Microsoft.Extensions.Configuration


```bash
dotnet add package RedisSearchService
```
🔹 3. In the main app, configure it from appsettings.json or environment:
```json
{
  "Redis": {
    "Server": "localhost:6379",
    "PoolSize": 8,
    "Strategy": "LeastLoaded"
  }
}
```
👨‍💻 Contributing
Fork the repo

Create a feature branch (git checkout -b feature/your-feature)

Commit your changes

Open a Pull Request
