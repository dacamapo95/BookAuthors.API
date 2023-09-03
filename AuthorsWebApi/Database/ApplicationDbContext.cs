using AuthorsWebApi.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Database
{
    public class ApplicationDbContext : IdentityDbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Author_Book>()
                .HasKey(association => new { association.AuthorId, association.BookId });
        }

        public ApplicationDbContext(DbContextOptions options) : base(options) { }   

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<BookComment> BookComments { get; set; }

        public DbSet<Author_Book> Author_Books { get; set; }

       
    }

   
}
