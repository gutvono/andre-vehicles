using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class PixTypesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        PixTypesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("adonet")]
        public async Task<ActionResult<IEnumerable<PixType>>> GetPixType()
        {
            var pixtypes = new List<PixType>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.PixType.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        pixtypes.Add(new PixType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return Ok(pixtypes);
        }

        [HttpGet("adonet/{id}")]
        public async Task<ActionResult<PixType>> GetPixType(int id)
        {
            PixType pixtype = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.PixType.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        pixtype = new PixType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                    }
                }
            }

            if (pixtype == null)
            {
                return NotFound();
            }

            return Ok(pixtype);
        }

        [HttpPut("adonet/{id}")]
        public async Task<IActionResult> PutPixType(int id, PixType pixtype)
        {
            if (id != pixtype.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.PixType.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Id", pixtype.Id);
                cmd.Parameters.AddWithValue("@Name", pixtype.Name);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("adonet")]
        public async Task<ActionResult<PixType>> PostPixType(PixType pixtype)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.PixType.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@Name", pixtype.Name);
                var id = (int)await cmd.ExecuteScalarAsync();

                pixtype.Id = id;
                return CreatedAtAction("PostPixType", new { id = pixtype.Id }, pixtype);
            }
        }

        [HttpDelete("adonet/{id}")]
        public async Task<IActionResult> DeletePixType(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.PixType.DELETE, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> PixTypeExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.PixType.EXISTS, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
