using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool isExitDoor = false;
    [SerializeField] GameObject playerBarrier;
    [SerializeField] MeshRenderer doorMesh;
    [SerializeField] bool startsLocked = false;

    #region Cahced References
    Room parentRoom;
    #endregion

    #region Runtime variables
    bool isOpen = false;
    bool islocked = false;
    #endregion

    private void Awake()
    {
        islocked = startsLocked;
        Open();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isOpen) return;

        if(other.gameObject.CompareTag("Player"))
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

        playerBarrier.SetActive(false);
        doorMesh.enabled = false;
        isOpen = true;
    }

    public void Close()
    {
        playerBarrier.SetActive(true);
        doorMesh.enabled = true;
        isOpen = false;
    }

    public void LockDoor()
    {
        islocked = true;
        playerBarrier.SetActive(true);
        doorMesh.enabled = true;
        isOpen = false;
    }

    public void SetParentRoom(Room parent)
    {
        parentRoom = parent;
    }

}
