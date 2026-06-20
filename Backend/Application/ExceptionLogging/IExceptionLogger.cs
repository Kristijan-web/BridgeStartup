namespace Application.ExceptionLogging
{
    public interface IExceptionLogger
    {
        public Guid Log(Exception ex);
    }
}
