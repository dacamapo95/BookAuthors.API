using AuthorsWebApi.DataTransferObjects;
using AuthorsWebApi.Entities;
using AutoMapper;

namespace AuthorsWebApi.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<NewAuthorDTO, Author>();
            CreateMap<AuthorDTO, Author>();
            CreateMap<NewBookDTO, Book>();  

            CreateMap<Author, AuthorDTO>()
                .ForMember(authorDto => authorDto.Books, options => options.MapFrom(MapBooksFromAuthor));

            CreateMap<NewBookDTO, Book>()
                .ForMember(book => book.Authors, options => options.MapFrom(MapAuthorsFromBookDto));

            CreateMap<Book, BookDTO>()
                .ForMember(bookDto => bookDto.Authors, options => options.MapFrom(MapAuthorsFromBook));

            CreateMap<BookPatchDTO, Book>().ReverseMap();


            CreateMap<NewBookCommentDTO, BookComment>();

            CreateMap<BookCommentDTO , BookComment>();

            CreateMap<BookComment, BookCommentDTO>();
        }

        private List<BookDTO> MapBooksFromAuthor(Author author, AuthorDTO authorDTO)
        {
            if (author.Books == null) return new List<BookDTO>();
            return author.Books
                         .Select(association => new BookDTO()
                         {
                             BookId = association.BookId,
                             Title = association.Book.Title
                         }).ToList();
        }

        private List<AuthorDTO> MapAuthorsFromBook(Book book, BookDTO bookDto)
        {
            if (book.Authors is null) return new List<AuthorDTO>();
                                                     
            return
            book.Authors
            .OrderBy(association => association.Order)
            .Select(association => new AuthorDTO()
            {
                AuthorId = association.AuthorId,
                Name = association.Author?.Name
            }).ToList();
        }

        private List<Author_Book> MapAuthorsFromBookDto(NewBookDTO newBook, Book book)
        {
            var Associations = new List<Author_Book>();

            if (newBook.AuthorIds == null) return Associations;

            newBook.AuthorIds
                   .Select((authorId, index) => new Author_Book() { AuthorId = authorId, Order = index + 1 })
                   .ToList()
                   .ForEach(association => Associations.Add(association));

            return Associations;
        }
    }
}
