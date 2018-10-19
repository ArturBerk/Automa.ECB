using System.Runtime.CompilerServices;
using Automa.Common;
using Automa.EntityComponents.Model;
using NUnit.Framework;

namespace Automa.EntityComponents.Internal
{
    [TestFixture]
    public class EntityTypeChunkTests
    {
        [Test]
        public void Construct()
        {
            var component1Type = ComponentType.Create<Component1>();
            var component2Type = ComponentType.Create<Component2>();
            EntityTypeChunk chunk = new EntityTypeChunk(null, EntityTypes.Type1);
            Assert.AreEqual(EntityTypes.Type1, chunk.EntityType);
            Assert.AreEqual(0, chunk.EntityCount);
            Assert.NotNull(chunk.ComponentDatas);
            Assert.AreEqual(ComponentTypeManager.TypeCount, chunk.ComponentDatas.Length);
            Assert.NotNull(chunk.ComponentDatas[0]);
            Assert.IsInstanceOf(typeof(ComponentData<Component1>), chunk.ComponentDatas[component1Type.TypeIndex]);
            Assert.IsInstanceOf(typeof(ComponentData<Component2>), chunk.ComponentDatas[component2Type.TypeIndex]);
            Assert.AreEqual(chunk.ComponentDatas[0], chunk.EntityData);
        }

        [Test]
        public void CreateEntity()
        {
            EntityTypeChunk chunk = new EntityTypeChunk(null, EntityTypes.Type1);
            for (int k = 0; k < 10; k++)
            {
                var entity = chunk.AddEntity(new Entity(), null);
                Assert.AreEqual(k, entity.IndexInChunk);
                Assert.AreEqual(entity, chunk.EntityData[k]);
                Assert.AreEqual(k + 1, chunk.EntityCount);
                Assert.AreEqual(chunk.EntityCount, chunk.EntityData.Count);
                for (int i = 0; i < EntityTypes.Type1.ComponentTypes.Length; i++)
                {
                    Assert.AreEqual(chunk.EntityCount, chunk.ComponentDatas[EntityTypes.Type1.ComponentTypes[i].TypeIndex].Count);
                }
            }
        }

        [Test]
        public void RemoveEntity()
        {
            var component1Type = ComponentType.Create<Component1>();
            var component2Type = ComponentType.Create<Component2>();
            EntityTypeChunk chunk = new EntityTypeChunk(null, EntityTypes.Type1);
            ArrayList<Entity> entities = new ArrayList<Entity>(5);
            ArrayList<int> values = new ArrayList<int>(5);
            for (int k = 0; k < 5; k++)
            {
                entities.Add(chunk.AddEntity(new Entity(), null));
                var component1Data = Data<Component1>(chunk.ComponentDatas[component1Type.TypeIndex]);
                var component2Data = Data<Component2>(chunk.ComponentDatas[component2Type.TypeIndex]);
                component1Data[k] = new Component1();
                component1Data[k].Value = k;
                component2Data[k].Value = k;
                values.Add(k);
            }
            chunk.RemoveEntity(entities[0], null);
            entities.UnorderedRemoveAt(0);
            values.UnorderedRemoveAt(0);
            Assert.AreEqual(4, chunk.EntityCount);
            for (int k = 0; k < chunk.EntityCount; k++)
            {
                Assert.AreEqual(entities[k], chunk.EntityData[k]);
                Assert.AreEqual(k, entities[k].IndexInChunk);
                ref var component1 = ref Data<Component1>(chunk.ComponentDatas[component1Type.TypeIndex])[k];
                Assert.AreEqual(values[k], component1.Value);
                ref var component2 = ref Data<Component2>(chunk.ComponentDatas[component2Type.TypeIndex])[k];
                Assert.AreEqual(values[k], component2.Value);
            }

            chunk.RemoveEntity(entities[1], null);
            entities.UnorderedRemoveAt(1);
            values.UnorderedRemoveAt(1);
            Assert.AreEqual(3, chunk.EntityCount);
            for (int k = 0; k < chunk.EntityCount; k++)
            {
                Assert.AreEqual(entities[k], chunk.EntityData[k]);
                Assert.AreEqual(k, entities[k].IndexInChunk);
                ref var component1 = ref Data<Component1>(chunk.ComponentDatas[component1Type.TypeIndex])[k];
                Assert.AreEqual(values[k], component1.Value);
                ref var component2 = ref Data<Component2>(chunk.ComponentDatas[component2Type.TypeIndex])[k];
                Assert.AreEqual(values[k], component2.Value);
            }


            chunk.RemoveEntity(entities[2], null);
            entities.UnorderedRemoveAt(2);
            values.UnorderedRemoveAt(2);
            Assert.AreEqual(2, chunk.EntityCount);
            for (int k = 0; k < chunk.EntityCount; k++)
            {
                Assert.AreEqual(entities[k], chunk.EntityData[k]);
                Assert.AreEqual(k, entities[k].IndexInChunk);
                ref var component1 = ref Data<Component1>(chunk.ComponentDatas[component1Type.TypeIndex])[k];
                Assert.AreEqual(values[k], component1.Value);
                ref var component2 = ref Data<Component2>(chunk.ComponentDatas[component2Type.TypeIndex])[k];
                Assert.AreEqual(values[k], component2.Value);
            }
        }

        [Test]
        public void AddedRemovedListenerTest()
        {
            EntityTypeChunk chunk = new EntityTypeChunk(null, EntityTypes.Type1);
            int count = 0;
            chunk.EntityAdded += (typeChunk, entity) => ++count;
            chunk.EntityRemoving += (typeChunk, entity) => --count;

            var entities = new Entity[10];
            for (int i = 0; i < 10; i++)
            {
                entities[i] = chunk.AddEntity(new Entity(), null);
            }
            Assert.AreEqual(10, count);
            for (int i = 0; i < 10; i++)
            {
                chunk.RemoveEntity(entities[i], null);
            }
            Assert.AreEqual(0, count);
        }

        public void RemovingListenerTest()
        {

        }

        private static ComponentData<T> Data<T>(ComponentData data)
        {
            return (ComponentData<T>) data;
        }
    }
}
