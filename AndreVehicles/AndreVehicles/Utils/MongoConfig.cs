namespace AndreVehicles.Utils
{
    public class MongoConfig : IMongoConfig
    {
        public string AddressCollection { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
