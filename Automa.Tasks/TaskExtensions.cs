using System.Collections.Generic;

namespace Automa.Tasks
{
    public static class TaskExtensions
    {


        public static void Schedule<T>(this TaskManager taskManager, params T[] tasks) where T : Task
        {
            for (var index = 0; index < tasks.Length; index++)
            {
                var task = tasks[index];
                taskManager.Schedule(task);
            }
        }

        public static void Schedule<T>(this TaskManager taskManager, IList<T> tasks) where T : Task
        {
            for (var index = 0; index < tasks.Count; index++)
            {
                var task = tasks[index];
                taskManager.Schedule(task);
            }
        }

        public static void Schedule<T>(this TaskManager taskManager, IEnumerable<T> tasks) where T : Task
        {
            foreach (var task in tasks)
            {
                taskManager.Schedule(task);
            }
        }

        public static void WaitAll<T>(this IList<T> tasks) where T : Task
        {
            for (var index = 0; index < tasks.Count; index++)
            {
                var task = tasks[index];
                if (!task.Completed.IsSet)
                {
                    task.Completed.Wait();
                }
            }
        }

        public static void WaitAll<T>(this T[] tasks) where T : Task
        {
            for (var index = 0; index < tasks.Length; index++)
            {
                var task = tasks[index];
                if (!task.Completed.IsSet)
                {
                    task.Completed.Wait();
                }
            }
        }

        public static void WaitAll<T>(this IEnumerable<T> tasks) where T : Task
        {
            foreach (var task in tasks)
            {
                if (!task.Completed.IsSet)
                {
                    task.Completed.Wait();
                }
            }
        }
    }
}
