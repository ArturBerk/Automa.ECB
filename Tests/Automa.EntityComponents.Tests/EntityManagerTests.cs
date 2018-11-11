using System;
using Automa.Common;
using Automa.EntityComponents.Model;
using NUnit.Framework;

namespace Automa.EntityComponents
{
    [TestFixture]
    public class EntityManagerTests
    {
        [Test]
        public void AddEntity()
        {
            var entityManager = new EntityManager();
            var entityType = EntityTypes.Type1;
            for (int i = 0; i < 10; i++)
            {
                var entity = entityManager.CreateEntity(entityType);
                Assert.NotNull(entity.Chunk);
                Assert.AreEqual(i, entity.IndexInChunk);
            }
        }

        [Test]
        public void RemoveEntity()
        {
            var entityManager = new EntityManager();
            var entityType = EntityTypes.Type1;
            var entities = new ArrayList<Entity>(10);
            var values = new ArrayList<int>(10);
            for (int i = 0; i < 10; i++)
            {
                entities[i] = entityManager.CreateEntity(entityType);
                entities[i].SetComponent(new Component1 { Value = i });
                entities[i].GetComponent<Component2>().Value = i;
                values[i] = i;
            }
            var random = new Random();
            while (entities.Count > 0)
            {
                var removeIndex = random.Next(entities.Count);
                entityManager.RemoveEntity(entities[removeIndex]);
                values.UnorderedRemoveAt(removeIndex);
                entities.UnorderedRemoveAt(removeIndex);
                for (int i = 0; i < entities.Count; i++)
                {
                    Assert.AreEqual(values[i], entities[i].GetComponent<Component1>().Value);
                    Assert.AreEqual(values[i], entities[i].GetComponent<Component2>().Value);
                }
            }
        }

        [Test]
        public void ChangeEntityType()
        {
            var entityType1 = EntityTypes.Type1;
            var entityType2 = EntityTypes.Type3;
            var entityManager = new EntityManager();
            var entity = entityManager.CreateEntity(entityType1);
            var chunk1 = entity.Chunk;
            entityManager.ChangeEntityType(entity, entityType2);
            Assert.AreNotEqual(chunk1, entity.Chunk);
            Assert.AreEqual(0, entity.IndexInChunk);
        }

        [Test]
        public void BindGroupsAndEnumerators()
        {
            var entityManager = new EntityManager();
            //            for (int i = 0; i < 10; i++)
            //            {
            //                entityManager.CreateEntity(EntityTypes.Type1);
            //            }
            //            for (int i = 0; i < 10; i++)
            //            {
            //                entityManager.CreateEntity(EntityTypes.Type2);
            //            }
            //            for (int i = 0; i < 10; i++)
            //            {
            //                entityManager.CreateEntity(EntityTypes.Type3);
            //            }
            entityManager.BindGroupsAndEnumerators(this);
            Assert.NotNull(testGroup);
            Assert.NotNull(testGroup.Entity);
            Assert.NotNull(testGroup.Component1);
            Assert.NotNull(testGroup.Component2);
            Assert.NotNull(testEntity);
            Assert.NotNull(testEntity.Entity);
            Assert.NotNull(testEntity.Component1);
            Assert.NotNull(testEntity.Component2);
        }

        [Test]
        public void BindGroup()
        {
            var entityManager = new EntityManager();
            for (int i = 0; i < 10; i++)
            {
                var e = entityManager.CreateEntity(EntityTypes.Type1);
                e.SetComponent(new Component1 { Value = i });
                e.GetComponent<Component2>().Value = i;
            }
            for (int i = 0; i < 10; i++)
            {
                var e = entityManager.CreateEntity(EntityTypes.Type2);
                e.SetComponent(new Component1 { Value = i + 10 });
                e.GetComponent<Component2>().Value = i + 10;
            }
            for (int i = 0; i < 10; i++)
            {
                entityManager.CreateEntity(EntityTypes.Type3);
            }
            var group = new TestGroup();
            entityManager.BindGroup(group);
            Assert.NotNull(group.Entity);
            Assert.NotNull(group.Component1);
            Assert.NotNull(group.Component2);

            var count = group.Entity.EvaluateCount;
            Assert.AreEqual(20, count);
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(i, group.Component1[i].Value);
                Assert.AreEqual(i, group.Component2[i].Value);
            }
        }

