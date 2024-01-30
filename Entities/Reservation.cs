namespace Library.Entities;

public class Reservation 
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

    public Reservation(string bookCode, Period period, Status status)
    {
        BookCode = bookCode;
        Period = period;
        Status = status;
    }
}

public enum Status
{
    Reserved = 0,
    Picked = 1,
    Returned = 2
}

public class Period
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
        if (dateFrom > dateTo)
            throw new InvalidOperationException("Impossible to have a period with dateTo previous to dateFrom");

        DateFrom = dateFrom;
        DateTo = dateTo;
    }
}
