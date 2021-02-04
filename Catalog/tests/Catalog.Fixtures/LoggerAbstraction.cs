using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Catalog.Fixtures
{
    public abstract class LoggerAbstraction<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public bool IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter) => throw new NotImplementedException();

        public abstract void Log(LogLevel logLevel, Exception ex, string information);
    }
}
