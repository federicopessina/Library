using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Library.Core.API.Controllers;
/// <summary>
/// PersonController.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase, IPersonController
{
    private const string PersonTag = "Person";
    private readonly IPersonStore PersonStore;
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="personStore"></param>
    public PersonController(IPersonStore personStore)
    {
        PersonStore = personStore;
    }
    /// <summary>
    /// GetAll.
    /// </summary>
    /// <returns></returns>
    [Tags(PersonTag)]
    [HttpGet($"{nameof(GetAll)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var people = await PersonStore.GetStoreAsync();
            return people is null ? NotFound() : Ok(people);
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
    /// <summary>
    /// Get.
    /// </summary>
    /// <param name="idCode"></param>
    /// <returns></returns>
    [Tags(PersonTag)]
    [HttpGet($"{nameof(Get)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string idCode)
    {
        try
        {
            var people = await PersonStore.GetById(idCode);
            return people is null ? NotFound() : Ok(people);
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
    /// <summary>
    /// Insert.
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    [Tags(PersonTag)]
    [HttpPut($"{nameof(Insert)}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Insert([FromBody] Person person)
    {
        try
        {
            await PersonStore.InsertAsync(person);

            return CreatedAtAction(nameof(Insert), new { person.Id }, person);
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
    /// Update Person Controller.
    /// </summary>
    /// <param name="tuple"></param>
    /// <returns></returns>
    [Tags(PersonTag)]
    [HttpPost($"{nameof(Update)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] Tuple<string, Address> tuple)
    {
        try
        {
            // TODO Rename variables.
            await PersonStore.UpdateAddressAsync(tuple.Item1, tuple.Item2);

            return CreatedAtAction(nameof(Update), new { idCode = tuple.Item1 }, new { addess = tuple.Item2 });
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
}
