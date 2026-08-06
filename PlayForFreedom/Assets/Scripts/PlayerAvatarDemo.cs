using UnityEngine;

public class PlayerAvatarDemo : MonoBehaviour
{
    [SerializeField] GameObject defaultAvatar;
    [SerializeField] Transform avatarSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(defaultAvatar, avatarSpawn.position, avatarSpawn.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
