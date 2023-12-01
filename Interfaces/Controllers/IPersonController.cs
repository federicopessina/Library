using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces.Controllers;

public interface IPersonController
{
    Task<IActionResult> Insert(Person person);
    Task<IActionResult> Update([FromBody] Tuple<string, Address> tuple);
}
