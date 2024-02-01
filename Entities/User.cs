namespace Library.Entities;

public sealed class User 
{
    public int CardNumber { get; set; }
    public string PersonId { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public User()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {

    }
    public User(int cardNumber, string personId)
    {
        CardNumber = cardNumber;
        PersonId = personId;
    }
}
