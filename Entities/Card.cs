namespace Library.Entities;

public class Card
{
    public int Number { get; }
    public bool IsBlocked { get; set; } = false;

    public Card()
    {

    }
    public Card(int number)
    {
        Number = number;
    }

    public Card(int number, bool isBlocked = false)
    {
        Number = number;
        IsBlocked = isBlocked;
    }

    public Card Clone()
    {
        return (Card)this.MemberwiseClone();
    }
}
