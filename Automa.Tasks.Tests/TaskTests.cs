using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace Automa.Tasks.Tests
{
    [TestFixture]
    public class TaskTests
    {
        private TaskManager taskManager;

        [OneTimeSetUp]
        public void Setup()
        {
            taskManager = new TaskManager();
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            //taskManager?.Dispose();
        }

        [Test]
        public void WaitSingleTask()
        {
            var task = new CountTask();
            taskManager.Schedule(task);
            task.Wait();
            Assert.AreEqual(1000, task.Count);
        }

        [Test]
        public void MultiTask()
        {
            var tasks = new []
            {
                new CountTask(),
                new CountTask(),
                new CountTask(),
            };
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            taskManager.Schedule(tasks);
            tasks.WaitAll();
            stopwatch.Stop();
            foreach (var task in tasks)
            {
                Assert.AreEqual(1000, task.Count);
            }
        }

        [Test]
        public void SubTask()
        {
            var aTask = new AggregateTask<CountTask>
            {
                Tasks = new []
                {
                    new CountTask(),
                    new CountTask(),
                    new CountTask(),
                }
            };
            taskManager.Schedule(aTask);
            aTask.Wait();
            foreach (var task in aTask.Tasks)
            {
                Assert.AreEqual(1000, task.Count);
            }
        }

        private class CountTask : Task
        {
            public int Count;

            public override void Execute()
            {
                Count = 0;
                for (int i = 0; i < 1000; i++)
                {
                    ++Count;
                }
            }
        }

        private class AggregateTask<T> : Task where T : Task
        {
            public IEnumerable<T> Tasks { get; set; }

            public override void Execute()
            {
                CurrentTaskManager.Schedule(Tasks);
                Tasks.WaitAll();
            }
        }

    }
}
