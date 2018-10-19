using System.Collections.Generic;
using System.Data;

namespace Automa.Context
{
    public class Context : IContext
    {
        private readonly HashSet<object> services = new HashSet<object>();

        public T GetService<T>()
        {
            foreach (var service in services)
            {
                if (service is T typedService)
                {
                    return typedService;
                }
            }
            throw new ConstraintException($"Service not found: {typeof(T)}");
        }

        public void AttachService(object service)
        {
            services.Add(service);
            if (service is IService serviceHandler)
            {
                serviceHandler.OnAttach(this);
            }
        }

        public void DetachService(object service)
        {
            services.Remove(service);
            if (service is IService serviceHandler)
            {
                serviceHandler.OnDetach(this);
            }
        }
    }
}
