using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton instance of the LevelManager class
    static LevelManager coroutineRunner;
    static bool activeRunner;

    const float LOADING_TIMEOUT = 30f;
    const string MAIN_MENU_SCENE_NAME = "MainMenu";
    const string OPTIONS_MENU_SCENE_NAME = "OptionsMene";
    const string BOOTSTRAP_SCENE_NAME = "Bootstrap";
    const string CHARACTER_SELECT_SCENE_NAME = "CharacterSelect";
    const string LEVEL_ONE_SCENE_NAME = "Level 1";
    const string COROUTINE_RUNNER_NAME = "Coroutine Runner";
    
    

    static Scene bootStrapScene;
    static Scene currentScene;

    static LevelManager CoroutineRunner
    {
        get
        {
            if (Instance != null)
            {
                //// if We have an instance, destroy any coroutine runners, the instance will be used instead.
                //if (coroutineRunner != null && !activeRunner)
                //{
                //    Destroy(coroutineRunner.gameObject);
                //    coroutineRunner = null;
                //}

                return Instance;
            }
                
            if (coroutineRunner != null) return coroutineRunner;

            GameObject go = new(COROUTINE_RUNNER_NAME);
            DontDestroyOnLoad(go);
            coroutineRunner = go.AddComponent<LevelManager>();
            return coroutineRunner;
        }
    }
        
    private void Awake()
    {
        if (Instance == null)
        {
            if (name == COROUTINE_RUNNER_NAME) return;    // If the object only exists to carry a coroutine, dont store the instance.
            
            Instance = this; // Assign the singleton instance
            StoreBootstrapReference();
        }
        else
        {
            Debug.LogWarning("Multiple LevelManager instances detected! Destroying duplicate.");
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
    }

    private void Start()
    {
        ForceLoadInEditor();
    }

#if UNITY_EDITOR
    static void ForceLoadInEditor()
    {
        if (SceneManager.sceneCount != 1) return;   // Only load when 1 scene eg bootstrap
        if (SceneManager.GetActiveScene().name != BOOTSTRAP_SCENE_NAME) return;


        Debug.Log($"Detected only bootstrap scene loaded, loading default level: {LEVEL_ONE_SCENE_NAME}, this will recreate bootstrap");
        CoroutineRunner.StartCoroutine(LoadNewLevelAsync(LEVEL_ONE_SCENE_NAME));
    }
#endif

    private void OnDistroy()
    {
        Instance = null;
        bootStrapScene = default;
    }

    void StoreBootstrapReference()
    {
        if (SceneManager.GetActiveScene().name == BOOTSTRAP_SCENE_NAME) bootStrapScene = SceneManager.GetActiveScene();
    }

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE_NAME);
    }
    public static void LoadCharacterSelect(bool recreateBootstrap = false)
    {
        CoroutineRunner.StartCoroutine(LoadNewLevelAsync(CHARACTER_SELECT_SCENE_NAME, true));
        if (ArenaCamera.Instance) ArenaCamera.Instance.SetCameraForMenu();
    }
    public static void LoadOptions()
    {
        SceneManager.LoadScene(OPTIONS_MENU_SCENE_NAME);
    }
    public static void LoadBootstrap()
    {
        SceneManager.LoadScene(BOOTSTRAP_SCENE_NAME);
        
    }
    public static void LoadFirstLevel(bool recreateBootstrap = false)
    {
        if (ArenaCamera.Instance) ArenaCamera.Instance.SetCameraForGameplay();


        MusicManager musicManager = FindAnyObjectByType<MusicManager>();
        //Check if the music manager is valid and that it doesnty have a game manager component attached
        if (musicManager != null && musicManager.GetComponent<GameManager>() == null)
        {
            Destroy(musicManager.gameObject);
            MusicManager.Instance = null; // Reset the singleton instance
        }

        CoroutineRunner.StartCoroutine(LoadNewLevelAsync(LEVEL_ONE_SCENE_NAME));
    }
    static IEnumerator LoadNewLevelAsync(string levelName, bool recreateBootstrap = false)
    {
        if (Instance == null) activeRunner = true;

        // First validate the existing bootstrap and reload if needed
        yield return ValidateBootstrap(recreateBootstrap);

        // Second unload the current scene if one is already loaded
        if (currentScene.IsValid()) SceneManager.UnloadSceneAsync(currentScene);
        
        // Third load the new scene
        yield return DoLevelLoad(levelName);

        if (Instance == null) activeRunner = false;

    }
    static IEnumerator ValidateBootstrap(bool recreateBootstrap = false)
    {

        float timeoutExitTime = Time.unscaledTime + LOADING_TIMEOUT;


        if (recreateBootstrap || !bootStrapScene.IsValid())
        {
            if (recreateBootstrap && bootStrapScene.IsValid()) SceneManager.UnloadSceneAsync(bootStrapScene);

            CloseAllScenes(); // If we are recreating bootstrap, also unload scenes to reset game state.

            SceneManager.LoadSceneAsync(BOOTSTRAP_SCENE_NAME);

            yield return null;
        }

        while (bootStrapScene.isLoaded == false && Time.unscaledTime < timeoutExitTime)
        {
            yield return new WaitForFixedUpdate();
        }
        if (Time.unscaledTime >= timeoutExitTime) Debug.LogError($"Timed out while loading scene: {bootStrapScene.name}");

        yield break;
    }
    static void CloseAllScenes()
    {
        int count = SceneManager.sceneCount;

        for(int i = 0; i < count; i++)
        {            
            int buildIndex = SceneManager.GetSceneAt(i).buildIndex;
            if (SceneManager.GetSceneByBuildIndex(buildIndex).name == BOOTSTRAP_SCENE_NAME) continue;
            SceneManager.UnloadSceneAsync(buildIndex);
        }
    }
    static IEnumerator DoLevelLoad(string levelName)
    {
        Debug.Log($"Current active scene before load async: {SceneManager.GetActiveScene().name}");
        currentScene = SceneManager.GetSceneByName(levelName);
        SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        Debug.Log($"Current active scene after load async: {SceneManager.GetActiveScene().name}");

        float timeoutExitTime = Time.unscaledTime + LOADING_TIMEOUT;

        while (currentScene.isLoaded == false && Time.unscaledTime < timeoutExitTime)
        {
            yield return new WaitForFixedUpdate();
        }
        if (Time.unscaledTime >= timeoutExitTime) Debug.LogError($"Timed out while loading scene: {currentScene.name}");

        yield break;
    }
    public static void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }   
    public static void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels to load. Returning to Main Menu.");
            LoadMainMenu();
        }
    }
    public static void LoadWinScreen()
    {
        SceneManager.LoadScene("WinScreen");
    }   
}
