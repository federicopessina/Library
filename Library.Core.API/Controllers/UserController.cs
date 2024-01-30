using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Controllers;
using Library.Interfaces.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Library.Core.API.Controllers;
/// <summary>
/// UserController.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase, IUserController
{
    private const string UserTag = "User";
    private readonly IUserStore UserStore;
    private readonly ICardStore CardStore;
    private readonly IPersonStore PersonStore;
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="cardStore"></param>
    /// <param name="personStore"></param>
    /// <param name="userStore"></param>
    public UserController(ICardStore cardStore, IPersonStore personStore, IUserStore userStore)
    {
        this.CardStore = cardStore;
        this.PersonStore = personStore;
        this.UserStore = userStore;
    }
    /// <summary>
    /// Delete user but not person.
    /// </summary>
    /// <param name="cardNumber"></param>
    /// <returns></returns>
    [Tags(UserTag)]
    [HttpDelete($"{nameof(Delete)}/{{cardNumber}}")]
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
    /// <summary>
    /// Insert new user.
    /// </summary>
    /// <param name="cardNumber"></param>
    /// <param name="personIdCode"></param>
    /// <returns></returns>
    [Tags(UserTag)]
    [HttpPut($"{nameof(Insert)}")]
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
    /// <summary>
    /// Register new user one-shot.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    [Tags(UserTag)]
    [HttpPut($"{nameof(Register)}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(User user)
    {
        try
        {
            await CardStore.InsertAsync(user.Card);
            await PersonStore.InsertAsync(user.Person);
            await UserStore.InsertAsync(user.Card.Number, user.Person.Id);
            return CreatedAtAction(nameof(Register), new { cardNumber = user.Card.Number, personIdCode = user.Person.Id });
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
}