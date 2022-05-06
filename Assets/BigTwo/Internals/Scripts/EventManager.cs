using System;
using System.Collections.Generic;

namespace BigTwo
{
    public interface EventListenerBase
    {

    }

    public interface EventListener<T> : EventListenerBase
    {
        void OnEventInvoked(T eventType);
    }

    public static class EventManager
    {
        private static Dictionary<Type, List<EventListenerBase>> dictionaryOfEvent;

        static EventManager()
        {
            dictionaryOfEvent = new Dictionary<Type, List<EventListenerBase>>();
        }

        public static void AddListener<T>(EventListener<T> subscriber) where T : struct
        {
            Type eventType = typeof(T);

            if (!dictionaryOfEvent.ContainsKey(eventType))
                dictionaryOfEvent[eventType] = new List<EventListenerBase>();

            if (!SubscriptionExist(eventType, subscriber))
                dictionaryOfEvent[eventType].Add(subscriber);
        }

        public static void RemoveListener<T>(EventListener<T> subscriber) where T : struct
        {
            Type eventType = typeof(T);

            if (dictionaryOfEvent.TryGetValue(eventType, out var subscribersForCurrentEvent))
            {
                for (int i = 0; i < subscribersForCurrentEvent.Count; i++)
                {
                    if (subscribersForCurrentEvent[i] == subscriber)
                    {
                        subscribersForCurrentEvent.RemoveAt(i);

                        if (subscribersForCurrentEvent.Count <= 0)
                            dictionaryOfEvent.Remove(eventType);

                        return;
                    }
                }
            }
        }

        public static void Invoke<T>(T invokedEvent) where T : struct
        {
            Type eventType = typeof(T);
            if (dictionaryOfEvent.TryGetValue(eventType, out var subscribersForCurrentEvent))
            {
                for (int i = 0; i < subscribersForCurrentEvent.Count; i++)
                {
                    var subscriber = subscribersForCurrentEvent[i] as EventListener<T>;
                    subscriber.OnEventInvoked(invokedEvent);
                }
            }
        }

        private static bool SubscriptionExist(Type eventType, EventListenerBase subscriber)
        {
            if (dictionaryOfEvent.TryGetValue(eventType, out var subscribersForCurrentEvent))
            {
                for (int i = 0; i < subscribersForCurrentEvent.Count; i++)
                {
                    if (subscribersForCurrentEvent[i] == subscriber)
                        return true;
                }
            }

            return false;
        }
    }
}
