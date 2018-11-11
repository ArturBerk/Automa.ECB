using NUnit.Framework;

namespace Automa.Behaviours.Async.Task
{
    [TestFixture]
    public class BehaviourTreeBuilderTests
    {
        [Test]
        public void SyncBehaviours()
        {
            var tree = BehaviourTree.Builder
                .With(new SyncBehaviour1())
                .With(new SyncBehaviour2())
                .Build();
            Assert.AreEqual(0, tree.Groups.Length);
            Assert.AreEqual(0, tree.MainThreadBehaviours.Length);
            Assert.AreEqual(2, tree.SyncBehaviours.Length);
        }

        [Test]
        public void MainThreadBehaviours()
        {
            var tree = BehaviourTree.Builder
                .With(new MainThreadBehaviour1())
                .With(new MainThreadBehaviour2())
                .Build();
            Assert.AreEqual(0, tree.Groups.Length);
            Assert.AreEqual(2, tree.MainThreadBehaviours.Length);
            Assert.AreEqual(0, tree.SyncBehaviours.Length);
        }

        [Test]
        public void AsyncBehaviours()
        {
            var tree = BehaviourTree.Builder
                .With(new AsyncBehaviourModify1())
                .With(new AsyncBehaviourRead1())
                .With(new AsyncBehaviourModify2())
                .Build();
            Assert.AreEqual(2, tree.Groups.Length);
            Assert.AreEqual(0, tree.MainThreadBehaviours.Length);
            Assert.AreEqual(0, tree.SyncBehaviours.Length);

            Assert.AreEqual(2, tree.Groups[0].Behaviours.Length);
            Assert.IsInstanceOf<AsyncBehaviourModify1>(tree.Groups[0].Behaviours[0]);
            Assert.IsInstanceOf<AsyncBehaviourModify2>(tree.Groups[0].Behaviours[1]);
            Assert.AreEqual(1, tree.Groups[1].Behaviours.Length);
            Assert.IsInstanceOf<AsyncBehaviourRead1>(tree.Groups[1].Behaviours[0]);
        }
    }
}