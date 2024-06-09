using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config DapperFile;

        RolesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Dapper\Query.json"))
            {
                string json = reader.ReadToEnd();
                DapperFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = DapperFile.ConnectionString;
            }
        }

        [HttpGet("role/dapper")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRole()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var roles = await connection.QueryAsync<Role>(DapperFile.Query.Role.GET);
                return Ok(roles);
            }
        }

        [HttpGet("role/dapper/{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var role = await connection.QueryFirstOrDefaultAsync<Role>(DapperFile.Query.Role.GETBYID, new { Id = id });

                if (role == null)
                {
                    return NotFound();
                }

                return Ok(role);
            }
        }

        [HttpPut("role/dapper/{id}")]
        public async Task<IActionResult> PutRole(int id, Role role)
        {
            if (id.Equals(role.Id))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.Role.UPDATE, role);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("role/dapper/")]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(DapperFile.Query.Role.INSERT, role);

                role.Id = id;

                return CreatedAtAction("GetRole", new { Id = role.Id }, role);
            }
        }

        [HttpDelete("role/dapper/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.Role.DELETE, new { Id = id });

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> RoleExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<bool>(DapperFile.Query.Role.EXISTS, new { Id = id });
            }
        }
    }
}
