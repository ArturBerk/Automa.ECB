using System.Diagnostics;
using System.Threading;
using Automa.Tasks;
using NUnit.Framework;

namespace Automa.Behaviours.Async.Task
{
    [TestFixture]
    public class BehaviourTreeTests
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
        public void OnlySyncBehaviours()
        {
            var testBehaviours = new TestBehaviour[]
            {
                new SyncBehaviour1(),
                new SyncBehaviour2()
            };
            var tree = BehaviourTree.Builder.With(testBehaviours).Build();
            var handler = tree.Start(taskManager);
            handler.Complete();

            Assert.IsTrue(testBehaviours[0].Executed);
            Assert.IsTrue(testBehaviours[1].Executed);
            Assert.AreNotEqual(Thread.CurrentThread, testBehaviours[0].ExecutedThread);
            Assert.AreNotEqual(Thread.CurrentThread, testBehaviours[1].ExecutedThread);
        }

        [Test]
        public void OnlyMainThreadBehaviours()
        {
            var testBehaviours = new TestBehaviour[]
            {
                new MainThreadBehaviour1(), 
                new MainThreadBehaviour2()
            };
            var tree = BehaviourTree.Builder.With(testBehaviours).Build();
            var handler = tree.Start(taskManager);
            handler.Complete();

            Assert.IsTrue(testBehaviours[0].Executed);
            Assert.IsTrue(testBehaviours[1].Executed);
            Assert.AreEqual(Thread.CurrentThread, testBehaviours[0].ExecutedThread);
            Assert.AreEqual(Thread.CurrentThread, testBehaviours[1].ExecutedThread);
        }

        [Test]
        public void OnlyAsyncBehaviours()
        {
            var testBehaviours = new TestBehaviour[]
            {
                new AsyncBehaviourModify1(), 
                new AsyncBehaviourModify2(),
                new AsyncBehaviourRead1()
            };
            var tree = BehaviourTree.Builder.With(testBehaviours).Build();
            var handler = tree.Start(taskManager);
            handler.Complete();

            Assert.IsTrue(testBehaviours[0].Executed);
            Assert.IsTrue(testBehaviours[1].Executed);
            Assert.IsTrue(testBehaviours[2].Executed);
            Assert.AreNotEqual(Thread.CurrentThread, testBehaviours[0].ExecutedThread);
            Assert.AreNotEqual(Thread.CurrentThread, testBehaviours[1].ExecutedThread);
            Assert.AreNotEqual(Thread.CurrentThread, testBehaviours[2].ExecutedThread);
        }

        [Test]
        public void AllBehaviours()
        {
            ExecutionOrder.Executed.Clear();
            var testBehaviours = new TestBehaviour[]
            {
                new MainThreadBehaviour1(),
                new MainThreadBehaviour2(),
                new SyncBehaviour1(),
                new SyncBehaviour2(),
                new AsyncBehaviourModify1(),
                new AsyncBehaviourModify2(),
                new AsyncBehaviourRead1(),
                new AsyncBehaviourModify1(),
                new AsyncBehaviourModify2(),
                new AsyncBehaviourRead1(),
                new AsyncBehaviourModify1(),
                new AsyncBehaviourModify2(),
                new AsyncBehaviourRead1()
            };
            var tree = BehaviourTree.Builder.With(testBehaviours).Build();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var handler = tree.Start(taskManager);
            handler.Complete();
            stopwatch.Stop();
            var elapsed1 = stopwatch.Elapsed;

            for (int i = 0; i < 7; i++)
            {
                Assert.IsTrue(testBehaviours[i].Executed);
            }
            for (int i = 0; i < 2; i++)
            {
                Assert.AreEqual(Thread.CurrentThread.ManagedThreadId, testBehaviours[i].ExecutedThread);
            }
            for (int i = 2; i < 7; i++)
            {
                Assert.AreNotEqual(Thread.CurrentThread.ManagedThreadId, testBehaviours[i].ExecutedThread);
            }
            Assert.AreEqual(testBehaviours.Length, ExecutionOrder.Executed.Count);
            Assert.IsInstanceOf<SyncBehaviour1>(ExecutionOrder.Executed[testBehaviours.Length - 4]);
            Assert.IsInstanceOf<SyncBehaviour2>(ExecutionOrder.Executed[testBehaviours.Length - 3]);
            Assert.IsInstanceOf<MainThreadBehaviour1>(ExecutionOrder.Executed[testBehaviours.Length - 2]);
            Assert.IsInstanceOf<MainThreadBehaviour2>(ExecutionOrder.Executed[testBehaviours.Length - 1]);

            stopwatch.Restart();
            foreach (var testBehaviour in testBehaviours)
            {
                testBehaviour.Apply();
            }
            stopwatch.Stop();
            var elapsed2 = stopwatch.Elapsed;
        }
    }
}
