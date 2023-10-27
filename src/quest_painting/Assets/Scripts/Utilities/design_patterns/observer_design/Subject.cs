using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Subject is a class which can be observed by others, by subscribing, in order to receive a notification and do something
/// when subject is in a changing stage
/// </summary>
public abstract class Subject : MonoBehaviour
{
    /// <summary>
    /// list of subscribed observers, which will be notified
    /// </summary>
    private readonly List<IObserver> _subscribers = new List<IObserver>();

    /// <summary>
    /// add a subscriber to list of the one which need to be notified
    /// </summary>
    /// <param name="toSubscribe">the observer which need to be subscribed</param>
    public void Subscribe(IObserver toSubscribe) => _subscribers.Add(toSubscribe);

    /// <summary>
    /// remove a subscriber of list, to avoid it to be notified
    /// </summary>
    /// <param name="toUnSubscribe">the observer which need to be unsubscribed</param>
    public void UnSubscribe(IObserver toUnSubscribe) => _subscribers.Remove(toUnSubscribe);

    protected void NotifySubscribers()
    {
        foreach (var subscriber in _subscribers)
            subscriber.Notify();
    }
}
