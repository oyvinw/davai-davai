using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Planning,
    Execute,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{
    private int LastLvl;
    public static GameManager Instance;
    public SquadController squad;
    public EnemyHiveMind hiveMind;
    UIController UI;
    private GameState _state;
    public GameState State
    {
        get => _state;
        set
        {
            _state = value;
            switch (value)
            {
                case GameState.Execute:
                    UI.SetTitleText("DAVAI PHASE");
                    break;
                case GameState.Planning:
                    UI.SetTitleText("PLANNING PHASE");
                    break;
                case GameState.Lose:
                    UI.Lost();
                    break;
                case GameState.Win:
                    UI.Won(LastLvl == SceneManager.GetActiveScene().buildIndex + 1);
                    break;
            }
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LastLvl = SceneManager.sceneCountInBuildSettings;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (SceneManager.GetActiveScene().name != "main_menu")
        {
            Instance.InitLevel();
            Instance.State = GameState.Planning;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
        if (SceneManager.GetActiveScene().name != "main_menu")
        {
            InitLevel();
            State = GameState.Planning;
        }
    }

    private void InitLevel()
    {
        squad = FindObjectOfType<SquadController>();
        hiveMind = FindObjectOfType<EnemyHiveMind>();
        UI = FindObjectOfType<UIController>();
    }

    public void Execute()
    {
        if (State == GameState.Planning)
        {
            State = GameState.Execute;
            hiveMind.ExecuteAllEnemies();
            squad.ExecuteAllSquaddieOrders();
        }
    }

    public void ExecuteBtnClick()
    {
        Instance.Execute();
    }

    public void ResetState()
    {
        squad.ResetAllSquaddies();
    }

    public void ClearBtnClick()
    {
        Instance.Clear();
    }

    public void Clear()
    {
        if (State == GameState.Planning)
        {
            squad.ClearAllSquaddieOrders();
        }
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMenu()
    {
        Instance.State = GameState.Menu;
        SceneManager.LoadScene("main_menu");
    }

    public void LoadScene(string sceneName, float time = 0)
    {
        StartCoroutine(LoadLevelAfterDelay(sceneName, 2));
    }

    IEnumerator LoadLevelAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    public void Exitgame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
