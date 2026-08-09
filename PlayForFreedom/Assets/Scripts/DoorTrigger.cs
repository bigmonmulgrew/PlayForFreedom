using System;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public event Action<Collider> OnTriggerEnterAction;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterAction?.Invoke(other);
    }
}
