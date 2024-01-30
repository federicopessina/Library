using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces;

public interface IBookController
{
    Task<IActionResult> DeleteAll();
    Task<IActionResult> DeleteByCode(string code);
    Task<IActionResult> GetAll();
    Task<IActionResult> GetByPosition([FromQuery] int? position);
    Task<IActionResult> GetByCode(string code);
    Task<IActionResult> Insert([FromBody] Book book);
    Task<IActionResult> Update([FromBody] Book book);
}
