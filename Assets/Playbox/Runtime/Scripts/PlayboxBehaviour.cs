using System;
using UnityEngine;

namespace Playbox
{
    public class PlayboxBehaviour : MonoBehaviour
    {
        public static PlayboxBehaviour AddToGameObject<T>(GameObject target, bool hasAdd = true) where T : PlayboxBehaviour
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (hasAdd)
            {
                return target.gameObject.AddComponent<T>();
            }
            else
            {
                return null;
            }
        }

        public virtual void Initialization()
        {
        }

        public virtual void Close()
        {
        }
    }
}