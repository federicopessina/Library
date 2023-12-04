using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces.Controllers;

public interface ICardController
{
    Task<IActionResult> Delete(Card cardNumber); // TODO Change to int
    Task<IActionResult> Get();
    Task<IActionResult> Get(int cardNumber);
    Task<IActionResult> GetIsBlocked(bool isBlocked);
    Task<IActionResult> Insert(Card card);
    Task<IActionResult> UpdateIsBlocked(int cardNumber, bool? isBlocked);

}
