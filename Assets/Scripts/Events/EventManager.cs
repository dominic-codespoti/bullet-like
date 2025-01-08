using System;
using System.Collections.Generic;

namespace Events
{
    public class EventManager
    {
        private const int GlobalId = int.MinValue;

        private static readonly Dictionary<Type, Dictionary<int, List<Delegate>>> EventListeners = new();

        public static void Subscribe<T>(Action<T> listener, int? id = null) where T : BaseEvent
        {
            var eventType = typeof(T);
            var actualId = id ?? GlobalId;

            if (!EventListeners.ContainsKey(eventType)) EventListeners[eventType] = new Dictionary<int, List<Delegate>>();

            if (!EventListeners[eventType].ContainsKey(actualId))
                EventListeners[eventType][actualId] = new List<Delegate>();

            EventListeners[eventType][actualId].Add(listener);
        }

        public static void Unsubscribe<T>(Action<T> listener, int? id = null) where T : BaseEvent
        {
            var eventType = typeof(T);
            var actualId = id ?? GlobalId;

            if (EventListeners.ContainsKey(eventType) &&
                EventListeners[eventType].ContainsKey(actualId))
            {
                EventListeners[eventType][actualId].Remove(listener);

                if (EventListeners[eventType][actualId].Count == 0) EventListeners[eventType].Remove(actualId);

                if (EventListeners[eventType].Count == 0) EventListeners.Remove(eventType);
            }
        }

        public static void Publish<T>(T eventToPublish, int? id = null) where T : BaseEvent
        {
            var eventType = typeof(T);
            var actualId = id ?? GlobalId;

            if (EventListeners.ContainsKey(eventType) && EventListeners[eventType].ContainsKey(actualId))
                foreach (var listener in EventListeners[eventType][actualId])
                    (listener as Action<T>)?.Invoke(eventToPublish);

            if (id != GlobalId &&
                EventListeners.ContainsKey(eventType) &&
                EventListeners[eventType].ContainsKey(GlobalId))
                foreach (var listener in EventListeners[eventType][GlobalId])
                    (listener as Action<T>)?.Invoke(eventToPublish);
        }
    }
}