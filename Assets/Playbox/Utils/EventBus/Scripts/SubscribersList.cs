using System.Collections.Generic;

namespace EventBusSystem
{
    internal class SubscribersList<TSubscriber> where TSubscriber : class
    {
        public readonly List<TSubscriber> List = new();

        private bool _needsCleanUp = false;
        private bool _executing = false;
        
        public bool Executing
        {
            get => _executing;
            set => _executing = value;
        }

        public void Add(TSubscriber subscriber)
        {
            List.Add(subscriber);
        }

        public void Remove(TSubscriber subscriber)
        {
            if (Executing)
            {
                var index = List.IndexOf(subscriber);
                
                if (index >= 0)
                {
                    _needsCleanUp = true;
                    List[index] = null;
                }
            }
            else
            {
                List.Remove(subscriber);
            }
        }

        public void Cleanup()
        {
            if (!_needsCleanUp)
            {
                return;
            }

            List.RemoveAll(s => s == null);
            _needsCleanUp = false;
        }
    }
}