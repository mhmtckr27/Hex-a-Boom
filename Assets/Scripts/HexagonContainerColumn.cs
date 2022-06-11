using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexagonContainerColumn : MonoBehaviour
{
    public List<Hexagon> hexagons;

    public List<Hexagon> Init(int columnIndex)
    {
        GetComponent<GridLayoutGroup>().cellSize = new Vector2(GridManager.Instance.hexWidth, GridManager.Instance.hexHeight);

        hexagons = new List<Hexagon>();
        for (int i = 0; i < GridManager.Instance.gridSizeY; i++)
        {
            hexagons.Add(Instantiate(GridManager.Instance.hexagonPrefab, transform).GetComponent<Hexagon>());
            bool shouldBeBlack = i == 0 || i == GridManager.Instance.gridSizeY - 1 || columnIndex == 0 ||
                                 columnIndex == GridManager.Instance.gridSizeX - 1;
            shouldBeBlack = false;
            hexagons[i].Init(new Vector2Int(columnIndex, i), (i == 0 ? Color.black : hexagons[i - 1].colorProp), shouldBeBlack);
        }

        return hexagons;
    }

    public void ToggleVisibilityOfChildrenBetweenIndices(int start, int end, bool enable)
    {
        for(int i = start; i < end; i++)
        {
            hexagons[i].hexagonImage.enabled = enable;
        }
    }
}
