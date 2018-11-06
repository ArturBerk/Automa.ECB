using System;
using System.Diagnostics;
using Automa.EntityComponents;
using Automa.EntityComponents.Behaviours;
using Automa.EntityComponents.Internal;

namespace Automa.ECB.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var Entities = new EntityManager();

            var type1 = EntityType.FromComponentTypes(
                ComponentType.Create<Component1>(),
                ComponentType.Create<Component2>());
            var type2 = EntityType.FromComponentTypes(
                ComponentType.Create<Component1>(),
                ComponentType.Create<Component2>(),
                ComponentType.Create<Component3>());
            var type3 = EntityType.FromComponentTypes(
                ComponentType.Create<Component1>(),
                ComponentType.Create<Component2>(),
                ComponentType.Create<Component4>());
            var type4 = EntityType.FromComponentTypes(
                ComponentType.Create<Component1>(),
                ComponentType.Create<Component2>(),
                ComponentType.Create<Component3>(),
                ComponentType.Create<Component4>());
            var entities = new Entity[20000];
            for (int i = 0; i < 5000; i++)
            {
                var e = Entities.CreateEntity(type1);
                e.SetComponent(new Component1());
                e.SetComponent(new Component2());
                entities[i] = e;
            }
            for (int i = 0; i < 5000; i++)
            {
                var e = Entities.CreateEntity(type2);
                e.SetComponent(new Component1());
                e.SetComponent(new Component2());
                entities[i + 5000] = e;
            }
            for (int i = 0; i < 5000; i++)
            {
                var e = Entities.CreateEntity(type3);
                e.SetComponent(new Component1());
                e.SetComponent(new Component2());
                entities[i + 10000] = e;
            }
            for (int i = 0; i < 5000; i++)
            {
                var e = Entities.CreateEntity(type4);
                e.SetComponent(new Component1());
                e.SetComponent(new Component2());
                entities[i + 15000] = e;
            }
            
            var entity2 = new Entity2();
            var group = new Group1();
            Entities.BindEnumerator(entity2);
            Entities.BindGroup(group);
            
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Restart();
            for (int i = 0; i < 10; i++)
            {
                while (entity2.MoveNext())
                {
                    ref var c1 = ref entity2.Component1.Value;
                    ref var c2 = ref entity2.Component2.Value;
                    ref var c3 = ref entity2.Component1.Value;
                    ref var c4 = ref entity2.Component2.Value;
                    c2.Value = c1.Value;
                }
                entity2.Reset();
            }
            stopwatch.Stop();
            Console.WriteLine("Value: " + stopwatch.ElapsedTicks);

            stopwatch.Restart();
            for (int i = 0; i < 10; i++)
            {
                var count = @group.Entity.EvaluateCount;
                for (int j = 0; j < count; j++)
                {
                    ref var c1 = ref group.Component1[j];
                    ref var c2 = ref group.Component2[j];
                    ref var c3 = ref group.Component1[j];
                    ref var c4 = ref group.Component2[j];
                    c2.Value = c1.Value;
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Group: " + stopwatch.ElapsedTicks);

            stopwatch.Restart();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 20000; j++)
                {
                    var e = entities[j];
                    ref var c1 = ref e.GetComponent<Component1>();
                    ref var c2 = ref e.GetComponent<Component2>();
                    ref var c3 = ref e.GetComponent<Component1>();
                    ref var c4 = ref e.GetComponent<Component2>();
                    c2.Value = c1.Value;
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Group (GetComponent): " + stopwatch.ElapsedTicks);
            Console.ReadKey();
        }


        private struct Component1
        {
            public int Value;
        }

        private struct Component2
        {
            public int Value;
        }

        private struct Component3
        {

        }

        private struct Component4
        {

        }

        private class Entity2 : EntityIterator
        {
            public IValue<Component1> Component1;
            public IValue<Component2> Component2;
        }

        private class Group1 : EntityGroup
        {
            public IArray<Component1> Component1;
            public IArray<Component2> Component2;
        }
    }
}
