namespace SciFiKit
{
    using UnityEngine;

    [ExecuteAlways]
    public class SpinObject : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private float rotationSpeed = 100f;

        [Header("Editor Preview")]
        [SerializeField] private bool playInEditor = true;

        private void Update()
        {
            // In the editor, only spin if the preview is enabled.
            if (!Application.isPlaying && !playInEditor)
                return;

            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}