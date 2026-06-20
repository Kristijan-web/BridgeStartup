using Application.ExceptionLogging;
using Microsoft.Extensions.Logging;

namespace Implementation.ExceptionLogging
{
    public class ConsoleLogging : IExceptionLogger
    {

        private readonly ILogger<ConsoleLogging> _logger;
        public ConsoleLogging(ILogger<ConsoleLogging> logger)
        {
            _logger = logger;
        }
        public Guid Log(Exception ex)
        {
            Guid id = new Guid();

            string logMessage = ex.Message;
            string logStackTrace = ex.StackTrace;
            DateTime timeOfLog = DateTime.Now;

            string FullLogMessage = $"id: {id}, time of error: {timeOfLog}, error message: {logMessage}, stack trace: {logStackTrace}";

            _logger.LogInformation(FullLogMessage);

            // Da li mogu kroz Di da ubacim ovde i ILogger?

            return id;


        }
    }
}
