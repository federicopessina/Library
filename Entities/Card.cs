namespace Library.Entities;

public class Card
{
    public int Number { get; set; }
    public bool IsBlocked { get; set; } = false;

    public Card()
    {

    }
    public Card(int number, bool isBlocked = false)
    {
        Number = number;
        IsBlocked = isBlocked;
    }
}
