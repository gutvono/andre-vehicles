using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Model;
using System.Configuration;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config DapperFile;

        public AddressesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Dapper\Query.json"))
            {
                string json = reader.ReadToEnd();
                DapperFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = DapperFile.ConnectionString;
            }
        }

        [HttpGet("addresses/dapper")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddress()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var addresses = await connection.QueryAsync<Address>(DapperFile.Query.AddressesController.GET);
                return Ok(addresses);
            }
        }

        [HttpGet("addresses/dapper/{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var address = await connection.QueryFirstOrDefaultAsync<Address>(DapperFile.Query.AddressesController.GETBYID, new { Id = id });

                if (address == null)
                {
                    return NotFound();
                }

                return Ok(address);
            }
        }

        [HttpPut("addresses/dapper/{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.AddressesController.UPDATE, address);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("addresses/dapper/")]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(DapperFile.Query.AddressesController.INSERT, address);

                address.Id = id;

                return CreatedAtAction("GetAddress", new { id = address.Id }, address);
            }
        }

        [HttpDelete("addresses/dapper/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.AddressesController.DELETE, new { Id = id });

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> AddressExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<bool>(DapperFile.Query.AddressesController.EXISTS, new { Id = id });
            }
        }
    }
}
