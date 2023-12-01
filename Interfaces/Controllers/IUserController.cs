using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces.Controllers;

public interface IUserController
{
    Task<IActionResult> Delete(int cardNumber);
    Task<IActionResult> Insert(int cardNumber, string personIdCode);
}
