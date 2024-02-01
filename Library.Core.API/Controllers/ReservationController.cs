using Library.Core.Exceptions.BookStore;
using Library.Core.Exceptions.ReservationStore;
using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Controllers;
using Library.Interfaces.Stores;
using Microsoft.AspNetCore.Mvc;
using StoreIsEmptyException = Library.Core.Exceptions.BookStore.StoreIsEmptyException;

namespace Library.Core.API.Controllers;
/// <summary>
/// ReservationController.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase, IReservationController
{
    private const string ReservationTag = "Reservation";

    private readonly IBookStore BookStore;
    private readonly ICardStore CardStore;
    private readonly IUserStore UserStore;
    private readonly IReservationStore ReservationStore;
    /// <summary>
    /// Contructor.
    /// </summary>
    /// <param name="bookStore"></param>
    /// <param name="cardStore"></param>
    /// <param name="userStore"></param>
    /// <param name="reservationStore"></param>
    public ReservationController(IBookStore bookStore, ICardStore cardStore, IUserStore userStore, IReservationStore reservationStore)
    {
        this.BookStore = bookStore;
        this.CardStore = cardStore;
        this.UserStore = userStore;
        this.ReservationStore = reservationStore;
    }
    /// <summary>
    /// GetAll.
    /// </summary>
    /// <returns></returns>
    [Tags(ReservationTag)]
    [HttpGet($"{nameof(GetAll)}")]
    [ProducesResponseType(typeof(Dictionary<int, Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var reservations = await ReservationStore.GetAllAsync();
            return reservations is null ? NotFound() : Ok(reservations);
        }
        catch (StoreIsEmptyException)
        {

            return NoContent();
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Get all delayed reservations.
    /// </summary>
    /// <param name="isBlocked"></param>
    /// <returns></returns>
    [Tags(ReservationTag)]
    [HttpGet($"{nameof(GetDelayed)}")]
    [ProducesResponseType(typeof(Dictionary<int, Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDelayed(bool? isBlocked = null)
    {
        try
        {
            var reservations = await ReservationStore.GetDelayedAsync(isBlocked);
            return reservations is null ? NotFound() : Ok(reservations);
        }
        catch (StoreIsEmptyException)
        {

            return NoContent();
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Insert.
    /// </summary>
    /// <param name="tuple"></param>
    /// <returns></returns>
    [Tags(ReservationTag)]
    [HttpPut($"{nameof(Insert)}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Insert(Tuple<int, Reservation> tuple)
    {
        try
        {
            // TODO Rename variables.
            await ReservationStore.InsertAsync(tuple.Item1, tuple.Item2);

            return CreatedAtAction(nameof(Insert), new { cardNumber = tuple.Item1, reservation = tuple.Item2 });
        }
        catch (BookCodeNotFoundException)
        {

            return NotFound();
        }
        catch (BookNotReservableException)
        {

            return BadRequest();
        }
        catch (CardBlockedException)
        {

            return BadRequest();
        }
        catch (BookAlreadyReservedException)
        {

            return Conflict();
        }
        catch (CardNotInUserStoreException)
        {

            return NotFound();
        }
        catch (NumberOfReservationsExceededException)
        {

            return BadRequest();
        }
        catch (InvalidOperationException)
        {

            return BadRequest();
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Update period of reservation.
    /// </summary>
    /// <param name="tuple"></param>
    /// <returns></returns>
    [Tags(ReservationTag)]
    [HttpPost($"{nameof(UpdatePeriod)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePeriod(Tuple<int, string, DateTime> tuple)
    {
        try
        {
            // TODO Change names.
            await ReservationStore.UpdatePeriodAsync(tuple.Item1, tuple.Item2, tuple.Item3);
            return CreatedAtAction(nameof(UpdatePeriod), new { cardNumber = tuple.Item1 }, tuple);
        }
        catch (StoreIsEmptyException)
        {

            return NoContent();
        }
        catch (CardNotInUserStoreException)
        {

            return NotFound();
        }
        catch (InvalidOperationException)
        {

            return BadRequest();
        }
        catch (ReservationNotFoundException)
        {

            return NotFound();
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
}
