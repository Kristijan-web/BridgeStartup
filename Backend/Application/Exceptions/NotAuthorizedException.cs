namespace Application.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(string UseCase) : base($"You are not authorized to perform this action ({UseCase})") { }
    }
}
