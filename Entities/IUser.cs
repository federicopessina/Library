namespace Library.Entities
{
    public interface IUser
    {
        Card Card { get; set; }
        Person Person { get; set; }
    }
}