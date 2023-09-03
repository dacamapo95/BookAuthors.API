namespace AuthorsWebApi.DataTransferObjects
{
    public class AuthorDTO
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }

        public List<BookDTO> Books { get; set; }
    }
}
