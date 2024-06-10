using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        CustomersController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("customer/adonet")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
            var customers = new List<Customer>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Customer.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        customers.Add(new Customer
                        {
                            Document = reader.GetString(0),
                            PdfDocument = reader.GetString(1),
                            Income = reader.GetDecimal(2),
                            Name = reader.GetString(3),
                            BirthDate = reader.GetString(4),
                            Address = new Address{ Id = reader.GetInt32(5) },
                            Email = reader.GetString(6)
                        });
                    }
                }
            }
            return Ok(customers);
        }

        [HttpGet("customer/adonet/{document}")]
        public async Task<ActionResult<Customer>> GetCustomer(string document)
        {
            Customer customer = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Customer.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Customer", document);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        customer = new Customer
                        {
                            Document = reader.GetString(0),
                            PdfDocument = reader.GetString(1),
                            Income = reader.GetDecimal(2),
                            Name = reader.GetString(3),
                            BirthDate = reader.GetString(4),
                            Address = new Address { Id = reader.GetInt32(5) },
                            Email = reader.GetString(6)
                        };
                    }
                }
            }

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPut("customer/adonet/{document}")]
        public async Task<IActionResult> PutCustomer(string document, Customer customer)
        {
            if (document != customer.Document)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Customer.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Document", customer.Document);
                cmd.Parameters.AddWithValue("@PdfDocument", customer.PdfDocument);
                cmd.Parameters.AddWithValue("@Income", customer.Income);
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@BirthDate", customer.BirthDate);
                cmd.Parameters.AddWithValue("@Address", customer.Address.Id);
                cmd.Parameters.AddWithValue("@Email", customer.Email);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("customer/adonet")]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Customer.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@Document", customer.Document);
                cmd.Parameters.AddWithValue("@PdfDocument", customer.PdfDocument);
                cmd.Parameters.AddWithValue("@Income", customer.Income);
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@BirthDate", customer.BirthDate);
                cmd.Parameters.AddWithValue("@Address", customer.Address.Id);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                var document = (string)await cmd.ExecuteScalarAsync();

                customer.Document = document;
                return CreatedAtAction("PostCustomer", new { Document = customer.Document }, customer);
            }
        }

        [HttpDelete("customer/adonet/{document}")]
        public async Task<IActionResult> DeleteCustomer(string document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Customer.DELETE, connection);
                cmd.Parameters.AddWithValue("@Document", document);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> CustomerExists(string document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Customer.EXISTS, connection);
                cmd.Parameters.AddWithValue("@CustomerNumber", document);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
