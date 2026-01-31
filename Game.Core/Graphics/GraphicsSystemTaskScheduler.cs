using System.Collections.Concurrent;

namespace Game.Core.Graphics
{
    public class GraphicsSystemTaskScheduler(int mainThreadID) : TaskScheduler
    {
        private readonly BlockingCollection<Task> _tasks = [];
        private readonly int _mainThreadID = mainThreadID;

        public void FlushQueuedTasks()
        {
            while (_tasks.TryTake(out Task t))
                TryExecuteTask(t);
        }

        protected override IEnumerable<Task> GetScheduledTasks() => [.. _tasks];
        protected override void QueueTask(Task task) => _tasks.Add(task);
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => Environment.CurrentManagedThreadId == _mainThreadID && TryExecuteTask(task);
        public void Shutdown() => _tasks.CompleteAdding();

        public Task<T> ExecuteOnMainThread<T>(Func<T> func)
        {
            if (Environment.CurrentManagedThreadId == _mainThreadID)
                return Task.FromResult(func());

            return Task.Factory.StartNew(func, CancellationToken.None, TaskCreationOptions.None, this);
        }

        public Task ExecuteOnMainThread(Action action)
        {
            if (Environment.CurrentManagedThreadId == _mainThreadID)
            {
                action();
                return Task.CompletedTask;
            }

            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this);
        }
    }
}
