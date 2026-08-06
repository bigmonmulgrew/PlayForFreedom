using UnityEngine;

public class SessionPanelUI : MonoBehaviour
{
    public void StartGame()
    {
        LevelManager.LoadFirstLevel();
    }
}
