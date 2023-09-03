namespace AuthorsWebApi.DataTransferObjects
{
    public class UserAuthenticationResponse
    { 
        public string Token { get; set; }

        public DateTime ExpirationDate { get; set; }

    }
}
