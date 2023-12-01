using Library.Entities;
using Library.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces;

public interface IBookController
{
    Task<IActionResult> DeleteAll();
    Task<IActionResult> DeleteByCode(string code);
    Task<IEnumerable<Book>> Get();
    Task<IActionResult> GetAll();
    Task<IActionResult> GetByAuthor([FromQuery] string? author);
    Task<IActionResult> GetByDefinition([FromBody] BookSearch book);
    Task<IActionResult> GetByGenre(EGenre? genre);
    Task<IActionResult> GetByPosition([FromQuery] int? position);
    Task<IActionResult> GetByCode(string code);
    Task<IActionResult> GetByTitle([FromQuery] string? title);
    Task<IActionResult> Insert([FromBody] Book book);
    Task<IActionResult> Update([FromBody] Book book);
}
