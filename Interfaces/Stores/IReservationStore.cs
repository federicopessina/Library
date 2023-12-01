using Library.Entities;

namespace Library.Interfaces.Stores;

public interface IReservationStore
{
    Dictionary<int, List<Reservation>> Store { get; set; }
    Task<List<Reservation>> GetDelayedAsync(bool? isBlocked);
    Task InsertAsync(int cardNumber, Reservation reservation);
    Task UpdatePeriodAsync(int cardNumber, Reservation reservation, DateTime dateTo);
    Task UpdateStatusAsync(int cardNumber, Reservation reservation, Status status = Status.Returned);
}
