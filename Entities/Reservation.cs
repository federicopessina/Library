namespace Library.Entities;

public class Reservation : IReservation
{
    public Book Book { get; }
    public Period Period { get; }
    public Status Status { get; set; } = Status.Reserved;
    public Reservation()
    {

    }
    public Reservation(Period period, Book book)
    {
        Period = period;
        Book = book;
    }
}

public enum Status
{
    Reserved = 0,
    Picked = 1,
    Returned = 2
}

public class Period // TODO Change public ?
{
    private const int maxNoOfDays = 5;

    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public Period()
    {
        DateFrom = DateTime.Now.Date;
        DateTo = DateTime.Now.Date.AddDays(5);
    }
    public Period(DateTime dateFrom)
    {
        DateFrom = dateFrom;
        DateTo = dateFrom.AddDays(maxNoOfDays);
    }
    public Period(DateTime dateFrom, DateTime dateTo)
    {
        DateFrom = dateFrom;
        DateTo = dateTo;
    }
}
