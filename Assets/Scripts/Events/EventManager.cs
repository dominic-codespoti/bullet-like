using System;
using System.Collections.Generic;
using BulletLike.Events;

public class EventManager
{
    private const int GlobalId = int.MinValue;

    private static readonly Dictionary<Type, Dictionary<int, List<Delegate>>> _eventListeners = new();

    public static void Subscribe<T>(Action<T> listener, int? id = null) where T : BaseEvent
    {
        var eventType = typeof(T);
        var actualId = id ?? GlobalId;

        if (!_eventListeners.ContainsKey(eventType))
        {
            _eventListeners[eventType] = new Dictionary<int, List<Delegate>>();
        }

        if (!_eventListeners[eventType].ContainsKey(actualId))
        {
            _eventListeners[eventType][actualId] = new List<Delegate>();
        }

        _eventListeners[eventType][actualId].Add(listener);
    }

    public static void Unsubscribe<T>(Action<T> listener, int? id = null) where T : BaseEvent
    {
        var eventType = typeof(T);
        var actualId = id ?? GlobalId;

        if (_eventListeners.ContainsKey(eventType) &&
            _eventListeners[eventType].ContainsKey(actualId))
        {
            _eventListeners[eventType][actualId].Remove(listener);

            if (_eventListeners[eventType][actualId].Count == 0)
            {
                _eventListeners[eventType].Remove(actualId);
            }

            if (_eventListeners[eventType].Count == 0)
            {
                _eventListeners.Remove(eventType);
            }
        }
    }

    public static void Publish<T>(T eventToPublish, int? id = null) where T : BaseEvent
    {
        var eventType = typeof(T);
        var actualId = id ?? GlobalId;

        if (_eventListeners.ContainsKey(eventType) && _eventListeners[eventType].ContainsKey(actualId))
        {
            foreach (var listener in _eventListeners[eventType][actualId])
            {
                (listener as Action<T>)?.Invoke(eventToPublish);
            }
        }

        if (id != GlobalId &&
            _eventListeners.ContainsKey(eventType) &&
            _eventListeners[eventType].ContainsKey(GlobalId))
        {
            foreach (var listener in _eventListeners[eventType][GlobalId])
            {
                (listener as Action<T>)?.Invoke(eventToPublish);
            }
        }
    }
}
