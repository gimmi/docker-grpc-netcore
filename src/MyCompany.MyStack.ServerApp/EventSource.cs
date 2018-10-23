using Microsoft.Extensions.Logging;
using System;

namespace MyCompany.MyStack.ServerApp
{
    public class EventSource
    {
        private readonly ILogger _logger;
        public EventHandler EventPublished;

        public EventSource(ILogger<EventSourceHostedService> logger)
        {
            _logger = logger;
        }

        public void Publish()
        {
            EventPublished?.Invoke(this, EventArgs.Empty);
        }
    }
}
