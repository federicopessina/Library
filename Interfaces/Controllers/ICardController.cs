using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces.Controllers;

public interface ICardController
{
    Task<IActionResult> Delete(int number);
    Task<IActionResult> Get();
    Task<IActionResult> Get(int number);
    Task<IActionResult> GetIsBlocked(bool isBlocked);
    Task<IActionResult> Insert(Card card);
    Task<IActionResult> UpdateIsBlocked(int cardNumber, bool? isBlocked);

}
