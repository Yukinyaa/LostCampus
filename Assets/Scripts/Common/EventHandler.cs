using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour
{
    public UnityEvent<EventHandler, Collider> OnTriggerEnterEvent = new UnityEvent<EventHandler, Collider>();
    public UnityEvent<EventHandler, Collider> OnTriggerEvent = new UnityEvent<EventHandler, Collider>();
    public UnityEvent<EventHandler, Collider> OnTriggerExitEvent = new UnityEvent<EventHandler, Collider>();
    public UnityEvent<EventHandler, Collision> OnCollisionEnterEvent = new UnityEvent<EventHandler, Collision>();
    public UnityEvent<EventHandler, Collision> OnCollisionEvent = new UnityEvent<EventHandler, Collision>();
    public UnityEvent<EventHandler, Collision> OnCollisionExitEvent = new UnityEvent<EventHandler, Collision>();
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent.Invoke(this, other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEvent.Invoke(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent.Invoke(this, other);
    }

    private void OnCollisionEnter(Collision other)
    {
        OnCollisionEnterEvent.Invoke(this, other);
    }
    
    private void OnCollisionStay(Collision other)
    {
        OnCollisionEvent.Invoke(this, other);
    }

    private void OnCollisionExit(Collision other)
    {
        OnCollisionExitEvent.Invoke(this, other);
    }
}
