namespace AuthorsWebApi.DataTransferObjects
{
    public class BookDTO
    {
        public int BookId { get; set; }

        public string Title { get; set; }

        public DateTime PublicationDate { get; set; }

        public List<BookCommentDTO> Comments { get; set; }

        public List<AuthorDTO> Authors { get; set; }
    }
}
