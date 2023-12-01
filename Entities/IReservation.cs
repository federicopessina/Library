namespace Library.Entities
{
    public interface IReservation
    {
        Book Book { get; }
        Period Period { get; }
        Status Status { get; set; }
    }
}