using System;
using Inferno.Core;
using Inferno.Core.Logging;
using System.Collections.Generic;

namespace WorldCup.Bootstrap
{
    public interface IItemsControlLogger : ILogger
    {
        IList<string> ItemsSource { get; }
    }

    public class ItemsControlLogger : IItemsControlLogger
    {
        public ItemsControlLogger()
        {
            ItemsSource = new BindableCollection<string>();
        }

        public IList<string> ItemsSource { get; }

        public void Log(LogEntry entry)
        {
            if (entry.Severity == LoggingEventType.Information)
            {
                ItemsSource.Add($"[{DateTime.Now.ToLongTimeString()}] {(entry.Classifier != null ? $"{entry.Classifier}: " : null)}{entry.Message}");
            }
        }
    }
}
