namespace Application.Exceptions
{
    public class LoginException : Exception
    {

        public LoginException() : base("Invalid credentials") { }
    }
}
