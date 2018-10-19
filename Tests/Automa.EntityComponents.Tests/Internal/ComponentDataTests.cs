using NUnit.Framework;

namespace Automa.EntityComponents.Internal
{
    [TestFixture]
    public class ComponentDataTests
    {
        [Test]
        public void Add()
        {
            ComponentData<int> data = new ComponentData<int>();
            for (int i = 0; i < 100; i++)
            {
                data.Add();
            }
            Assert.AreEqual(100, data.Count);
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(0, data[i]);
            }
        }

        [Test]
        public void AddAndSet()
        {
            ComponentData<int> data = new ComponentData<int>();
            for (int i = 0; i < 100; i++)
            {
                data.Add();
                data.Set(i, ref i);
            }
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i, data[i]);
            }
        }

        [Test]
        public void RemoveFromEnd()
        {
            ComponentData<int> data = new ComponentData<int>();
            for (int i = 0; i < 10; i++)
            {
                data.Add();
                data.Set(i, ref i);
            }
            for (int i = 9; i >= 0; i--)
            {
                Assert.IsFalse(data.Remove(i));
                Assert.AreEqual(i, data.Count);
                for (int j = 0; j < i; j++)
                {
                    Assert.AreEqual(j, data[j]);
                }
            }
        }

        [Test]
        public void RemoveFromCenter()
        {
            ComponentData<int> data = new ComponentData<int>();
            for (int i = 0; i < 5; i++)
            {
                data.Add();
                data.Set(i, ref i);
            }
            Assert.IsTrue(data.Remove(2));
            Assert.AreEqual(4, data.Count);

            Assert.AreEqual(0, data[0]);
            Assert.AreEqual(1, data[1]);
            Assert.AreEqual(4, data[2]);
            Assert.AreEqual(3, data[3]);

            Assert.IsTrue(data.Remove(0));
            Assert.AreEqual(3, data.Count);

            Assert.AreEqual(3, data[0]);
            Assert.AreEqual(1, data[1]);
            Assert.AreEqual(4, data[2]);
        }
    }
}
