using Library.Core.Stores;
using Library.Interfaces;
using Library.Interfaces.Controllers;
using Library.Interfaces.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Library.Core.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase, IUserController
{
    private readonly IUserStore UserStore;
    private readonly ICardStore CardStore;
    private readonly IPersonStore PersonStore;

    public UserController(ICardStore cardStore, IPersonStore personStore, IUserStore userStore)
    {
        this.CardStore = cardStore;
        this.PersonStore = personStore;
        this.UserStore = userStore;
    }

    [Tags("Delete")]
    [HttpDelete("DeleteByCardNumber/{cardNumber}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int cardNumber)
    {
        try
        {
            await UserStore.DeleteAsync(cardNumber);
            return NoContent();
        }
        catch (InvalidOperationException)
        {

            return NotFound();
        }
        catch (NullReferenceException)
        {
            return BadRequest();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Tags("Insert")]
    [HttpPut("Insert")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Insert(int cardNumber, string personIdCode)
    {
        try
        {
            await UserStore.InsertAsync(cardNumber, personIdCode);

            return CreatedAtAction(nameof(Insert), new { cardNumber = cardNumber, personIdCode = personIdCode });
        }
        catch (InvalidOperationException)
        {

            return BadRequest();
        }
        catch (KeyNotFoundException)
        {

            return BadRequest();
        }
    }
}
