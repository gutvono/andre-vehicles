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
    public class CardsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;

        public CardsController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("card/dapper")]
        public async Task<ActionResult<IEnumerable<Card>>> GetCard()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var cards = await connection.QueryAsync<Card>(QueryFile.Query.Card.GET);
                return Ok(cards);
            }
        }

        [HttpGet("card/dapper/{CardNumber}")]
        public async Task<ActionResult<Card>> GetCard(string CardNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var card = await connection.QueryFirstOrDefaultAsync<Card>(QueryFile.Query.Card.GETBYID, new { CardNumber = CardNumber });

                if (card == null)
                {
                    return NotFound();
                }

                return Ok(card);
            }
        }

        [HttpPut("card/dapper/{CardNumber}")]
        public async Task<IActionResult> PutCard(string cardNumber, Card card)
        {
            if (cardNumber != card.CardNumber)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Card.UPDATE, card);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("card/dapper/")]
        public async Task<ActionResult<Card>> PostCard(Card card)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var CardNumber = await connection.QuerySingleAsync<string>(QueryFile.Query.Card.INSERT, card);

                card.CardNumber = CardNumber;

                return CreatedAtAction("GetAddress", new { CardNumber = card.CardNumber }, card);
            }
        }

        [HttpDelete("card/dapper/{CardNumber}")]
        public async Task<IActionResult> DeleteCard(int cardNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Card.DELETE, new { CardNumber = cardNumber });

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> CardExists(string cardNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<bool>(QueryFile.Query.Card.EXISTS, new { CardNumber = cardNumber });
            }
        }
    }
}
