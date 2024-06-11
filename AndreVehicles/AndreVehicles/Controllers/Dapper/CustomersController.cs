using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
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

        [HttpGet("dapper")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var customer = await connection.QueryAsync<Customer>(QueryFile.Query.Customer.GET);
                return Ok(customer);
            }
        }

        [HttpGet("dapper/{document}")]
        public async Task<ActionResult<Customer>> GetCustomer(string document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var customer = await connection.QueryFirstOrDefaultAsync<Customer>(QueryFile.Query.Customer.GETBYID, new { Document = document });

                if (customer == null)
                {
                    return NotFound();
                }

                return Ok(customer);
            }
        }

        [HttpPut("dapper/{document}")]
        public async Task<IActionResult> PutCustomer(string document, Customer customer)
        {
            if (document.Equals(customer.Document))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Customer.UPDATE, customer);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("dapper/")]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var document = await connection.QuerySingleAsync<string>(QueryFile.Query.Customer.INSERT, customer);

                customer.Document = document;

                return CreatedAtAction("GetCustomer", new { Document = customer.Document }, customer);
            }
        }

        [HttpDelete("dapper/{document}")]
        public async Task<IActionResult> DeleteCustomer(string document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Customer.DELETE, new { Document = document });

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
                return await connection.ExecuteScalarAsync<bool>(QueryFile.Query.Customer.EXISTS, new { Document = document });
            }
        }
    }
}
