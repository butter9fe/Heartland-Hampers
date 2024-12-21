using UnityEngine;
using System;

namespace DesignPatterns
{
    // Custom abstract class for creating singletons
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Type t = typeof(T);

                    instance = (T)FindObjectOfType(t);
                    if (instance == null)
                    {
                        Debug.LogError(t + " not attached to any GameObject!");
                    }
                }

                return instance;
            }
        }

        virtual protected void Awake()
        {
            CheckInstance();
        }

        protected bool CheckInstance()
        {
            if (instance == null)
            {
                instance = this as T;
                return true;
            }
            else if (Instance == this)
            {
                return true;
            }
            Destroy(this);
            return false;
        }
    }

    // Singleton except it persists through scenes
    public abstract class SingletonPersistent<T> : Singleton<T> where T : Singleton<T>
    {
        private static GameObject gameObjectInstance;
        void Start()
        {
            if (gameObjectInstance != null)
            {
                Destroy(gameObjectInstance);
            }
            gameObjectInstance = gameObject;
            DontDestroyOnLoad(this);
        }
    }
}
