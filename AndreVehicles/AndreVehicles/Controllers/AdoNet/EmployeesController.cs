using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        EmployeesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("adonet")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            var employees = new List<Employee>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Employee.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employees.Add(new Employee
                        {
                            Document = reader.GetString(0),
                            ComissionValue = reader.GetDecimal(1),
                            Comission = reader.GetDecimal(2),
                            Name = reader.GetString(3),
                            BirthDate = reader.GetString(4),
                            Address = new Address { Id = reader.GetInt32(5) },
                            Email = reader.GetString(6)
                        });
                    }
                }
            }
            return Ok(employees);
        }

        [HttpGet("adonet/{document}")]
        public async Task<ActionResult<Employee>> GetEmployee(string document)
        {
            Employee employee = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Employee.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Employee", document);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        employee = new Employee
                        {
                            Document = reader.GetString(0),
                            ComissionValue = reader.GetDecimal(1),
                            Comission = reader.GetDecimal(2),
                            Name = reader.GetString(3),
                            BirthDate = reader.GetString(4),
                            Address = new Address { Id = reader.GetInt32(5) },
                            Email = reader.GetString(6)
                        };
                    }
                }
            }

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPut("adonet/{document}")]
        public async Task<IActionResult> PutEmployee(string document, Employee employee)
        {
            if (document != employee.Document)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Employee.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Document", employee.Document);
                cmd.Parameters.AddWithValue("@ComissionValue", employee.ComissionValue);
                cmd.Parameters.AddWithValue("@Comission", employee.Comission);
                cmd.Parameters.AddWithValue("@Name", employee.Name);
                cmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate);
                cmd.Parameters.AddWithValue("@Address", employee.Address.Id);
                cmd.Parameters.AddWithValue("@Email", employee.Email);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("adonet")]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Employee.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@Document", employee.Document);
                cmd.Parameters.AddWithValue("@ComissionValue", employee.ComissionValue);
                cmd.Parameters.AddWithValue("@Comission", employee.Comission);
                cmd.Parameters.AddWithValue("@Name", employee.Name);
                cmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate);
                cmd.Parameters.AddWithValue("@Address", employee.Address.Id);
                cmd.Parameters.AddWithValue("@Email", employee.Email);
                var document = (string)await cmd.ExecuteScalarAsync();

                employee.Document = document;
                return CreatedAtAction("PostEmployee", new { Document = employee.Document }, employee);
            }
        }

        [HttpDelete("adonet/{document}")]
        public async Task<IActionResult> DeleteEmployee(string document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Employee.DELETE, connection);
                cmd.Parameters.AddWithValue("@Document", document);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> EmployeeExists(string document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Employee.EXISTS, connection);
                cmd.Parameters.AddWithValue("@EmployeeNumber", document);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
