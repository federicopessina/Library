using Library.Entities;

namespace Library.Interfaces.Stores;

public interface IReservationStore
{
    Task<bool> Contains(int cardNumber);
    Task<List<Reservation>> GetDelayedAsync(bool? isBlocked);
    Task<Dictionary<int, List<Reservation>>> GetAllAsync();
    Task InsertAsync(int cardNumber, Reservation reservation);
    Task UpdatePeriodAsync(int cardNumber, Reservation reservation, DateTime dateTo);
    Task UpdateStatusAsync(int cardNumber, Reservation reservation, Status status = Status.Returned);
}
