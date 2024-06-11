using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Model;
using System.Net;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        CardsController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("adonet")]
        public async Task<ActionResult<IEnumerable<Card>>> GetCardes()
        {
            var cards = new List<Card>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Card.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cards.Add(new Card
                        {
                            CardNumber = reader.GetString(0),
                            SecurityCode = reader.GetString(1),
                            ExpirationDate = reader.GetString(2),
                            CardName = reader.GetString(3)
                        });
                    }
                }
            }
            return Ok(cards);
        }

        [HttpGet("adonet/{CardNumber}")]
        public async Task<ActionResult<Card>> GetCard(string cardNumber)
        {
            Card card = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Card.GETBYID, connection);
                cmd.Parameters.AddWithValue("@CardNumber", cardNumber);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        card = new Card
                        {
                            CardNumber = reader.GetString(0),
                            SecurityCode = reader.GetString(1),
                            ExpirationDate = reader.GetString(2),
                            CardName = reader.GetString(3)
                        };
                    }
                }
            }

            if (card == null)
            {
                return NotFound();
            }

            return Ok(card);
        }

        [HttpPut("adonet/{CardNumber}")]
        public async Task<IActionResult> PutCard(string cardNumber, Card card)
        {
            if (cardNumber != card.CardNumber)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Card.UPDATE, connection);
                cmd.Parameters.AddWithValue("@CardNumber", card.CardNumber);
                cmd.Parameters.AddWithValue("@SecurityCode", card.SecurityCode);
                cmd.Parameters.AddWithValue("@ExpirationDate", card.ExpirationDate);
                cmd.Parameters.AddWithValue("@CardName", card.CardName);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("adonet")]
        public async Task<ActionResult<Card>> PostCard(Card card)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Card.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@CardNumber", card.CardNumber);
                cmd.Parameters.AddWithValue("@SecurityCode", card.SecurityCode);
                cmd.Parameters.AddWithValue("@ExpirationDate", card.ExpirationDate);
                cmd.Parameters.AddWithValue("@CardName", card.CardName);
                var cardNumber = (string)await cmd.ExecuteScalarAsync();

                card.CardNumber = cardNumber;
                return CreatedAtAction("PostCard", new { id = card.CardNumber }, card);
            }
        }

        [HttpDelete("adonet/{id}")]
        public async Task<IActionResult> DeleteCard(string cardNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Card.DELETE, connection);
                cmd.Parameters.AddWithValue("@CardNumber", cardNumber);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

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
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Card.EXISTS, connection);
                cmd.Parameters.AddWithValue("@CardNumber", cardNumber);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}