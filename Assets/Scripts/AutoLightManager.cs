using UnityEngine;

public class AutoLightManager : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<AutoLight>(out AutoLight al)) al.TurnOn();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<AutoLight>(out AutoLight al)) al.TurnOff();
    }
}
