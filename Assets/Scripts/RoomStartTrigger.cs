using System;
using UnityEngine;

public class RoomStartTrigger : MonoBehaviour
{
    public event Action RoomStartTriggered;

    bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;
        RoomStartTriggered?.Invoke();
        isTriggered = true;
        Destroy(gameObject);
    }
}
