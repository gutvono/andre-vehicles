using AndreVehicles.Utils;
using Model;
using Model.DTO;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace AndreVehicles.Services
{
    public class AddressService
    {
        private readonly IMongoCollection<Address> _address;
        static readonly HttpClient address = new HttpClient();

        public AddressService(IMongoConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            var db = client.GetDatabase(config.DatabaseName);
            _address = db.GetCollection<Address>(config.AddressCollection);
        }

        public async Task<AddressViacepDTO> GetViacepAddress(string cep)
        {
            try
            {
                HttpResponseMessage response = await address.GetAsync($"https://viacep.com.br/ws/{cep}/json/");
                response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<AddressViacepDTO>(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e) { throw; }
        }
    }
}
