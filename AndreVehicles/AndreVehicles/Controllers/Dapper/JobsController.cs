using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config DapperFile;

        JobsController()
        {
            using (var reader = new StreamReader(@".\Controllers\Dapper\Query.json"))
            {
                string json = reader.ReadToEnd();
                DapperFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = DapperFile.ConnectionString;
            }
        }

        [HttpGet("job/dapper")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJob()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var jobs = await connection.QueryAsync<Job>(DapperFile.Query.Job.GET);
                return Ok(jobs);
            }
        }

        [HttpGet("job/dapper/{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var job = await connection.QueryFirstOrDefaultAsync<Job>(DapperFile.Query.Job.GETBYID, new { Id = id });

                if (job == null)
                {
                    return NotFound();
                }

                return Ok(job);
            }
        }

        [HttpPut("job/dapper/{id}")]
        public async Task<IActionResult> PutJob(int id, Job job)
        {
            if (id.Equals(job.Id))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.Job.UPDATE, job);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("job/dapper/")]
        public async Task<ActionResult<Job>> PostJob(Job job)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(DapperFile.Query.Job.INSERT, job);

                job.Id = id;

                return CreatedAtAction("GetJob", new { Id = job.Id }, job);
            }
        }

        [HttpDelete("job/dapper/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.Job.DELETE, new { Id = id });

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
                return await connection.ExecuteScalarAsync<bool>(DapperFile.Query.Job.EXISTS, new { Id = id });
            }
        }
    }
}
