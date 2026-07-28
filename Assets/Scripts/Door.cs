using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool isExitDoor = false;
    [SerializeField] GameObject playerBarrier;
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
        if (isExitDoor && other.gameObject.CompareTag("Player"))
        {
            LevelManager.LoadWinScreen();
        }
    }
}
