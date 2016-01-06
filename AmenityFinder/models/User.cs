namespace AmenityFinder.models
{
    public class User: AbstractModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AccessTokenModel : AbstractModel
    {
        public string AccessToken { get; set; }
    }

    public class AuthenticationResponseModel : AbstractModel
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Error { get; set; }
    }
}
