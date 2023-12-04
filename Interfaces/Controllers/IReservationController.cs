using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces.Controllers;

public interface IReservationController
{
    Task<IActionResult> GetAll();
    Task<IActionResult> GetDelayed(bool? isBlocked);
    Task<IActionResult> Insert(Tuple<int, Reservation> tuple);
    Task<IActionResult> UpdatePeriod(Tuple<int, Reservation, DateTime> tuple);
    //Task<IActionResult> UpdateStatus(Tuple<int, Reservation, Status> tuple);
}
