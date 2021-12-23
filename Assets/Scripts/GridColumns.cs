using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridColumns : MonoBehaviour
{
    public GridColumnsType gridColumnsType;
    public List<HexagonContainerColumn> hexagonContainerColumns;

    public List<Hexagon> Init()
    {
        List<Hexagon> hexagons = new List<Hexagon>();
        int columnCount = 0;
        switch (gridColumnsType)
        {
            case GridColumnsType.First:
                columnCount = (GridManager.Instance.gridSizeX + 1) / 2;
                break;
            case GridColumnsType.Second:
                columnCount = GridManager.Instance.gridSizeX / 2;
                float posX = GridManager.Instance.hexWidth * 3 / 4;
                float posY = GetComponent<RectTransform>().anchoredPosition.y + (-1 * (GridManager.Instance.hexWidth / 4) * Mathf.Sqrt(3));
                GetComponent<RectTransform>().anchoredPosition = new Vector3(posX + 0.5f, posY, 0);
                break;
            default:
                break;
        }

        GetComponent<GridLayoutGroup>().cellSize = new Vector2(GridManager.Instance.hexWidth, GridManager.Instance.gridSizeY * GridManager.Instance.hexHeight + (GridManager.Instance.gridSizeY - 1));
        GetComponent<GridLayoutGroup>().spacing = new Vector2(GridManager.Instance.hexWidth / 2 + 1, 1);

        hexagonContainerColumns = new List<HexagonContainerColumn>();
        for(int i = 0; i < columnCount; i++)
        {
            hexagonContainerColumns.Add(Instantiate(GridManager.Instance.hexagonContainerColumnPrefab, transform).GetComponent<HexagonContainerColumn>());
            hexagons.AddRange(hexagonContainerColumns[i].Init(gridColumnsType == GridColumnsType.First ? i * 2 : i * 2 + 1));
        }
        return hexagons;
    }
}

public enum GridColumnsType
{
    First,
    Second
}
