using System.Collections.Generic;

namespace EventBusSystem
{
    /// <summary>
    /// SubscribersList maintains a list of subscribers with safe iteration and cleanup functionality.
    /// </summary>
    internal class SubscribersList<TSubscriber> where TSubscriber : class
    {
        public readonly List<TSubscriber> List = new();

        private bool _needsCleanUp = false;
        private bool _executing = false;
        
        /// <summary>
        /// Indicates whether the list is currently being iterated.
        /// </summary>
        public bool Executing
        {
            get => _executing;
            set => _executing = value;
        }

        /// <summary>
        /// Adds a subscriber to the list.
        /// </summary>
        public void Add(TSubscriber subscriber)
        {
            List.Add(subscriber);
        }
        
        /// <summary>
        /// Removes a subscriber from the list or marks it for cleanup if iterating.
        /// </summary>
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

        /// <summary>
        /// Removes all null entries from the list after iteration.
        /// </summary>
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