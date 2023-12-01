namespace Library.Entities;

public class User : IUser
{
    public Card Card { get; set; }
    public Person Person { get; set; }
    public User()
    {

    }
    public User(Card card, Person person)
    {
        Card = card;
        Person = person;
    }
}
