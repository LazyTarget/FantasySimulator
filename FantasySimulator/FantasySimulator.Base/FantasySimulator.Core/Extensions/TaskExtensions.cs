using System.Threading.Tasks;

namespace FantasySimulator.Core
{
    public static class TaskExtensions
    {
        public static TResult WaitForResult<TResult>(this Task<TResult> task)
        {
            task.Wait();
            return task.Result;
        }

        public static TResult WaitForResult<TResult>(this Task<TResult> task, int millisecondsTimeout)
        {
            task.Wait(millisecondsTimeout);
            return task.Result;
        }
    }
}
