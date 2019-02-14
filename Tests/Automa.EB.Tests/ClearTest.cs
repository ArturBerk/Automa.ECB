using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Automa.Entities;
using NUnit.Framework;

namespace Automa.EB.Tests
{
    [TestFixture]
    public class ClearTest
    {
        [Test]
        public void Test()
        {
            EntityGroup entityGroup = new EntityGroup();

            for (int i = 0; i < 10; i++)
            {
                new ChildEntity().Register(entityGroup);
            }
            Assert.IsTrue(entityGroup.Entities<ChildEntity>().Count == 10);
            Assert.IsTrue(entityGroup.Entities<RootEntity>().Count == 10);

            entityGroup.Clear();

            Assert.IsTrue(entityGroup.Entities<ChildEntity>().Count == 0);
            Assert.IsTrue(entityGroup.Entities<RootEntity>().Count == 0);
        }

        public class RootEntity : Entity<RootEntity>
        {
        }

        public class ChildEntity : RootEntity
        {
            public override void Register(IEntityGroup entityGroup)
            {
                base.Register(entityGroup);
                RegisterAs<ChildEntity>(entityGroup);
            }
        }

        public class Entity<TEntity> : IEntity<TEntity> where TEntity : class, IEntity<TEntity>
        {
            public IEntityReference<TEntity> Reference { get; set; }

            public bool IsRegistered => Reference != null;

            public virtual void Register(IEntityGroup entityGroup)
            {
                ((IEntityCollection)entityGroup.Entities<TEntity>()).Add(this);
                RegisterReferences(entityGroup);
            }

            public virtual void Unregister()
            {
                Reference?.Dispose();
            }

            protected virtual void RegisterReferences(IEntityGroup entityGroup)
            {
            }

            protected void RegisterReferenced<T>(IEntityGroup entityGroup, T value) where T : class
            {
                ((IEntityCollection)entityGroup.Entities<T>()).AddReferenced(this, value);
            }

            protected void RegisterAs<T>(IEntityGroup entityGroup) where T : class
            {
                ((IEntityCollection)entityGroup.Entities<T>()).AddReferenced(this, this);
            }
        }
    }
}
