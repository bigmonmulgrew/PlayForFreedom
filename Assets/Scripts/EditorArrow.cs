using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorArrow : MonoBehaviour
{
    [SerializeField] float size = 0.8f;
    [SerializeField] Color colour = Color.cyan;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = colour;

        Handles.ArrowHandleCap(
                controlID: 0,
                position: transform.position,
                rotation: transform.rotation,
                size: size * HandleUtility.GetHandleSize(transform.position),
                eventType: EventType.Repaint
            );
    }
#endif

}
