using System;
using System.IO;

namespace Logger
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileAnyTimeWritter = new LogWritter(new FileLogWritter(), new AnyTimeWriter(), new StopWritter());
            var consoleAnyTimeWritter = new LogWritter(new ConsoleLogWritter(), new AnyTimeWriter(), new StopWritter());
            var fileSpecificDayWritter = new LogWritter(new FileLogWritter(), new SpecificDayWriter(DayOfWeek.Friday), new StopWritter());
            var consoleSpecificDayWritter = new LogWritter(new ConsoleLogWritter(), new SpecificDayWriter(DayOfWeek.Friday), new StopWritter());

            var pathfinder1 = new Pathfinder(fileAnyTimeWritter);
            var pathfinder2 = new Pathfinder(consoleAnyTimeWritter);
            var pathfinder3 = new Pathfinder(fileSpecificDayWritter);
            var pathfinder4 = new Pathfinder(consoleSpecificDayWritter);
            var pathfinder5 = new Pathfinder(new LogWritter(new ConsoleLogWritter(), new AnyTimeWriter(), fileSpecificDayWritter));
        }
    }

    public class Pathfinder
    {
        private readonly ILogWritter _logWritter;

        public Pathfinder(ILogWritter logWritter)
        {
            _logWritter = logWritter;
        }

        public void Find()
        {
            string message = "";
            _logWritter.WriteError(message);
        }
    }

    public interface ILogWritterPolicy
    {
        void WriteError(string message);
    }

    public interface ILogWritter : ILogWritterPolicy
    {
        ILogWriteTimePolicy TimePolicy { get; }
    }

    public class ConsoleLogWritter : ILogWritterPolicy
    {
        public void WriteError(string message) => Console.WriteLine(message);
    }

    public class FileLogWritter : ILogWritterPolicy
    {
        public void WriteError(string message) => File.WriteAllText("log.txt", message);
    }

    public class LogWritter : ILogWritter
    {
        private readonly ILogWritter _logWritter;

        public LogWritter(ILogWritterPolicy writePolicy, ILogWriteTimePolicy logWriteTimePolicy, ILogWritter logWritter)
        {
            _logWritter = logWritter;
            WritePolicy = writePolicy;
            TimePolicy = logWriteTimePolicy;
        }

        public ILogWriteTimePolicy TimePolicy { get; }
        private ILogWritterPolicy WritePolicy { get; }

        public void WriteError(string message)
        {
            if (TimePolicy.CanWrite)
                WritePolicy.WriteError(message);

            _logWritter.WriteError(message);
        }
    }

    public class StopWritter : ILogWritter
    {
        public StopWritter()
        {
            TimePolicy = new AnyTimeWriter();
        }

        public void WriteError(string message) { }

        public ILogWriteTimePolicy TimePolicy { get; }
    }

    public interface ILogWriteTimePolicy
    {
        bool CanWrite { get; }
    }

    public class AnyTimeWriter : ILogWriteTimePolicy
    {
        public bool CanWrite => true;
    }

    public class SpecificDayWriter : ILogWriteTimePolicy
    {
        private readonly DayOfWeek _needWriteDay;

        public SpecificDayWriter(DayOfWeek needWriteDay)
        {
            _needWriteDay = needWriteDay;
        }

        public bool CanWrite => DateTime.Now.DayOfWeek == _needWriteDay;
    }
}