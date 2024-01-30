using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces.Controllers;

public interface IPublicationController
{
    Task<IActionResult> Delete(string isbn);
    Task<IActionResult> Get(string isbn);
    Task<IActionResult> GetAll();
    Task<IActionResult> GetByAuthor(string author);
    Task<IActionResult> Insert(Publication publication);
}
