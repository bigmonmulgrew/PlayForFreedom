using UnityEngine;

public class ArenaCamera : MonoBehaviour
{
    public static ArenaCamera Instance;
    [SerializeField] float transitionTime = 2.0f;

    Transform targetTransform;
    float startTime;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        SmoothTransition();
    }

    private void SmoothTransition()
    {
        if (targetTransform == null) return;

        float interpolatioRatio = (Time.time - startTime) / transitionTime;

        transform.position = Vector3.Lerp(transform.position, targetTransform.position, interpolatioRatio);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, interpolatioRatio);

        if (interpolatioRatio >= 1) targetTransform = null; 
    }

    public void SetNewTransfrom(Transform newTransform)
    {
        targetTransform = newTransform;
        startTime = Time.time;
    }
}
