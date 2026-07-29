using UnityEngine;

/// <summary>
/// This is a mock up of the light that will eventuall follow the player. This will be deleted and replaced with player foolow light/replay camera later
/// It is here now to give a sense of motion.
/// </summary>
public class PlayerLightMockUp : MonoBehaviour
{
   
    void Update()
    {
        transform.Rotate(0, 10f * Time.deltaTime, 0);
    }
}
