using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public List<SceneData> scenesData;

    private GridManager gridManager;

    private static GameManager instance;

    public static GameManager Instance
    {
        get => instance;
    }

    private int score;
    public int Score
    {
        get => score;
        set
        {
            score = value;
        }
    }

    private int movesCount;
    public int MovesCount
    {
        get => movesCount;
        set
        {
            movesCount = value;
        }
    }



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); 
        
        gridManager = gameObject.GetComponent<GridManager>();
        gridManager.enabled = false;
    }


    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name.Equals("Level_1"))
        {
            gridManager.canvas = FindObjectOfType<Canvas>();
            gridManager.enabled = true;
            gridManager.Init();
            Score = 0;
            MovesCount = 0;
            gridManager.onScoreChanged.AddListener((dummy, scoreIncrement) =>
            {
                Score += scoreIncrement;
            });
            gridManager.onMovesCountChanged.AddListener(increment => {
                MovesCount += increment;
            });
        }
        else if (SceneManager.GetActiveScene().name.Equals("Settings"))
        {
            UIManager.Instance.LoadColorsGrid();
            UIManager.Instance.LoadGridSize();
        }
    }

    public void OnButtonPressed(int buttonType)
    {
        switch ((ButtonType)buttonType)
        {
            case ButtonType.Quit:
                Application.Quit();
                break;
            default:
                SceneManager.LoadScene(scenesData[(int)buttonType].sceneName);
                break;
        }
    }
}

[System.Serializable]
public enum ButtonType
{

    Quit = -1,
    Play,
    Settings,
    MainMenu,
    Credits
}

[System.Serializable]
public struct SceneData
{
    public ButtonType buttonType;
    public string sceneName;
}