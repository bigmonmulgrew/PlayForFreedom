using BMD;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(MusicManager))]
[RequireComponent(typeof(LevelManager))]
public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    // TODO move this project wide.
    public const bool DEBUG_ENABLED = true;

    #region Configuration

    [Header("Canvase References")]
    [SerializeField] GameObject playCanvas;
    #endregion

    #region Cached References
    PlayerControls playerControls;
    InputAction winButton;
    InputAction loseButton;
    #endregion

    #region Runtime Variables
    //Level settings

    bool gameOver = false;
    bool gameWon = false;
    bool roundOver = false;
    #endregion

    #region Properties
    public bool GameIsOver { get => gameOver; }
    public bool GameWon { get => gameWon; }
    public bool RoundOver { get => roundOver; set => gameOver = value;  }
    #endregion

    private void Awake()
    {
        // Setup singleton instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep this instance across scenes

        // Subscrite to sceneLoaded once.
        SceneManager.sceneLoaded += OnSceneLoaded;

        

    }
    private void OnEnable()
    {
        if (DEBUG_ENABLED)
        {
            playerControls = new PlayerControls();
            playerControls.Debug.Enable();
            winButton = playerControls.Debug.DebugWin;
            loseButton = playerControls.Debug.DebugLose;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

    }

    void Update()
    {
        if(gameOver || gameWon) return;

        DebugInputs();
    }
    
    void DebugInputs()
    {
        if (winButton.WasPerformedThisFrame())  WinGame();
        if (loseButton.WasPerformedThisFrame()) GameOver();
    }
    private void WinGame()
    {
        gameWon = true;
        playCanvas.SetActive(false);

        LevelManager.LoadWinScreen();
    }
    
   
    void GameOver()
    {
        playCanvas.SetActive(false);
        LevelManager.LoadGameOver();
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RunPerSceneSetup();
    }
    private void RunPerSceneSetup()
    {

    }
    private void OnDestroy()
    {
        // Always unsubscribe to avoid memory leaks if destroyed manually
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
