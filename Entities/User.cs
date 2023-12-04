namespace Library.Entities;

public sealed class User 
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
