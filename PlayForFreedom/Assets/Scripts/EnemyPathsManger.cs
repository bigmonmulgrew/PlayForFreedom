using UnityEngine;
using UnityEngine.Splines;

public class EnemyPathsManger : MonoBehaviour
{
    [SerializeField] SplineContainer roomOrbitSplines;
    [SerializeField] SplineContainer leaveRoomSplines;

    public SplineContainer RoomOrbitSplines => roomOrbitSplines;
    public SplineContainer LeaveRoomSplines => leaveRoomSplines;
}
