using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        JobsController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("adonet")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJob()
        {
            var jobs = new List<Job>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Job.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        jobs.Add(new Job
                        {
                            Id = reader.GetInt32(0),
                            Description = reader.GetString(1)
                        });
                    }
                }
            }
            return Ok(jobs);
        }

        [HttpGet("adonet/{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            Job job = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Job.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        job = new Job
                        {
                            Id = reader.GetInt32(0),
                            Description = reader.GetString(1)
                        };
                    }
                }
            }

            if (job == null)
            {
                return NotFound();
            }

            return Ok(job);
        }

        [HttpPut("adonet/{id}")]
        public async Task<IActionResult> PutJob(int id, Job job)
        {
            if (id != job.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Job.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Id", job.Id);
                cmd.Parameters.AddWithValue("@Description", job.Description);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("adonet")]
        public async Task<ActionResult<Job>> PostJob(Job job)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Job.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@Description", job.Description);
                var id = (int)await cmd.ExecuteScalarAsync();

                job.Id = id;
                return CreatedAtAction("PostJob", new { id = job.Id }, job);
            }
        }

        [HttpDelete("adonet/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Job.DELETE, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> JobExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Job.EXISTS, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
