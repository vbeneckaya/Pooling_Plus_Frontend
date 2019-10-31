using System.Threading;
using System.Threading.Tasks;

namespace Tasks.Common
{
    public interface IScheduledTask
    {
        string Schedule { get; }
        string TaskName { get; }
        Task Execute(string consoleParameters, CancellationToken cancellationToken);
    }
}
