namespace MediScreenApi;

public class MongoDbConfig
{
    public string DatabaseName { get; set; }
    public string Host { get; init; }
    public int Port { get; init; }
    public string ConnectionString => Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING") ?? $"mongodb://{Host}:{Port}";
}