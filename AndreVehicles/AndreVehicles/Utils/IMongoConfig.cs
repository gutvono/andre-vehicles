namespace AndreVehicles.Utils
{
    public interface IMongoConfig
    {
        string AddressCollection { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
