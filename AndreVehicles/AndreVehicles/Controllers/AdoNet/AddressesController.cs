using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Model;
using System.Net;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        AddressesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("address/adonet")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            var addresses = new List<Address>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Address.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        addresses.Add(new Address
                        {
                            Id = reader.GetInt32(0),
                            Street = reader.GetString(1),
                            PostalCode = reader.GetString(2),
                            Neighborhood = reader.GetString(3),
                            StreetType = reader.GetString(4),
                            Number = reader.GetInt32(5),
                            Complement = reader.GetString(6),
                            State = reader.GetString(7),
                            City = reader.GetString(8)
                        });
                    }
                }
            }
            return Ok(addresses);
        }

        [HttpGet("address/adonet/{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            Address address = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Address.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        address = new Address
                        {
                            Id = reader.GetInt32(0),
                            Street = reader.GetString(1),
                            PostalCode = reader.GetString(2),
                            Neighborhood = reader.GetString(3),
                            StreetType = reader.GetString(4),
                            Number = reader.GetInt32(5),
                            Complement = reader.GetString(6),
                            State = reader.GetString(7),
                            City = reader.GetString(8)
                        };
                    }
                }
            }

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

        [HttpPut("address/adonet/{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Address.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Id", address.Id);
                cmd.Parameters.AddWithValue("@Street", address.Street);
                cmd.Parameters.AddWithValue("@PostalCode", address.PostalCode);
                cmd.Parameters.AddWithValue("@Neighborhood", address.Neighborhood);
                cmd.Parameters.AddWithValue("@StreetType", address.StreetType);
                cmd.Parameters.AddWithValue("@Number", address.Number);
                cmd.Parameters.AddWithValue("@Complement", address.Complement);
                cmd.Parameters.AddWithValue("@State", address.State);
                cmd.Parameters.AddWithValue("@City", address.City);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("address/adonet")]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Address.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@Street", address.Street);
                cmd.Parameters.AddWithValue("@CEP", address.PostalCode);
                cmd.Parameters.AddWithValue("@Neighborhood", address.Neighborhood);
                cmd.Parameters.AddWithValue("@StreetType", address.StreetType);
                cmd.Parameters.AddWithValue("@Complement", address.Complement);
                cmd.Parameters.AddWithValue("@Number", address.Number);
                cmd.Parameters.AddWithValue("@Uf", address.State);
                cmd.Parameters.AddWithValue("@City", address.City);
                var id = (int) await cmd.ExecuteScalarAsync();

                address.Id = id;
                return CreatedAtAction("PostAddress", new { id = address.Id }, address);
            }
        }

        [HttpDelete("address/adonet/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Address.DELETE, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> AddressExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Address.EXISTS, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
