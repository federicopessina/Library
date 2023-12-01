using Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Core.API.Data
{
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
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        {

        }
    }
}
