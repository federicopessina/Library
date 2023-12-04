namespace Library.Entities;

public class Reservation : IReservation
{
    public string BookCode { get; set; }
    public Period Period { get; set; }
    public Status Status { get; set; } = Status.Reserved;
    public Reservation()
    {

    }
    public Reservation(string bookCode, Period period)
    {
        BookCode = bookCode;
        Period = period;
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
