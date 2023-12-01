using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces.Controllers;

public interface IReservationController
{
    Task<IActionResult> GetDelayed(bool? isBlocked = null);
    Task<IActionResult> Insert(Tuple<int, Reservation> tuple);
    Task<IActionResult> UpdatePeriod(Tuple<int, Reservation, DateTime> tuple);
    //Task<IActionResult> UpdateStatus(Tuple<int, Reservation, Status> tuple);
}
