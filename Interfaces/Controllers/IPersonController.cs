using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces.Controllers;

public interface IPersonController
{
    Task<IActionResult> GetAll();
    Task<IActionResult> Get(string idCode);
    Task<IActionResult> Insert(Person person);
    Task<IActionResult> Update([FromBody] Tuple<string, Address> tuple);
}
