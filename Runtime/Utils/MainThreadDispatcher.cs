using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils {
    internal class MainThreadDispatcher : MonoBehaviour {
        private static readonly object _queueLock = new object();
        private static readonly List<Action> _queue = new List<Action>();
        private static MainThreadDispatcher _instance;

        internal static void Init() {
            if (_instance != null) {
                return;
            }

            GameObject go = new GameObject(nameof(MainThreadDispatcher));
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<MainThreadDispatcher>();
        }

        internal static void Enqueue(Action action) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }

            lock (_queueLock) {
                _queue.Add(action);
            }
        }

        private void Update() {
            lock (_queueLock) {
                foreach (Action action in _queue) {
                    action();
                }

                _queue.Clear();
            }
        }
    }
}
