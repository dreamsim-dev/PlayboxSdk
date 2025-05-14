using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBusSystem
{
    internal static class EventBusHelper
    {
        private static Dictionary<Type, List<Type>> _cashedSubscriberTypes = new();

        public static List<Type> GetSubscriberTypes(IGlobalSubscriber globalSubscriber)
        {
            Type type = globalSubscriber.GetType();
            
            if (_cashedSubscriberTypes.ContainsKey(type))
            {
                return _cashedSubscriberTypes[type];
            }

            List<Type> subscriberTypes = type
                .GetInterfaces()
                .Where(t => t.GetInterfaces()
                    .Contains(typeof(IGlobalSubscriber)))
                .ToList();

            _cashedSubscriberTypes[type] = subscriberTypes;
            
            return subscriberTypes;
        }
    }
}