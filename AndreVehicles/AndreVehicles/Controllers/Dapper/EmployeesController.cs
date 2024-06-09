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
        private readonly Config DapperFile;

        EmployeesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Dapper\Query.json"))
            {
                string json = reader.ReadToEnd();
                DapperFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = DapperFile.ConnectionString;
            }
        }

        [HttpGet("employee/dapper")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployee()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var employees = await connection.QueryAsync<Employee>(DapperFile.Query.Employee.GET);
                return Ok(employees);
            }
        }

        [HttpGet("employee/dapper/{document}")]
        public async Task<ActionResult<Employee>> GetEmployee(string document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var employee = await connection.QueryFirstOrDefaultAsync<Employee>(DapperFile.Query.Employee.GETBYID, new { Document = document });

                if (employee == null)
                {
                    return NotFound();
                }

                return Ok(employee);
            }
        }

        [HttpPut("employee/dapper/{document}")]
        public async Task<IActionResult> PutEmployee(string document, Employee employee)
        {
            if (document.Equals(employee.Document))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.Employee.UPDATE, employee);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("employee/dapper/")]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var document = await connection.QuerySingleAsync<string>(DapperFile.Query.Employee.INSERT, employee);

                employee.Document = document;

                return CreatedAtAction("GetEmployee", new { Document = employee.Document }, employee);
            }
        }

        [HttpDelete("employee/dapper/{document}")]
        public async Task<IActionResult> DeleteEmployee(string document)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.Employee.DELETE, new { Document = document });

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
                return await connection.ExecuteScalarAsync<bool>(DapperFile.Query.Employee.EXISTS, new { Document = document });
            }
        }
    }
}
