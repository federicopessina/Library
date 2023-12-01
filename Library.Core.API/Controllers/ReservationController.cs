using Library.Core.Stores;
using Library.Entities;
using Library.Interfaces.Controllers;
using Library.Interfaces.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Library.Core.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationController : ControllerBase, IReservationController
{
    public static ReservationStore ReservationStore { get; set; }
    private readonly ICardStore _cardStore;
    private readonly IUserStore _userStore;

    public ReservationController(ICardStore cardStore, IUserStore userStore)
    {
        this._cardStore = cardStore;
        this._userStore = userStore;

        if (ReservationStore is null)
            ReservationStore = new ReservationStore(_cardStore, _userStore);
    }

    [Tags("Get")]
    [HttpGet("GetDelayed")]
    [ProducesResponseType(typeof(Dictionary<int, Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDelayed(bool? isBlocked = null)
    {
        try
        {
            var reservations = await ReservationStore.GetDelayedAsync(isBlocked);
            return reservations is null ? NotFound() : Ok(reservations);
        }
        catch (KeyNotFoundException)
        {

            return NotFound();
        }
        catch (NullReferenceException)
        {
            return BadRequest();
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
    public async Task<IActionResult> Insert(Tuple<int, Reservation> tuple)
    {
        try
        {
            // TODO Rename variables.
            await ReservationStore.InsertAsync(tuple.Item1, tuple.Item2);

            return CreatedAtAction(nameof(Insert), new { tuple.Item1 }, tuple.Item2);
        }
        catch (InvalidOperationException)
        {

            return BadRequest();
        }
        catch (KeyNotFoundException)
        {

            return BadRequest();
        }
        catch (NullReferenceException)
        {

            return BadRequest();
        }
    }

    [Tags("Update")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePeriod(Tuple<int, Reservation, DateTime> tuple)
    {
        try
        {
            // TODO Change names.
            await ReservationStore.UpdatePeriodAsync(tuple.Item1, tuple.Item2, tuple.Item3);

            return CreatedAtAction(nameof(UpdatePeriod), new { cardNumber = tuple.Item1 }, tuple);
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

            return BadRequest();
        }
    }

    //[Tags("Update")]
    //[HttpPost]
    //[ProducesResponseType(StatusCodes.Status204NoContent)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> UpdateStatus(Tuple<int, Reservation, Status> tuple)
    //{
    //    try
    //    {
    //        // TODO Change names.
    //        await ReservationStore.UpdateStatusAsync(tuple.Item1, tuple.Item2, tuple.Item3);

    //        return CreatedAtAction(nameof(UpdateStatus), new { cardNumber = tuple.Item1 }, tuple);
    //    }
    //    catch (KeyNotFoundException)
    //    {

    //        return NotFound();
    //    }
    //    catch (InvalidOperationException)
    //    {

    //        return BadRequest();
    //    }
    //    catch (NullReferenceException)
    //    {

    //        return BadRequest();
    //    }
    //}
}
