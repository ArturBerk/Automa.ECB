using System;
using System.Collections.Generic;
using System.Data;

namespace Automa.Context
{
    public class Context : IContext
    {
        private readonly HashSet<object> services = new HashSet<object>();
        private readonly List<IUpdateService> updateServices = new List<IUpdateService>();

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
            if (!services.Add(service)) return;
            if (service is IService serviceHandler)
            {
                serviceHandler.OnAttach(this);
            }
            if (service is IUpdateService updateService)
            {
                updateServices.Add(updateService);
            }
        }

        public void DetachService(object service)
        {
            if (!services.Remove(service)) return;
            if (service is IService serviceHandler)
            {
                serviceHandler.OnDetach(this);
            }
            if (service is IUpdateService updateService)
            {
                updateServices.Remove(updateService);
            }
        }

        public void Update()
        {
            for (var index = 0; index < updateServices.Count; index++)
            {
                var updateService = updateServices[index];
                updateService.UpdateService();
            }
        }
    }
}
