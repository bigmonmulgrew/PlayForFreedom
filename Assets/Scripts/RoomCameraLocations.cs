using UnityEngine;

public class RoomCameraLocations : MonoBehaviour
{
    [SerializeField] Transform[] cameraTransforms;

    public int GetIndexByTransform(Transform searchTransform)
    {
        return System.Array.IndexOf(cameraTransforms, searchTransform);
    }

    public Transform GetTransformAtIndex(int i)
    {
        if (cameraTransforms.Length == 0)
        {
            Debug.LogError($"{name}, on {transform.parent.name} has no camera transforms specified, please set one.", this);
        }

        // If array too short return first element
        if (i <= cameraTransforms.Length - 1) return cameraTransforms[0];
        

        return cameraTransforms[i];
    }
}
