using HSVPicker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] public Text scoreText; 
    [SerializeField] private Text scoreTextShadow;
    [SerializeField] private Text movesText; 
    [SerializeField] private Text movesTextShadow;
    [SerializeField] private InputField gridSizeX;
    [SerializeField] private InputField gridSizeY;
    [SerializeField] private InputField colorCountInput;
    [SerializeField] private GameObject colorsGrid;
    [SerializeField] private GameObject colorButton;
    [SerializeField] public ColorPicker colorPicker;
    [SerializeField] public Text scoreIncrementText;

    private static UIManager instance;
    public static UIManager Instance
    {
        get => instance;
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
    }

    void Start()
    {
        if (colorPicker)
        {
            colorPicker.gameObject.SetActive(false);
        }
        if (settingsPanel)
        {
            scoreIncrementText.transform.SetAsLastSibling();
            settingsPanel.transform.SetAsLastSibling();
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if(SceneManager.GetSceneByBuildIndex(level).name == "Level_1")
        {
            scoreText.text = "0";
            scoreTextShadow.text = "0";

            movesText.text = "0";
            movesTextShadow.text = "0";

            GridManager.Instance.onScoreChanged.AddListener(OnScoreChanged);
            GridManager.Instance.onMovesCountChanged.AddListener(increment => {
                movesText.text = (int.Parse(movesText.text) + increment).ToString();
                movesTextShadow.text = (int.Parse(movesTextShadow.text) + increment).ToString();
            });
        }
    }

    public void SetScore(int newScore)
    {
        scoreText.text = newScore.ToString();
        scoreTextShadow.text = newScore.ToString();
    }

    public void SetMovesCount(int newMovesCount)
    {
        movesText.text = newMovesCount.ToString();
        movesTextShadow.text = newMovesCount.ToString();
    }

    public void ToggleSettingsPanelVisibility(bool enable)
    {
        settingsPanel.SetActive(enable);
    }

    public void ToggleSettingsPanelVisibility(bool enable, bool isGameOver)
    {
        gameOverText.SetActive(isGameOver);
        settingsPanel.SetActive(enable);
    }



    private IEnumerator UpdateColorsGridHeight()
    {
        yield return new WaitForSeconds(0.025f);
        int colorCount = colorsGrid.transform.childCount;
        RectTransform rect = colorsGrid.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, Mathf.Ceil((float)colorCount / 5) * 150 + Mathf.Ceil((float)colorCount / 5 / 2) * 25 + ((colorCount > 0) ? 100 : 0));
    }

    public void LoadColorsGrid()
    {
        foreach (Color color in GridManager.Instance.hexagonColors)
        {
            GameObject temp = Instantiate(colorButton, colorsGrid.transform);
            temp.GetComponent<Image>().color = color;
        }

        colorCountInput.text = GridManager.Instance.hexagonColors.Count.ToString();
        StartCoroutine(UpdateColorsGridHeight());
    }

    public void LoadGridSize()
    {
        gridSizeX.text = GridManager.Instance.gridSizeX.ToString();
        gridSizeY.text = GridManager.Instance.gridSizeY.ToString();
    }

    public void ModifyColorsGrid()
    {
        int previousColorsCount = colorsGrid.transform.childCount;
        int newColorsCount = int.Parse(colorCountInput.text);

        int diff = newColorsCount - previousColorsCount;

        if(diff < 0)
        {
            RemoveExtraColorsFromGrid(diff * -1);
        }
        else if(diff > 0)
        {
            AddColorsToGrid(diff);
        }

        StartCoroutine(UpdateColorsGridHeight());
    }

    private void RemoveExtraColorsFromGrid(int count)
    {
        int lastIndex = colorsGrid.transform.childCount - 1;
        for(int i = 0; i < count; i++)
        {
            Destroy(colorsGrid.transform.GetChild(lastIndex - i).gameObject);
        }
    }

    private void AddColorsToGrid(int count)
    {
        for(int i = 0; i < count; i++)
        {
            Instantiate(colorButton, colorsGrid.transform);
        }
    }

    public void ToggleColorPickerVisibility(bool enable, Color defaultColor)
    {
        colorPicker.AssignColor(ColorValues.R, defaultColor.r);
        colorPicker.AssignColor(ColorValues.G, defaultColor.g);
        colorPicker.AssignColor(ColorValues.B, defaultColor.b);
        colorPicker.AssignColor(ColorValues.A, defaultColor.a);
        
        colorPicker.ToggleVisibility(enable);

        colorPicker.GetComponentInChildren<SVBoxSlider>().RegenerateSVTexture();
        colorPicker.GetComponentInChildren<SVBoxSlider>().slider.UpdateVisuals();
    }

    public void UpdateGameplayVariables()
    {
        UpdateHexagonColors();
        UpdateGridSize();

        GameManager.Instance.OnButtonPressed(2);
    }

    private void UpdateGridSize()
    {
        GridManager.Instance.gridSizeX = int.Parse(gridSizeX.text);
        GridManager.Instance.gridSizeY = int.Parse(gridSizeY.text);
    }

    public void UpdateHexagonColors()
    {
        GridManager.Instance.hexagonColors.Clear();
        for(int i = 0; i < colorsGrid.transform.childCount; i++)
        {
            GridManager.Instance.hexagonColors.Add(colorsGrid.transform.GetChild(i).GetComponent<Image>().color);
        }
    }

    public void OnScoreChanged(Vector3 startPos, int increment)
    {
        scoreIncrementText.text = "+" + increment;
        scoreIncrementText.gameObject.transform.position = startPos;
        StartCoroutine(MoveScoreIncrementTextWrapper(increment));
    }

    private IEnumerator MoveScoreIncrementTextWrapper(int increment)
    {
        yield return StartCoroutine(MoveScoreIncrementText(increment));
    }

    private IEnumerator MoveScoreIncrementText(int increment)
    {
        scoreIncrementText.gameObject.SetActive(true);
        while (Vector2.Distance(scoreIncrementText.transform.position, scoreText.transform.position) > 1f)
        {
            yield return new WaitForSeconds(0.025f);
            scoreIncrementText.transform.position = Vector2.Lerp(scoreIncrementText.transform.position, scoreText.transform.position, 0.1f);
        }
        scoreIncrementText.gameObject.SetActive(false);

        while(scoreText.rectTransform.localScale.x < 1.5f)
        {
            yield return new WaitForSeconds(0.025f);
            scoreText.rectTransform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
            scoreTextShadow.rectTransform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        }

        scoreText.text = (int.Parse(scoreText.text) + increment).ToString();
        scoreTextShadow.text = (int.Parse(scoreTextShadow.text) + increment).ToString();

        while (scoreText.rectTransform.localScale.x > 1f)
        {
            yield return new WaitForSeconds(0.05f);
            scoreText.rectTransform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
            scoreTextShadow.rectTransform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
        }

        scoreText.rectTransform.localScale = Vector3.one;
        scoreTextShadow.rectTransform.localScale = Vector3.one;
    }
}
