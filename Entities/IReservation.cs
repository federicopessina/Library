namespace Library.Entities
{
    public interface IReservation
    {
        string BookCode { get; }
        Period Period { get; }
        Status Status { get; set; }
    }
}