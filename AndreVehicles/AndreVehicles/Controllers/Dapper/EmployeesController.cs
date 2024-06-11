using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
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

        [HttpGet("dapper")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var employees = await connection.QueryAsync<Employee>(QueryFile.Query.Employee.GET);
                return Ok(employees);
            }
        }

        [HttpGet("dapper/{document}")]
        public async Task<ActionResult<Employee>> GetEmployee(string document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var employee = await connection.QueryFirstOrDefaultAsync<Employee>(QueryFile.Query.Employee.GETBYID, new { Document = document });

                if (employee == null)
                {
                    return NotFound();
                }

                return Ok(employee);
            }
        }

        [HttpPut("dapper/{document}")]
        public async Task<IActionResult> PutEmployee(string document, Employee employee)
        {
            if (document.Equals(employee.Document))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Employee.UPDATE, employee);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("dapper/")]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var document = await connection.QuerySingleAsync<string>(QueryFile.Query.Employee.INSERT, employee);

                employee.Document = document;

                return CreatedAtAction("GetEmployee", new { Document = employee.Document }, employee);
            }
        }

        [HttpDelete("dapper/{document}")]
        public async Task<IActionResult> DeleteEmployee(string document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Employee.DELETE, new { Document = document });

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
                return await connection.ExecuteScalarAsync<bool>(QueryFile.Query.Employee.EXISTS, new { Document = document });
            }
        }
    }
}
