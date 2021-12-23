using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hexagon : MonoBehaviour
{
    [SerializeField] public Vector2Int coordinates;
    public List<List<Hexagon>> neighbourTriples;
    public int neighbourTripleCount;
    private Color color;
    public Color colorProp
    {
        get
        {
            return color;
        }
        set
        {
            color = value;
            hexagonImage.color = color;
        }
    }
    public Image hexagonImage;

    private GameObject selectedOverlay;

    public Bomb bomb;

    private void Awake()
    {
        selectedOverlay = transform.GetChild(0).gameObject;
        hexagonImage = GetComponent<Image>();
    }

    public void Init(Vector2Int coordinates, Color previousHexagonColor)
    {
        this.coordinates = coordinates;
        colorProp = GetRandomColorExcept(previousHexagonColor);
    }

    //pretty messy TODO: simplify
    public void InitAllNeighbourTriples()
    {
        neighbourTriples = new List<List<Hexagon>>();
        if(coordinates.x == 0)
        {
            if(coordinates.y == 0)
            {
                neighbourTripleCount = 1;

                List<Hexagon> temp = new List<Hexagon>();

                temp.Add(this);
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                neighbourTriples.Add(temp);

            }
            else if(coordinates.y == GridManager.Instance.gridSizeY - 1)
            {
                neighbourTripleCount = 2;

                List<Hexagon> temp = new List<Hexagon>();
                temp.Add(this);
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y - 1));
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                neighbourTriples.Add(temp);

                temp = new List<Hexagon>();

                temp.Add(this);
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y - 1));
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                neighbourTriples.Add(temp);

            }
            else
            {
                neighbourTripleCount = 3;

                List<Hexagon> temp = new List<Hexagon>();
                temp.Add(this);
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y - 1));
                neighbourTriples.Add(temp);

                temp = new List<Hexagon>();

                temp.Add(this);
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y - 1));
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                neighbourTriples.Add(temp);

                temp = new List<Hexagon>();

                temp.Add(this);
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                neighbourTriples.Add(temp);

            }
        }
        else if(coordinates.x == GridManager.Instance.gridSizeX - 1)
        {
            if(GridManager.Instance.gridSizeX % 2 == 0)
            {
                if (coordinates.y == 0)
                {
                    neighbourTripleCount = 2;

                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y + 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y + 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    neighbourTriples.Add(temp);
                }
                else if (coordinates.y == GridManager.Instance.gridSizeY - 1)
                {
                    neighbourTripleCount = 1;

                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    neighbourTriples.Add(temp);
                }
                else
                {
                    neighbourTripleCount = 3;

                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y - 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    neighbourTriples.Add(temp);
                }
            }
            else
            {
                if (coordinates.y == 0)
                {
                    neighbourTripleCount = 1;

                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    neighbourTriples.Add(temp);
                }
                else if (coordinates.y == GridManager.Instance.gridSizeY - 1)
                {
                    neighbourTripleCount = 2;

                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y - 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    neighbourTriples.Add(temp);
                }
                else
                {
                    neighbourTripleCount = 3;

                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y - 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    neighbourTriples.Add(temp);
                }
            }
        }
        else
        {
            if(coordinates.y == 0)
            {
                if(coordinates.x % 2 == 0)
                {
                    neighbourTripleCount = 2;

                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                    neighbourTriples.Add(temp);

                }
                else
                {
                    neighbourTripleCount = 4;

                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y + 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y + 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y + 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y + 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                    neighbourTriples.Add(temp);
                }
            }
            else if(coordinates.y == GridManager.Instance.gridSizeY - 1)
            {
                if (coordinates.x % 2 == 0)
                {
                    neighbourTripleCount = 4;

                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y - 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y - 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                    neighbourTriples.Add(temp);

                }
                else
                {
                    neighbourTripleCount = 2;

                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                    neighbourTriples.Add(temp);
                }
            }
            else
            {
                neighbourTripleCount = 6;

                if (coordinates.x % 2 == 0)
                {
                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y - 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y - 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    neighbourTriples.Add(temp);
                }
                else
                {
                    List<Hexagon> temp = new List<Hexagon>();
                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y + 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x + 1, coordinates.y + 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y + 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y + 1));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y + 1));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    neighbourTriples.Add(temp);

                    temp = new List<Hexagon>();

                    temp.Add(this);
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x - 1, coordinates.y));
                    temp.Add(GridManager.Instance.GetHexagon(coordinates.x, coordinates.y - 1));
                    neighbourTriples.Add(temp);
                }
            }
        }
    }



    public static Color GetRandomColor()
    {
        return GridManager.Instance.hexagonColors[UnityEngine.Random.Range(0, GridManager.Instance.hexagonColors.Count)];
    }

    private Color GetRandomColorExcept(Color exception)
    {
        //return GetRandomColor();
        List<Color> filteredColors = GridManager.Instance.hexagonColors.Where(color => color != exception).ToList();

        return filteredColors[UnityEngine.Random.Range(0, filteredColors.Count)];
    }

    public void DeselectHexagon()
    {
        foreach (Hexagon hexagon in neighbourTriples[GridManager.Instance.selectedTripleIndex])
        {
            hexagon.ToggleSelectedOverlayVisibility(false);
        }
    }

    public void SelectHexagon()
    {
        if(!InputManager.Instance.isInputEnabled){
            return;
        }
        GridManager.Instance.SelectHexagon(this, GridManager.Instance.selectedTripleIndex + 1);
        foreach(Hexagon hexagon in neighbourTriples[GridManager.Instance.selectedTripleIndex])
        {
            hexagon.ToggleSelectedOverlayVisibility(true);
        }
    }

    public void SelectHexagon(int selectedTripleIndex)
    {
        if (!InputManager.Instance.isInputEnabled)
        {
            return;
        }
        GridManager.Instance.SelectHexagon(this, selectedTripleIndex);
        foreach (Hexagon hexagon in neighbourTriples[GridManager.Instance.selectedTripleIndex])
        {
            hexagon.ToggleSelectedOverlayVisibility(true);
        }
    }

    public void ToggleSelectedOverlayVisibility(bool enable)
    {
        selectedOverlay.SetActive(enable);
    }


    public List<List<Hexagon>> GetTriples()
    {
        //List<List<Hexagon>> neighbours = new List<List<Hexagon>>();

        /*foreach(List<Hexagon> triple in hexagon.neighbourTriples)
        {
            foreach(Hexagon neighbour in triple)
            {
                if(!neighbours.Contains(neighbour) && neighbour != hexagon)
                {
                    neighbours.Add(neighbour);
                }
            }
        }*/

        return neighbourTriples;
    }
}
