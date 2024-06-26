﻿using System;
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
using AndreVehicles.Services;
using Model.DTO;

namespace AndreVehicles.Controllers.Dapper
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        private readonly AddressService _addressService;

        public AddressesController(AddressService addressService)
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                _addressService = addressService;

                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("dapper")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddress()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var addresses = await connection.QueryAsync<Address>(QueryFile.Query.Address.GET);
                return Ok(addresses);
            }
        }

        [HttpGet("dapper/{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var address = await connection.QueryFirstOrDefaultAsync<Address>(QueryFile.Query.Address.GETBYID, new { Id = id });

                if (address == null)
                {
                    return NotFound();
                }

                return Ok(address);
            }
        }

        [HttpPut("dapper/{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Address.UPDATE, address);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("dapper/")]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(QueryFile.Query.Address.INSERT, address);

                address.Id = id;

                return CreatedAtAction("GetAddress", new { id = address.Id }, address);
            }
        }

        [HttpPost("dapper/{cep:length(8)}")]
        public ActionResult<AddressViacepDTO> GetViacepAddress(string cep) => _addressService.GetViacepAddress(cep).Result;

        [HttpDelete("dapper/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Address.DELETE, new { Id = id });

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
                return await connection.ExecuteScalarAsync<bool>(QueryFile.Query.Address.EXISTS, new { Id = id });
            }
        }
    }
}
