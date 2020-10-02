using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu]
public class SimEvent : ScriptableObject {
    private List<SimEventListener> listeners = new List<SimEventListener>();

    public void Raise() {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }

    public void RegisterListener(SimEventListener listener) {
        listeners.Add(listener);
    }

    public void UnregisterListener(SimEventListener listener) {
        listeners.Remove(listener);
    }
}
