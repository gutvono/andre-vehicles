using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        RolesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("role/adonet")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRole()
        {
            var roles = new List<Role>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Role.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        roles.Add(new Role
                        {
                            Id = reader.GetInt32(0),
                            Description = reader.GetString(1)
                        });
                    }
                }
            }
            return Ok(roles);
        }

        [HttpGet("role/adonet/{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            Role role = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Role.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        role = new Role
                        {
                            Id = reader.GetInt32(0),
                            Description = reader.GetString(1)
                        };
                    }
                }
            }

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        [HttpPut("role/adonet/{id}")]
        public async Task<IActionResult> PutRole(int id, Role role)
        {
            if (id != role.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Role.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Id", role.Id);
                cmd.Parameters.AddWithValue("@Description", role.Description);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("role/adonet")]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Role.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@Description", role.Description);
                var id = (int)await cmd.ExecuteScalarAsync();

                role.Id = id;
                return CreatedAtAction("PostRole", new { id = role.Id }, role);
            }
        }

        [HttpDelete("role/adonet/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Role.DELETE, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

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
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Role.EXISTS, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
