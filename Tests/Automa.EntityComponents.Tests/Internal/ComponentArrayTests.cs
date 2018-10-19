using Automa.Common;
using Automa.EntityComponents.Model;
using NUnit.Framework;

namespace Automa.EntityComponents.Internal
{
    [TestFixture]
    public class ComponentArrayTests
    {
        private ArrayList<EntityTypeChunk> chunks = new ArrayList<EntityTypeChunk>(4);
        private Entity[] entities;

        [OneTimeSetUp]
        public void Setup()
        {
            entities = new Entity[30];
            var entityTypeChunk = new EntityTypeChunk(null, EntityTypes.Type1);
            chunks.Add(entityTypeChunk);
            for (int i = 0; i < 10; i++)
            {
                entities[i] = chunks[0].AddEntity(new Entity(), null);
                entities[i].GetComponent<Component2>().Value = i;
            }

            entityTypeChunk = new EntityTypeChunk(null, EntityTypes.Type2);
            chunks.Add(entityTypeChunk);
            for (int i = 0; i < 10; i++)
            {
                entities[i + 10] = chunks[1].AddEntity(new Entity(), null);
                entities[i + 10].GetComponent<Component2>().Value = i + 10;
            }

            entityTypeChunk = new EntityTypeChunk(null, EntityTypes.Type3);
            chunks.Add(entityTypeChunk);
            for (int i = 0; i < 10; i++)
            {
                entities[i + 20] = chunks[2].AddEntity(new Entity(), null);
                entities[i + 20].GetComponent<Component2>().Value = i + 20;
            }
        }

        [Test]
        public void UpdateChunks()
        {
            ComponentArray<Component2> array = new ComponentArray<Component2>();
            var typeIndex = ComponentType.Create<Component2>().TypeIndex;
            Assert.AreEqual(typeIndex, array.TypeIndex);
            array.UpdateChunks(ref chunks);
            Assert.AreEqual(3, array.componentDatas.Length);
            Assert.AreEqual(array.componentDatas[0], chunks[0].ComponentDatas[typeIndex]);
            Assert.AreEqual(array.componentDatas[1], chunks[1].ComponentDatas[typeIndex]);
            Assert.AreEqual(array.componentDatas[2], chunks[2].ComponentDatas[typeIndex]);
        }

        [Test]
        public void GetByEnumerator()
        {
            Group group = new Group();
            group.Chunks = chunks;
            ComponentArray<Component2> array = new ComponentArray<Component2>();
            array.UpdateChunks(ref chunks);
            EntityEnumerator enumerator = new EntityEnumerator();
            enumerator.ApplyGroup(group);

            int entityCount = 0;
            while (enumerator.MoveNext())
            {
                var component = array.Get(enumerator);
                Assert.AreEqual(entityCount, component.Value);
                ++entityCount;
            }
        }

        [Test]
        public void GetByIndex()
        {
            ComponentArray<Component2> array = new ComponentArray<Component2>();
            array.UpdateChunks(ref chunks);
            var arrayCount = 30;
            for (int i = 0; i < arrayCount; i++)
            {
                Assert.AreEqual(i, array[i].Value);
            }
        }

    }
}
