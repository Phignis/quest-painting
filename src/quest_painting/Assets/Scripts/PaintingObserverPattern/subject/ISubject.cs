using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour
{
    private readonly List<IObserver> _list = new List<IObserver>();

    public void Subscribe(IObserver toSubscribe) => _list.Add(toSubscribe);

    public void UnSubscribe(IObserver toSubscribe) => _list.Remove(toSubscribe);

    protected void NotifySubscribers()
    {
        foreach (var subscriber in _list)
            subscriber.Notify();
    }
}
