using UnityEngine;

public class RoomCameraLocation : MonoBehaviour
{
    [SerializeField] RoomCameraLocation up;
    [SerializeField] RoomCameraLocation down;
    [SerializeField] RoomCameraLocation left;
    [SerializeField] RoomCameraLocation right;

    public RoomCameraLocation Up => up;
    public RoomCameraLocation Down => down;
    public RoomCameraLocation Left => left;
    public RoomCameraLocation Right => right;

    public void RotateLeft()
    {
        down = down.left;
        up = up.left;
    }

    public void RotateRight()
    {
        down = down.right;
        up = up.right;
    }
}
