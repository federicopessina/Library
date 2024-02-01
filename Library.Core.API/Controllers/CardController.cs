using Library.Core.Exceptions.CardStore;
using Library.Core.Exceptions.Results;
using Library.Core.Stores;
using Library.Entities;
using Library.Interfaces.Controllers;
using Library.Interfaces.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Library.Core.API.Controllers;

/// <summary>
/// 
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CardController : ControllerBase, ICardController
{
    private const string CardTag = "Card";

    private readonly ICardStore CardStore;
    private readonly IReservationStore ReservationStore;
    private readonly IUserStore UserStore;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cardStore"></param>
    /// <param name="reservationStore"></param>
    /// <param name="userStore"></param>
    public CardController(ICardStore cardStore, IReservationStore reservationStore, IUserStore userStore)
    {
        this.CardStore = cardStore;
        this.ReservationStore = reservationStore;
        this.UserStore = userStore;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cardNumber"></param>
    /// <returns></returns>
    [Tags(CardTag)]
    [HttpDelete($"{nameof(Delete)}/{{cardNumber}}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int cardNumber)
    {
        try
        {
            await CardStore.DeleteAsync(cardNumber, ReservationStore, UserStore);
            return NoContent();
        }
        catch (StoreIsEmptyException)
        {

            return NoContent();
        }
        catch (CardNumberNotFoundException)
        {

            return NotFound();
        }
        catch (ReservationOpenException)
        {

            return BadRequest();
        }
        catch (UserRegisteredException)
        {

            return BadRequest();
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Get parameterless.
    /// </summary>
    /// <returns></returns>
    [Tags(CardTag)]
    [HttpGet($"{nameof(Get)}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var cards = await CardStore.GetStore();
            return cards is null ? NotFound() : Ok(cards);
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Get by card number.
    /// </summary>
    /// <param name="cardNumber"></param>
    /// <returns></returns>
    [Tags(CardTag)]
    [HttpGet($"{nameof(Get)}/{{cardNumber}}")]
    [ProducesResponseType(typeof(Card), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int cardNumber)
    {
        try
        {
            var card = await CardStore.GetAsync(cardNumber);
            return card is null ? NotFound() : Ok(card);
        }
        catch (StoreIsEmptyException)
        {

            return NoContent();
        }
        catch (CardNumberNotFoundException)
        {

            return NotFound();
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isBlocked"></param>
    /// <returns></returns>
    [Tags(CardTag)]
    [HttpGet($"{nameof(GetIsBlocked)}/{{isBlocked}}")]
    [ProducesResponseType(typeof(List<Card>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIsBlocked(bool isBlocked)
    {
        try
        {
            var card = await CardStore.GetIsBlockedAsync(isBlocked);
            return card is null ? NotFound() : Ok(card);
        }
        catch (StoreIsEmptyException)
        {

            return NoContent();
        }
        catch (EmptyResultException)
        {

            return NotFound();
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Insert card in card store.
    /// </summary>
    /// <param name="card"><see cref="Card"/> object.</param>
    /// <returns></returns>
    [Tags(CardTag)]
    [HttpPut($"{nameof(Insert)}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Insert([FromBody] Card card)
    {
        try
        {
            await CardStore.InsertAsync(card);
            return CreatedAtAction(nameof(Insert), new { card }, card);
        }
        catch (DuplicatedCardNumberException)
        {

            return BadRequest();
        }
        catch (Exception)
        {

            throw;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cardNumber"></param>
    /// <param name="isBlocked"></param>
    /// <returns></returns>
    [Tags(CardTag)]
    [HttpPost($"{nameof(UpdateIsBlocked)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIsBlocked(int cardNumber, bool? isBlocked)
    {
        try
        {
            await CardStore.UpdateIsBlockedAsync(cardNumber);

            return CreatedAtAction(nameof(UpdateIsBlocked), new { cardNumber }, isBlocked);
        }
        catch (StoreIsEmptyException)
        {

            return NoContent();
        }
        catch (CardNumberNotFoundException)
        {

            return NotFound();
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
}
