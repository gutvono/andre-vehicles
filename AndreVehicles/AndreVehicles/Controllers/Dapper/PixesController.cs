using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
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

        [HttpGet("pix/dapper")]
        public async Task<ActionResult<IEnumerable<Pix>>> GetPix()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var pixes = await connection.QueryAsync<Pix>(QueryFile.Query.Pix.GET);
                return Ok(pixes);
            }
        }

        [HttpGet("pix/dapper/{id}")]
        public async Task<ActionResult<Pix>> GetPix(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var pix = await connection.QueryFirstOrDefaultAsync<Pix>(QueryFile.Query.Pix.GETBYID, new { Id = id });

                if (pix == null)
                {
                    return NotFound();
                }

                return Ok(pix);
            }
        }

        [HttpPut("pix/dapper/{id}")]
        public async Task<IActionResult> PutPix(int id, Pix pix)
        {
            if (id.Equals(pix.Id))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Pix.UPDATE, pix);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("pix/dapper/")]
        public async Task<ActionResult<Pix>> PostPix(Pix pix)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(QueryFile.Query.Pix.INSERT, pix);

                pix.Id = id;

                return CreatedAtAction("GetPix", new { Id = pix.Id }, pix);
            }
        }

        [HttpDelete("pix/dapper/{id}")]
        public async Task<IActionResult> DeletePix(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Pix.DELETE, new { Id = id });

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
                return await connection.ExecuteScalarAsync<bool>(QueryFile.Query.Pix.EXISTS, new { Id = id });
            }
        }
    }
}
