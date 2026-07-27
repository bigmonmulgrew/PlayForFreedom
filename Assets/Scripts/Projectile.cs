using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Cached references
    Rigidbody rb;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void FireProjectile(Vector3 direction,  float strength)
    {
        rb.AddForce(strength * direction, ForceMode.Impulse);
    }
}
