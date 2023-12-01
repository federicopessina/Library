using System.ComponentModel.DataAnnotations;

namespace Library.Entities;

public class Person
{
    #region Properties
    /// <summary>
    /// Id code.
    /// </summary>
    /// <remarks>Primary key of a user.</remarks>
    [Required]
    public string IdCode { get; set; }
    public string? Name { get; set; } = null;
    public string? Surname { get; set; } = null;
    public Address? Address { get; set; } = null;
    #endregion

    #region Constructors
    public Person()
    {

    }
    public Person(string idCode)
    {
        IdCode = idCode;
    }
    public Person(string idCode, string name, string surname, Address? address)
    {
        IdCode = idCode;
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
