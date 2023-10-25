using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaiterPaintCompleted : MonoBehaviour, IObserver
{
    public PaintedSubject subjectObserved;

    void Start()
    {
        subjectObserved.Subscribe(this);
    }

    public void Notify()
    {
        throw new System.NotImplementedException();
    }
}
