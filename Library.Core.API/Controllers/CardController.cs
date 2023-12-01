using Library.Core.Stores;
using Library.Entities;
using Library.Interfaces.Controllers;
using Library.Interfaces.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Library.Core.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CardController : ControllerBase, ICardController
{
    public static CardStore CardStore { get; set; }
    private readonly IReservationStore _reservationStore;
    private readonly IUserStore _userStore;

    public CardController(IReservationStore reservationStore, IUserStore userStore)
    {
        this._reservationStore = reservationStore;
        this._userStore = userStore;

        if (CardStore is null)
            CardStore = new CardStore();
    }

    [Tags("Delete")]
    [HttpDelete("Delete/{cardNumber}")]
    public async Task<IActionResult> Delete(Card card)
    {
        try
        {
            await CardStore.DeleteAsync(card, _reservationStore, _userStore);
            return NoContent();
        }
        catch (InvalidOperationException)
        {

            return NotFound();
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Tags("Get")]
    [HttpGet("Get/{cardNumber}")]
    [ProducesResponseType(typeof(Card), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int cardNumber)
    {
        try
        {
            var card = await CardStore.GetAsync(cardNumber);
            return card is null ? NotFound() : Ok(card);
        }
        catch (KeyNotFoundException)
        {

            return NotFound();
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }
    }

    [Tags("Get")]
    [HttpGet("GetIsBlocked/{isBlocked}")]
    [ProducesResponseType(typeof(List<Card>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIsBlocked(bool isBlocked)
    {
        try
        {
            var card = await CardStore.GetIsBlockedAsync(isBlocked);
            return card is null ? NotFound() : Ok(card);
        }
        catch (KeyNotFoundException)
        {

            return NotFound();
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }
    }

    [Tags("Insert")]
    [HttpPut("Insert")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Insert([FromBody] Card card)
    {
        try
        {
            await CardStore.InsertAsync(card);

            return CreatedAtAction(nameof(Insert), new { number = card.Number }, card);
        }
        catch (ArgumentException)
        {

            return BadRequest();
        }
        catch (NullReferenceException)
        {

            return NotFound();
        }
    }

    [Tags("Update")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIsBlocked(int cardNumber, bool? isBlocked)
    {
        try
        {
            await CardStore.UpdateIsBlockedAsync(cardNumber, isBlocked ?? true); // TODO Check ?? operator.

            return CreatedAtAction(nameof(UpdateIsBlocked), new { cardNumber }, isBlocked);
        }
        catch (KeyNotFoundException)
        {

            return NotFound();
        }
        catch (InvalidOperationException)
        {

            return BadRequest();
        }
        catch (NullReferenceException)
        {

            return NotFound();
        }
    }
}
