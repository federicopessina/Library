using System.ComponentModel.DataAnnotations;

namespace Library.Entities;

public sealed class Person
{
    #region Properties
    /// <summary>
    /// Id code.
    /// </summary>
    /// <remarks>Primary key of a user.</remarks>
    [Required]
    public string Id { get; set; }
    public string? Name { get; set; } = null;
    public string? Surname { get; set; } = null;
    public Address? Address { get; set; } = null;
    #endregion

    #region Constructors
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Person() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Person(string id)
    {
        Id = id;
    }
    public Person(string id, string name, string surname, Address? address)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Address = address;
    }
    #endregion
}

public class Address
{
    public string? Street { get; set; } = null;
    public string? Number { get; set; } = null;
    public string? PostCode { get; set; } = null;
    public Address()
    {

    }
    public Address(string street, string number, string postCode)
    {
        Street = street;
        Number = number;
        PostCode = postCode;
    }
}