        [Test]
        public void BindEnumerator()
        {
            var entityManager = new EntityManager();
            for (int i = 0; i < 10; i++)
            {
                var e = entityManager.CreateEntity(EntityTypes.Type1);
                e.SetComponent(new Component1 { Value = i });
                e.GetComponent<Component2>().Value = i;
            }
            for (int i = 0; i < 10; i++)
            {
                var e = entityManager.CreateEntity(EntityTypes.Type2);
                e.SetComponent(new Component1 { Value = i + 10 });
                e.GetComponent<Component2>().Value = i + 10;
            }
            for (int i = 0; i < 10; i++)
            {
                entityManager.CreateEntity(EntityTypes.Type3);
            }
            var entity = new TestEntity();
            entityManager.BindEnumerator(entity);
            Assert.NotNull(entity.Entity);
            Assert.NotNull(entity.Component1);
            Assert.NotNull(entity.Component2);

            entity.Reset();
            int index = 0;
            while (entity.MoveNext())
            {
                Assert.AreEqual(index, entity.Component1.Value.Value);
                Assert.AreEqual(index, entity.Component2.Value.Value);
                ++index;
            }
        }

        [Test]
        public void ReactiveGroup()
        {
            var group = new ReactiveTestGroup();
            var entityManager = new EntityManager();
            entityManager.BindGroup(group);
            var entities = new Entity[30];
            for (int i = 0; i < 10; i++)
            {
                var e = entityManager.CreateEntity(EntityTypes.Type1);
                e.SetComponent(new Component1 { Value = i });
                e.GetComponent<Component2>().Value = i;
                entities[i] = e;
            }
            for (int i = 0; i < 10; i++)
            {
                var e = entityManager.CreateEntity(EntityTypes.Type2);
                e.SetComponent(new Component1 { Value = i + 10 });
                e.GetComponent<Component2>().Value = i + 10;
                entities[i + 10] = e;
            }
            for (int i = 0; i < 10; i++)
            {
                entities[i + 20] = entityManager.CreateEntity(EntityTypes.Type3);
            }
            Assert.AreEqual(20, group.AddedFired);
            Assert.AreEqual(0, group.RemovingFired);
            foreach (var entity in entities)
            {
                entity.Remove();
            }
            Assert.AreEqual(20, group.AddedFired);
            Assert.AreEqual(20, group.RemovingFired);
        }


        [Test]
        public void ReactiveChangeTypeGroup()
        {
            var group = new ReactiveTestGroup();
            var entityManager = new EntityManager();
            entityManager.BindGroup(group);
            var entities = new Entity[10];
            for (int i = 0; i < 10; i++)
            {
                var e = entityManager.CreateEntity(EntityTypes.Type1);
                e.SetComponent(new Component1 { Value = i });
                e.GetComponent<Component2>().Value = i;
                entities[i] = e;
            }
            for (int i = 0; i < 10; i++)
            {
                var e = entityManager.CreateEntity(EntityTypes.Type2);
                e.SetComponent(new Component1 { Value = i + 10 });
                e.GetComponent<Component2>().Value = i + 10;
            }
            for (int i = 0; i < 10; i++)
            {
                entityManager.CreateEntity(EntityTypes.Type3);
            }
            Assert.AreEqual(20, group.AddedFired);
            Assert.AreEqual(0, group.RemovingFired);
            foreach (var entity in entities)
            {
                entity.SetType(EntityTypes.Type2);
            }
            Assert.AreEqual(20, group.AddedFired);
            Assert.AreEqual(0, group.RemovingFired);
            foreach (var entity in entities)
            {
                entity.SetType(EntityTypes.Type3);
            }
            Assert.AreEqual(20, group.AddedFired);
            Assert.AreEqual(10, group.RemovingFired);
        }

        private TestGroup testGroup;
        private TestEntity testEntity;

        private class TestGroup : IGroup
        {
            public IArray<Entity> Entity;
            public IArray<Component1> Component1;
            public IArray<Component2> Component2;
        }

        private class ReactiveTestGroup : IGroup, IEntityAddedListener, IEntityRemovingListener
        {
            public int AddedFired;
            public int RemovingFired;

            public IArray<Entity> Entity;
            public IArray<Component1> Component1;
            public IArray<Component2> Component2;

            public void EntityAdded(Entity entity)
            {
                ++AddedFired;
            }

            public void EntityRemoving(Entity entity)
            {
                ++RemovingFired;
            }
        }

        private class TestEntity : EntityIterator
        {
            public IValue<Entity> Entity;
            public IValue<Component1> Component1;
            public IValue<Component2> Component2;
        }
    }
}