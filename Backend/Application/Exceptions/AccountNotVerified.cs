namespace Application.Exceptions
{
    public class AccountNotVerified : Exception
    {
        public AccountNotVerified(string message) : base(message) { }
    }
}
