using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool isExitDoor = false;
    [SerializeField] GameObject playerBarrier;
    [SerializeField] MeshRenderer doorMesh;
    [SerializeField] bool startsLocked = false;

    #region Cahced References
    Room parentRoom;
    //Collider playerBarrierCollider;
    DoorTrigger playerBarrierDoorTrigger;
    #endregion

    #region Runtime variables
    bool isOpen = false;
    bool islocked = false;
    #endregion

    private void Awake()
    {
        playerBarrierDoorTrigger = playerBarrier.GetComponent<DoorTrigger>();
        islocked = startsLocked;
        Open();
    }

    private void OnEnable()
    {
        if (playerBarrierDoorTrigger) playerBarrierDoorTrigger.OnTriggerEnterAction += ChildTriggerTriggered;
    }

    private void OnDisable()
    {
        if (playerBarrierDoorTrigger) playerBarrierDoorTrigger.OnTriggerEnterAction -= ChildTriggerTriggered;
    }

    void OnTriggerEnter(Collider other)
    {
        ChildTriggerTriggered(other);
    }

    void ChildTriggerTriggered(Collider other)
    {
        if (!isOpen) return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (parentRoom) parentRoom.LockOtherDoors(this);
            islocked = true;
            // TODO we need to then ignore collision from all players on the player barrier until they exit the trigger.
            // Need a separate object to check they have passed through fully
        }

        if (isExitDoor && other.gameObject.CompareTag("Player"))
        {
            LevelManager.LoadWinScreen();
        }
    }

    public void Open()
    {
        if (islocked || isOpen) return;

        //playerBarrier.SetActive(false);
        playerBarrier.GetComponent<Collider>().isTrigger = true;
        doorMesh.enabled = false;
        isOpen = true;
    }

    public void Close()
    {
        //playerBarrier.SetActive(true);
        playerBarrier.GetComponent<Collider>().isTrigger = false;
        doorMesh.enabled = true;
        isOpen = false;
    }

    public void LockDoor()
    {
        islocked = true;
        //playerBarrier.SetActive(true);

        playerBarrier.GetComponent<Collider>().isTrigger = false;
        doorMesh.enabled = true;
        isOpen = false;
    }

    public void SetParentRoom(Room parent)
    {
        parentRoom = parent;
    }

}
