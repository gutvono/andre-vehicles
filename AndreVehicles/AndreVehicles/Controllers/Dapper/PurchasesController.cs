﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config DapperFile;

        PurchasesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Dapper\Query.json"))
            {
                string json = reader.ReadToEnd();
                DapperFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = DapperFile.ConnectionString;
            }
        }

        [HttpGet("purchase/dapper")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchase()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var purchases = await connection.QueryAsync<Purchase>(DapperFile.Query.Purchase.GET);
                return Ok(purchases);
            }
        }

        [HttpGet("purchase/dapper/{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var purchase = await connection.QueryFirstOrDefaultAsync<Purchase>(DapperFile.Query.Purchase.GETBYID, new { Id = id });

                if (purchase == null)
                {
                    return NotFound();
                }

                return Ok(purchase);
            }
        }

        [HttpPut("purchase/dapper/{id}")]
        public async Task<IActionResult> PutPurchase(int id, Purchase purchase)
        {
            if (id.Equals(purchase.Id))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.Purchase.UPDATE, purchase);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("purchase/dapper/")]
        public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(DapperFile.Query.Purchase.INSERT, purchase);

                purchase.Id = id;

                return CreatedAtAction("GetPurchase", new { Id = purchase.Id }, purchase);
            }
        }

        [HttpDelete("purchase/dapper/{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.Purchase.DELETE, new { Id = id });

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> PurchaseExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<bool>(DapperFile.Query.Purchase.EXISTS, new { Id = id });
            }
        }
    }
}