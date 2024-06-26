﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("dapper")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRole()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var roles = await connection.QueryAsync<Role>(QueryFile.Query.Role.GET);
                return Ok(roles);
            }
        }

        [HttpGet("dapper/{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var role = await connection.QueryFirstOrDefaultAsync<Role>(QueryFile.Query.Role.GETBYID, new { Id = id });

                if (role == null)
                {
                    return NotFound();
                }

                return Ok(role);
            }
        }

        [HttpPut("dapper/{id}")]
        public async Task<IActionResult> PutRole(int id, Role role)
        {
            if (id.Equals(role.Id))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Role.UPDATE, role);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("dapper/")]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(QueryFile.Query.Role.INSERT, role);

                role.Id = id;

                return CreatedAtAction("GetRole", new { Id = role.Id }, role);
            }
        }

        [HttpDelete("dapper/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Role.DELETE, new { Id = id });

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
                return await connection.ExecuteScalarAsync<bool>(QueryFile.Query.Role.EXISTS, new { Id = id });
            }
        }
    }
}
