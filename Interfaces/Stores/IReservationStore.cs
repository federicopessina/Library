using Library.Entities;

namespace Library.Interfaces.Stores;

public interface IReservationStore
{
    Task<bool> Contains(int cardNumber);
    Task<bool> Contains(string bookCode);
    Task<List<Reservation>> GetDelayedAsync(bool? isBlocked);
    Task<Dictionary<int, List<Reservation>>> GetAllAsync();
    Task InsertAsync(int cardNumber, Reservation reservation);
    Task UpdatePeriodAsync(int cardNumber, string bookCode, DateTime dateTo);
    Task UpdateStatusAsync(int cardNumber, string bookCode, Status status = Status.Returned);
}
