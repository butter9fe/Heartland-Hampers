using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    namespace DesignPatterns
    {
        /// <summary>
        /// Observer (ish???) design pattern
        /// Useful for mass calling a set function, such as resetting
        /// Only problem is that you cannot control the order of stuff. But thats okay!
        /// </summary>
        public class Observer<T> : Singleton<Observer<T>>
        {
            // Subscribers
            // Just call a for loop calling all the individual functions and stuffsies
            protected List<T> _subscribers;

            override protected void Awake()
            {
                base.Awake();

                // On Awake, make sure that it is intialized
                _subscribers = new List<T>();
            }

            public void OnDestroy()
            {
                _subscribers.Clear();
            }

            // Add an object as a subscriber to the list -> it will get notified when an event happens
            public void AddSubscriber(GameObject GO)
            {
                T newComponent = GO.GetComponent<T>();
                if (newComponent != null)
                {
                    if (!_subscribers.Contains(GO.GetComponent<T>()))
                    {
                        _subscribers.Add(GO.GetComponent<T>());

                    }
                }
                else
                {
                    // Type does not exist in the given object, reject
                    Debug.LogError("The gameobject you are trying to add does not contain" + typeof(T).FullName + ".");
                }
            }

            // Remove an object as a subscriber -> It won`t get notified anymore
            public void RemoveSubscriber(GameObject GO)
            {
                if (_subscribers.Remove(GO.GetComponent<T>()) == false)
                {
                    Debug.Log("The object you are trying to remove does not exist as a subscriber.");
                }
            }
        }
    }
}
