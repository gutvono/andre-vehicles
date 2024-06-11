using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class PixesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        PixesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("adonet")]
        public async Task<ActionResult<IEnumerable<Pix>>> GetPix()
        {
            var pixes = new List<Pix>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Pix.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        pixes.Add(new Pix
                        {
                            Id = reader.GetInt32(0),
                            PixType = new PixType { Id = reader.GetInt32(1) },
                            PixKey = reader.GetString(2)
                        });
                    }
                }
            }
            return Ok(pixes);
        }

        [HttpGet("adonet/{id}")]
        public async Task<ActionResult<Pix>> GetPix(int id)
        {
            Pix pix = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Pix.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        pix = new Pix
                        {
                            Id = reader.GetInt32(0),
                            PixType = new PixType { Id = reader.GetInt32(1) },
                            PixKey = reader.GetString(2)
                        };
                    }
                }
            }

            if (pix == null)
            {
                return NotFound();
            }

            return Ok(pix);
        }

        [HttpPut("adonet/{id}")]
        public async Task<IActionResult> PutPix(int id, Pix pix)
        {
            if (id != pix.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Pix.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Id", pix.Id);
                cmd.Parameters.AddWithValue("@PixType", pix.PixType);
                cmd.Parameters.AddWithValue("@PixKey", pix.PixKey);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("adonet")]
        public async Task<ActionResult<Pix>> PostPix(Pix pix)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Pix.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@PixType", pix.PixType);
                cmd.Parameters.AddWithValue("@PixKey", pix.PixKey);
                var id = (int)await cmd.ExecuteScalarAsync();

                pix.Id = id;
                return CreatedAtAction("PostPix", new { id = pix.Id }, pix);
            }
        }

        [HttpDelete("adonet/{id}")]
        public async Task<IActionResult> DeletePix(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Pix.DELETE, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> PixExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Pix.EXISTS, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
