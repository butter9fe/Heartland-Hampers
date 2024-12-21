using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    namespace DesignPatterns
    {
        // A static instance is similar to a singleton, 
        // except instead of destroying any new instance it just overrides the current instance
        // Good for resetting state, saves you from doing it manually
        public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
        {
            public static T Instance { get; private set; }
            protected virtual void Awake() => Instance = this as T;

            protected virtual void OnApplicationQuit()
            {
                Instance = null;
                Destroy(gameObject);
            }
        }

        // Standard singleton
        // Toh da jun is creaming
        public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
        {
            protected override void Awake()
            {
                if (Instance != null)
                {
                    Destroy(gameObject);
                }
                else
                {
                    base.Awake();
                }
            }
        }

        // Singleton except its persistent between scenes 
        // This honestly sucks in code architecture, use very sparingly
        public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
        {
            protected override void Awake()
            {
                base.Awake();
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
