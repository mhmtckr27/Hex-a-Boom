using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingHexagonsParent : MonoBehaviour
{
    public List<Hexagon> rotatingHexagons;

    private void Awake()
    {
        rotatingHexagons = new List<Hexagon>();
    }

    public Transform GetHexagon(Hexagon hexagon)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (Vector3.Distance(transform.GetChild(i).transform.position, hexagon.transform.position) < 0.1f)
            {
                return transform.GetChild(i);
            }
        }
        return null;
    }
}
