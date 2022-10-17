using Cards.API.Data;
using Cards.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cards.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : Controller
    {
        private readonly CardsDbContext cardsDbContext;

        public CardsController(CardsDbContext cardsDbContext)
        {
            this.cardsDbContext = cardsDbContext;
        }
        //get all cards
        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
            var cards = await cardsDbContext.Cards.ToListAsync();

            if (cards != null)
            {
                return Ok(cards);
            }

            return BadRequest();
        }

        //get a single card
        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetCard")]
        public async Task<IActionResult> GetCard([FromRoute] Guid id)
        {
            if(id != null)
            {
                var card = await cardsDbContext.Cards.FirstOrDefaultAsync(x => x.Id == id);

                if (card != null)
                {
                    return Ok(card);
                }

                return NotFound("Card not found");
            }
            return BadRequest("Id is null");
        }

        //Add card
        [HttpPost]
        public async Task<IActionResult> AddCard([FromBody] Card card)
        {
            if (card != null)
            {
                card.Id = Guid.NewGuid();

                var result = await cardsDbContext.Cards.AddAsync(card); 
                var save = await cardsDbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCard), new { id = card.Id }, card);

            }
            return BadRequest();
        }

        //updating cards
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateCard([FromRoute] Guid id, [FromBody] Card card)
        {
            var existingCard = await cardsDbContext.Cards.FirstOrDefaultAsync(x => x.Id == id);
            if (existingCard != null)
            {
                existingCard.CardholderName = card.CardholderName;
                existingCard.CardNumber = card.CardNumber;
                existingCard.ExpiryMonth = card.ExpiryMonth;
                existingCard.ExpiryYear = card.ExpiryYear;
                existingCard.CVC = card.CVC;

                var save = await cardsDbContext.SaveChangesAsync();
                return Ok(existingCard);            
            }
            return NotFound("Card not found");
        }

        //updating cards
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteCard([FromRoute] Guid id)
        {
            var existingCard = await cardsDbContext.Cards.FirstOrDefaultAsync(x => x.Id == id);
            if (existingCard != null)
            {
                cardsDbContext.Cards.Remove(existingCard);
                var save = await cardsDbContext.SaveChangesAsync();
                return Ok(existingCard);
            }
            return NotFound("Card not found");
        }



    }
}
