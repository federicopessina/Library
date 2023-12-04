using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Library.Core.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase, IPersonController
{
    private readonly IPersonStore PersonStore;

    public PersonController(IPersonStore personStore)
    {
        PersonStore = personStore;
    }

    [Tags("Get")]
    [HttpGet("GetAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var people = await PersonStore.GetAsync();
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

    [Tags("Get")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string idCode)
    {
        try
        {
            var people = await PersonStore.GetAsync(idCode);
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

    [Tags("Insert")]
    [HttpPut("Insert")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Insert([FromBody] Person person)
    {
        try
        {
            await PersonStore.InsertAsync(person);

            return CreatedAtAction(nameof(Insert), new { person.IdCode }, person);
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

    [Tags("Update")]
    [HttpPost]
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
