using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private String playSceneName = "Level_1";
    [SerializeField] private GameObject gridColumnsPrefab;
    [SerializeField] public GameObject hexagonContainerColumnPrefab;
    [SerializeField] public GameObject hexagonPrefab;
    [SerializeField] public GameObject rotatingHexagonsParentPrefab;
    [SerializeField] private GameObject explodeParticlePrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] public int gridSizeX = 8;
    [SerializeField] public int gridSizeY = 9;
    [SerializeField] private float gridOffsetFromEdges = 0f;
    [SerializeField] private int scoreIncrementPerBlockExploded = 5;
    [SerializeField] public List<Color> hexagonColors;

    private List<GridColumns> gridColumns;
    private float defaultHexWidth = 92;
    private float defaultHexHeight = 80;


    [HideInInspector] public Canvas canvas;
    [HideInInspector] public float hexWidth;
    [HideInInspector] public float hexHeight;

    private Dictionary<string, Hexagon> hexagons;
    public Hexagon selectedHexagon;
    public int selectedTripleIndex;
    private RotatingHexagonsParent rotatingHexagonsParent;

    //public bool isInputEnabled;

    Dictionary<List<Hexagon>, bool> allTriples;

    List<Hexagon> toDestroy;

    public UnityEvent<Vector3, int> onScoreChanged = new UnityEvent<Vector3, int>();
    public UnityEvent<int> onMovesCountChanged = new UnityEvent<int>();

    private int bombLastAppearedAtScore;

    private static GridManager instance;
    public static GridManager Instance
    {
        get => instance;
    }

    private int score;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        rotatingHexagonsParent = Instantiate(rotatingHexagonsParentPrefab, canvas.transform).GetComponent<RotatingHexagonsParent>();

        CalculateHexagonWidth();
        CreateGrid();

        selectedTripleIndex = -1;

        InputManager.Instance.isInputEnabled = true;
    }

    private void OnLevelWasLoaded(int level)
    {
        if(SceneManager.GetSceneByBuildIndex(level).name != "Level_1")
        {
            onScoreChanged.RemoveAllListeners();
            onMovesCountChanged.RemoveAllListeners();
            score = 0;
        }
    }

    public void TryRotate(RotateType rotateType)
    {
        if (selectedHexagon != null)
        {
            Rotate(rotateType);
        }
    }

    private void Update()
    {
        Debug.LogError(score);
    }

    private void Rotate(RotateType rotateType)
    {
        foreach (Hexagon hexagon in selectedHexagon.neighbourTriples[selectedTripleIndex])
        {
            hexagon.ToggleSelectedOverlayVisibility(false);
            hexagon.hexagonImage.enabled = false;
        }
        ToggleRotatingHexagonsVisibility(true);
        StartCoroutine(RotateRoutine(rotateType));
    }

    private IEnumerator RotateRoutine(RotateType rotateType)
    {
        InputManager.Instance.isInputEnabled = false;
        onMovesCountChanged.Invoke(1);

        bool explodeExist = false;
        toDestroy = new List<Hexagon>();

        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(0.05f);
                rotatingHexagonsParent.transform.Rotate(0, 0, (int)rotateType * 12);
            }

            for (int i = 0; i < 3; i++)
            {
                int temp = rotateType == RotateType.Right ? (i - j - 1) : (i + j + 1);
                temp += temp < 0 ? 3 : 0;
                temp %= 3;

                selectedHexagon.neighbourTriples[selectedTripleIndex][i].colorProp = rotatingHexagonsParent.transform.GetChild(temp).GetComponent<Hexagon>().colorProp;
            }

            explodeExist = DoesExplodeExist();

            if (explodeExist)
            {
                break;
            }
        }

        while (explodeExist)
        {
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(ExplodeRoutinesWrapper());

            ToggleRotatingHexagonsVisibility(false);


            int iterCount = toDestroy.Count;

            for (int i = 0; i < iterCount; i++)
            {
                if(toDestroy[i] != null)
                {
                    Destroy(toDestroy[i].gameObject);
                }
            }

            explodeExist = DoesExplodeExist();
            //DeselectHexagon();

        }

        for (int i = 0; i < 3; i++)
        {
            selectedHexagon.neighbourTriples[selectedTripleIndex][i].ToggleSelectedOverlayVisibility(true);
            selectedHexagon.neighbourTriples[selectedTripleIndex][i].hexagonImage.enabled = true;

            rotatingHexagonsParent.GetHexagon(selectedHexagon.neighbourTriples[selectedTripleIndex][i]).GetComponent<Hexagon>().colorProp = selectedHexagon.neighbourTriples[selectedTripleIndex][i].colorProp;

        }

        int selectedTripleIndexTemp = selectedTripleIndex;
        Hexagon selectedHexagonTemp = selectedHexagon;
        DeselectHexagon();

        InputManager.Instance.isInputEnabled = true;
        selectedHexagonTemp.SelectHexagon(selectedTripleIndexTemp);

    }

    private Dictionary<Hexagon, int> GetSeperateColumnHexagons(List<Hexagon> triple)
    {
        Dictionary<Hexagon, int> seperateColumnHexagons = new Dictionary<Hexagon, int>();

        Dictionary<HexagonContainerColumn, List<Hexagon>> keyValuePairs = new Dictionary<HexagonContainerColumn, List<Hexagon>>();

        foreach (Hexagon hexagon in triple)
        {
            if (!keyValuePairs.ContainsKey(hexagon.transform.GetComponentInParent<HexagonContainerColumn>()))
            {
                keyValuePairs.Add(hexagon.transform.GetComponentInParent<HexagonContainerColumn>(), new List<Hexagon>());
            }
            keyValuePairs[hexagon.transform.GetComponentInParent<HexagonContainerColumn>()].Add(hexagon);
        }

        foreach (KeyValuePair<HexagonContainerColumn, List<Hexagon>> keyValuePair in keyValuePairs)
        {
            if(keyValuePair.Value.Count == 2)
            {
                seperateColumnHexagons.Add(keyValuePair.Value[0].coordinates.y > keyValuePair.Value[1].coordinates.y ? keyValuePair.Value[0] : keyValuePair.Value[1], 2);
            }
            else
            {
                seperateColumnHexagons.Add(keyValuePair.Value[0], 1);
            }
        }

        return seperateColumnHexagons;
    }

    private IEnumerator ScrollColumnColorsInParallel(List<Hexagon> triple)
    {
        List<CoroutineHelper> scrollColorColumnRoutineHelpers = new List<CoroutineHelper>();
        Dictionary<Hexagon, int> seperateColumnHexagons = GetSeperateColumnHexagons(triple);

        ToggleRotatingHexagonsVisibility(false);
/*
        foreach(KeyValuePair<Hexagon, int> kvp in seperateColumnHexagons)
        {
            HexagonContainerColumn tempHexContainer = kvp.Key.transform.GetComponentInParent<HexagonContainerColumn>();
            tempHexContainer.ToggleVisibilityOfChildrenBetweenIndices(0, tempHexContainer.hexagons.IndexOf(kvp.Key) + 1, false);
        }
        */
        foreach (KeyValuePair<Hexagon, int> kvp in seperateColumnHexagons)
        {
            CoroutineHelper tempCoroutineHelper = new CoroutineHelper(null, CoroutineHelper.CoroutineState.Idle);

            Coroutine tempCoroutine = StartCoroutine(ScrollColorColumnRoutine(kvp, tempCoroutineHelper));

            tempCoroutineHelper.coroutine = tempCoroutine;
            scrollColorColumnRoutineHelpers.Add(tempCoroutineHelper);
        }

        do
        {
            yield return new WaitForSeconds(0.02f);
        } while (scrollColorColumnRoutineHelpers.Where(coroutineHelper => coroutineHelper.state != CoroutineHelper.CoroutineState.Finished).ToList().Count != 0);


        foreach (KeyValuePair<Hexagon, int> kvp in seperateColumnHexagons)
        {
            /*for (int i = 0; i < kvp.Value; i++)
            {
                GetHexagon(kvp.Key.coordinates.x, i).colorProp = Color.black;
            }*/

            HexagonContainerColumn tempHexContainer = kvp.Key.transform.GetComponentInParent<HexagonContainerColumn>();
            tempHexContainer.ToggleVisibilityOfChildrenBetweenIndices(0, tempHexContainer.hexagons.IndexOf(kvp.Key) + 1, true);
        }
    }


    private IEnumerator ScrollColorColumnRoutine(KeyValuePair<Hexagon, int> kvp, CoroutineHelper tempRoutineHelperOuter)
    {
        tempRoutineHelperOuter.state = CoroutineHelper.CoroutineState.Running;

        List<CoroutineHelper> scrollColorHexagonRoutineHelpers = new List<CoroutineHelper>();

        for(int i = kvp.Key.coordinates.y; i >= kvp.Value; i--)
        {
            CoroutineHelper tempCoroutineHelper = new CoroutineHelper(null, CoroutineHelper.CoroutineState.Idle);
            Coroutine tempCoroutine = StartCoroutine(ScrollColorHexagonRoutine(GetHexagon(kvp.Key.coordinates.x, i), GetHexagon(kvp.Key.coordinates.x, i - kvp.Value), kvp.Value, tempCoroutineHelper));
            
            tempCoroutineHelper.coroutine = tempCoroutine;
            scrollColorHexagonRoutineHelpers.Add(tempCoroutineHelper);

            yield return new WaitForSeconds(0.1f);
        }

        for(int i = kvp.Value - 1; i > -1 ; i--)
        {
            CoroutineHelper tempCoroutineHelper = new CoroutineHelper(null, CoroutineHelper.CoroutineState.Idle);

            Hexagon hexagon = Instantiate(hexagonPrefab, GetHexagon(kvp.Key.coordinates.x, 0).transform.position + Vector3.up, Quaternion.identity, canvas.transform).GetComponent<Hexagon>();
            hexagon.Init(new Vector2Int(0, 0), Color.black);
            hexagon.GetComponent<Image>().enabled = false;

            Coroutine tempCoroutine = StartCoroutine(ScrollColorHexagonRoutine(GetHexagon(kvp.Key.coordinates.x, i), hexagon, kvp.Value, tempCoroutineHelper));

            tempCoroutineHelper.coroutine = tempCoroutine;
            scrollColorHexagonRoutineHelpers.Add(tempCoroutineHelper);

            Destroy(hexagon.gameObject);
            yield return new WaitForSeconds(0.1f);
        }

        do
        {
            yield return new WaitForSeconds(0.02f);
        } while (scrollColorHexagonRoutineHelpers.Where(coroutineHelper => coroutineHelper.state != CoroutineHelper.CoroutineState.Finished).ToList().Count != 0);

        tempRoutineHelperOuter.state = CoroutineHelper.CoroutineState.Finished;
    }

    private IEnumerator ScrollColorHexagonRoutine(Hexagon hexagonLower, Hexagon hexagonUpper, int explodedHexCountInColumn, CoroutineHelper coroutineHelper)
    {
        coroutineHelper.state = CoroutineHelper.CoroutineState.Running;
        Hexagon temp = Instantiate(hexagonUpper.gameObject, hexagonUpper.transform.position, Quaternion.identity, canvas.transform).GetComponent<Hexagon>();
        /*if (hexagonUpper.bomb != null)
        {
            hexagonUpper.bomb.transform.SetParent(temp.transform, false);
        }*/
        toDestroy.Add(temp);
        hexagonUpper.hexagonImage.enabled = false;

        temp.GetComponent<Image>().enabled = true;
        temp.GetComponent<RectTransform>().sizeDelta = gridColumns[0].hexagonContainerColumns[0].GetComponent<GridLayoutGroup>().cellSize;

        while (Vector2.Distance(hexagonLower.transform.position, temp.transform.position) > 0.05f)
        {
            yield return new WaitForSeconds(0.015f);
            temp.transform.position = Vector3.Lerp(temp.transform.position, hexagonLower.transform.position, 0.1f * explodedHexCountInColumn);
            if(hexagonUpper.bomb != null)
            {
                hexagonUpper.bomb.transform.position = temp.transform.position;
            }
        }
        temp.transform.position = hexagonLower.transform.position;
        if(hexagonUpper.bomb != null)
        {
            hexagonUpper.bomb.transform.position = hexagonLower.transform.position;
        }
        hexagonLower.colorProp = hexagonUpper.colorProp;
        /*if (temp.bomb != null)
        {
            temp.bomb.transform.SetParent(hexagonLower.transform, false);
        }*/
        coroutineHelper.state = CoroutineHelper.CoroutineState.Finished;
    }

    private IEnumerator ExplodeRoutinesWrapper()
    {
        List<List<Hexagon>> shouldExplodeTriples = allTriples.Keys.Where(key => allTriples[key] == true).ToList();

        Vector3 tempPos = Vector3.zero;

        foreach (Hexagon hexagon in shouldExplodeTriples[0])
        {
            tempPos += hexagon.transform.position;
        }

        tempPos /= 3;

        int incrementScore = 3 * scoreIncrementPerBlockExploded;
        score += incrementScore;

        if(score >= (bombLastAppearedAtScore + 1000))
        {
            int randomX = UnityEngine.Random.Range(0, gridSizeX);
            int randomY = UnityEngine.Random.Range(0, gridSizeY);
            Hexagon hex = GetHexagon(randomX, randomY);
            Bomb temp = Instantiate(bombPrefab, hex.transform.position, Quaternion.identity, canvas.transform).GetComponent<Bomb>();
            if(hex.bomb != null)
            {
                Destroy(hex.bomb.gameObject);
            }
            hex.bomb = temp;
            temp.Init(10);
            bombLastAppearedAtScore = (GameManager.Instance.Score + incrementScore) / 1000 * 1000;
        }

        onScoreChanged.Invoke(tempPos, incrementScore);

        foreach (List<Hexagon> triple in shouldExplodeTriples)
        {
            yield return StartCoroutine(ExplodeRoutine(triple));
        }

    }

    private IEnumerator ExplodeRoutine(List<Hexagon> triple)
    {
        List<GameObject> tempParticles = new List<GameObject>();
        foreach (Hexagon hexagon in selectedHexagon.neighbourTriples[selectedTripleIndex])
        {
            if(!triple.Contains(hexagon))
            {
                hexagon.hexagonImage.enabled = true;
            }
        }

        foreach (Hexagon hexagon in triple)
        {
            ToggleRotatingHexagonsVisibility(false);

            hexagon.ToggleSelectedOverlayVisibility(false);
            
            hexagon.hexagonImage.enabled = false;
            GameObject temp = Instantiate(explodeParticlePrefab, hexagon.transform);
            tempParticles.Add(temp);
            ParticleSystem.MainModule main = temp.GetComponent<ParticleSystem>().main;
            main.startColor = hexagon.colorProp;

            temp.GetComponent<ParticleSystem>().Play();
            if(hexagon.bomb != null)
            {
                Destroy(hexagon.bomb.gameObject);
                hexagon.bomb = null;
            }
        }

        ParticleSystem tempSystem = tempParticles[2].GetComponent<ParticleSystem>();
        while (tempSystem.isPlaying)
        {
            yield return new WaitForSeconds(0.05f);
        }
        yield return StartCoroutine(ScrollColumnColorsInParallel(triple));
    }

    private bool DoesExplodeExist()
    {
        allTriples = GetAllTriples();

        /*GameObject dummy = new GameObject("ASDASD");

        foreach (KeyValuePair<List<Hexagon>, bool> keyValuePair in allTriples)
        {
            foreach (Hexagon hexagon in keyValuePair.Key)
            {
                Instantiate(hexagonPrefab, hexagon.transform.position, Quaternion.identity, canvas.transform).GetComponent<Hexagon>().colorProp = Color.white;
            }
        }*/

        MarkTripleThatWillExplode(allTriples);
        foreach (KeyValuePair<List<Hexagon>, bool> kvp in allTriples)
        {
            if (kvp.Value)
            {
                return true;
            }
        }
        return false;
    }

    private void MarkTripleThatWillExplode(Dictionary<List<Hexagon>, bool> allTriples)
    {
        for (int i = 0; i < allTriples.Count; i++)
        {
            allTriples[allTriples.Keys.ElementAt(i)] = ShouldMarkAsExplode(allTriples.Keys.ElementAt(i));
            if (allTriples[allTriples.Keys.ElementAt(i)])
            {
                break;
            }
        }
        /*
        foreach (KeyValuePair<List<Hexagon>, bool> kvp in allTriples)
        {s
            allTriples[kvp.Key] = ShouldMarkAsExplode(kvp.Key);
        }*/
    }

    private bool ShouldMarkAsExplode(List<Hexagon> hexagons)
    {
        return (hexagons[0].colorProp == hexagons[1].colorProp) && (hexagons[1].colorProp == hexagons[2].colorProp);
    }

    private HexagonContainerColumn GetHexagonContainerColumn(int index)
    {
        //Debug.LogError("A: " + index % 2 + " B: " + index / 2);
        return gridColumns[index % 2].hexagonContainerColumns[index / 2];
    }

    private Dictionary<List<Hexagon>, bool> GetAllTriples()
    {
        Dictionary<List<Hexagon>, bool> allTriples = new Dictionary<List<Hexagon>, bool>();

        foreach (Hexagon hexagon in selectedHexagon.neighbourTriples[selectedTripleIndex])
        {
            foreach (Hexagon hex in GetHexagonContainerColumn(hexagon.coordinates.x).hexagons)
            {
                foreach (List<Hexagon> triple in hex.GetTriples())
                {
                    if (!allTriples.ContainsKey(triple))
                    {
                        allTriples.Add(triple, false);
                    }
                }
            } 
        }

        return allTriples;
    }

    private void CalculateHexagonWidth()
    {
        hexWidth = (Screen.width - gridOffsetFromEdges * 2) / (gridSizeX - ((float) (gridSizeX - 1) / 4));
        hexHeight = hexWidth / defaultHexWidth * defaultHexHeight;
    }

    private void CreateGrid()
    {
        hexagons = new Dictionary<string, Hexagon>();
        gridColumns = new List<GridColumns>();
        for (int i = 0; i < 2; i++)
        {
            gridColumns.Add(Instantiate(gridColumnsPrefab, canvas.transform).GetComponent<GridColumns>());
            gridColumns[i].gridColumnsType = (GridColumnsType)i;
            AddHexagonsWithCoordinates(gridColumns[i].Init());
        }

        float totalPosX = gridColumns[0].GetComponent<RectTransform>().anchoredPosition.x + gridColumns[1].GetComponent<RectTransform>().anchoredPosition.x;

        gridColumns[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-totalPosX / 2, gridColumns[0].GetComponent<RectTransform>().anchoredPosition.y);
        gridColumns[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(totalPosX / 2, gridColumns[1].GetComponent<RectTransform>().anchoredPosition.y);

        foreach (KeyValuePair<string, Hexagon> kvp in hexagons)
        {
            kvp.Value.InitAllNeighbourTriples();
        }

    }

    private void AddHexagonsWithCoordinates(List<Hexagon> hexagonsToAdd)
    {
        foreach (Hexagon hexagon in hexagonsToAdd)
        {
            hexagons.Add(GetCoordinates(hexagon), hexagon);
        }
    }

    public string GetCoordinates(Hexagon hexagon)
    {
        return hexagon.coordinates.x + "_" + hexagon.coordinates.y;
    }

    public string GetCoordinates(int x, int y)
    {
        return x + "_" + y;
    }

    public string GetCoordinates(Vector2Int coordinates)
    {
        return coordinates.x + "_" + coordinates.y;
    }

    public Hexagon GetHexagon(int x, int y)
    {
        return hexagons[GetCoordinates(x, y)];
    }

    public Hexagon GetHexagon(Vector2Int coordinates)
    {
        return hexagons[GetCoordinates(coordinates)];
    }

    public Hexagon GetHexagon(string coordinates)
    {
        return hexagons[coordinates];
    }

    public void DeselectHexagon()
    {
        selectedHexagon.DeselectHexagon();
        selectedHexagon = null;
        ClearRotatingHexagons();
    }

    public void SelectHexagon(Hexagon hexagon, int tripleIndex)
    {
        if (selectedHexagon != null)
        {
            selectedHexagon.DeselectHexagon();
            ClearRotatingHexagons();
        }

        selectedTripleIndex = tripleIndex;

        selectedTripleIndex = ((selectedTripleIndex < 0) || (selectedTripleIndex >= hexagon.neighbourTripleCount)) ? 0 : selectedTripleIndex;

        if (selectedHexagon != null && hexagon != selectedHexagon)
        {
            selectedTripleIndex = 0;
        }

        selectedHexagon = hexagon;


        Vector3 pos = new Vector3();
        foreach (Hexagon hex in selectedHexagon.neighbourTriples[selectedTripleIndex])
        {
            pos += hex.transform.position;
        }

        pos /= 3;

        rotatingHexagonsParent.transform.position = pos;

        foreach (Hexagon hex in selectedHexagon.neighbourTriples[selectedTripleIndex])
        {
            GameObject temp = Instantiate(hex.gameObject, rotatingHexagonsParent.transform, true);
            Hexagon tempHex = temp.GetComponent<Hexagon>();
            rotatingHexagonsParent.rotatingHexagons.Add(tempHex);
            tempHex.ToggleSelectedOverlayVisibility(false);
            tempHex.colorProp = hex.colorProp;
        }
    }

    private void ToggleRotatingHexagonsVisibility(bool enable)
    {
        foreach (Hexagon hexagon in rotatingHexagonsParent.GetComponentsInChildren<Hexagon>())
        {
            hexagon.hexagonImage.enabled = enable;
            hexagon.ToggleSelectedOverlayVisibility(enable);
        }
    }

    private void ClearRotatingHexagons()
    {
        int count = rotatingHexagonsParent.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Destroy(rotatingHexagonsParent.transform.GetChild(i).gameObject);
        }

        rotatingHexagonsParent.rotatingHexagons = new List<Hexagon>();
    }
}

public enum RotateType
{
    Right = -1,
    Left = 1
}

public class CoroutineHelper
{
    public enum CoroutineState
    {
        Idle,
        Running,
        Finished
    }

    public CoroutineState state;
    public Coroutine coroutine;

    public CoroutineHelper(Coroutine coroutine, CoroutineState state)
    {
        this.coroutine = coroutine;
        this.state = state;
    }
}

