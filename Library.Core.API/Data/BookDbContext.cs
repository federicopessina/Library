using Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Core.API.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class BookDbContext : DbContext //TODO Implement.
    {
        /// <summary>
        /// Representation of Book table in the database.
        /// </summary>
        /// <remarks>Allows to manipulate data from the table Book.</remarks>
        public DbSet<Book> Books { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <remarks>Allows to set some optins needed by the DbContext (e.g. connection string).</remarks>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options) { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
