﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
{
    [Route("api/[controller]")]
    [ApiController]
    public class PixTypesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config DapperFile;

        PixTypesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Dapper\Query.json"))
            {
                string json = reader.ReadToEnd();
                DapperFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = DapperFile.ConnectionString;
            }
        }

        [HttpGet("pixtype/dapper")]
        public async Task<ActionResult<IEnumerable<PixType>>> GetPixType()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var pixtypes = await connection.QueryAsync<PixType>(DapperFile.Query.PixType.GET);
                return Ok(pixtypes);
            }
        }

        [HttpGet("pixtype/dapper/{id}")]
        public async Task<ActionResult<PixType>> GetPixType(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var pixtype = await connection.QueryFirstOrDefaultAsync<PixType>(DapperFile.Query.PixType.GETBYID, new { Id = id });

                if (pixtype == null)
                {
                    return NotFound();
                }

                return Ok(pixtype);
            }
        }

        [HttpPut("pixtype/dapper/{id}")]
        public async Task<IActionResult> PutPixType(int id, PixType pixtype)
        {
            if (id.Equals(pixtype.Id))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.PixType.UPDATE, pixtype);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("pixtype/dapper/")]
        public async Task<ActionResult<PixType>> PostPixType(PixType pixtype)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(DapperFile.Query.PixType.INSERT, pixtype);

                pixtype.Id = id;

                return CreatedAtAction("GetPixType", new { Id = pixtype.Id }, pixtype);
            }
        }

        [HttpDelete("pixtype/dapper/{id}")]
        public async Task<IActionResult> DeletePixType(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.PixType.DELETE, new { Id = id });

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
                return await connection.ExecuteScalarAsync<bool>(DapperFile.Query.PixType.EXISTS, new { Id = id });
            }
        }
    }
}
