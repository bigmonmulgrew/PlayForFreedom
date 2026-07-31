using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(Collider))]
public class AutoLight : MonoBehaviour
{
    Light light;

    private void Awake()
    {
        light = GetComponent<Light>();
        TurnOff();
    }
    public void TurnOff()
    {
        light.enabled = false;
    }
    public void TurnOn()
    {
        light.enabled = true;
    }

    public void Toggle()
    {
        light.enabled = !light.enabled;
    }
}
