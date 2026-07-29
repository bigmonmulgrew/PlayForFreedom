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
        float interpolatioRation = (Time.time - startTime) / transitionTime;

        transform.position = Vector3.Lerp(transform.position, targetTransform.position, interpolatioRation);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, interpolatioRation);
    }

    public void SetNewTransfrom(Transform newTransform)
    {
        targetTransform = newTransform;
        startTime = Time.time;
    }
}
