using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool IsExitDoor = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsExitDoor && other.gameObject.CompareTag("Player"))
        {
            LevelManager.LoadWinScreen();
        }
    }
}
