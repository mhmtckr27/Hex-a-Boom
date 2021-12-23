using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBG : MonoBehaviour
{
    [SerializeField] private GameObject dummyHexagonPrefab;
    [SerializeField] private int dummyHexagonCount;
    [SerializeField] private float dummyHexagonLifeTime;

    List<MainMenuDummyHexagon> dummies;

    private void Awake()
    {
        dummies = new List<MainMenuDummyHexagon>();
        for(int i = 0; i < dummyHexagonCount; i++)
        {
            dummies.Add(CreateDummyHexagon());
        }
    }

    private MainMenuDummyHexagon CreateDummyHexagon()
    {
        MainMenuDummyHexagon temp = Instantiate(dummyHexagonPrefab, transform).GetComponent<MainMenuDummyHexagon>();
        temp.Init(dummyHexagonLifeTime, this);
        return temp;
    }

    private void ReInitDummy(MainMenuDummyHexagon dummy)
    {
        dummy.gameObject.SetActive(true);
        dummy.Init(dummyHexagonLifeTime, this);
    }

    public void OnDummyHexagonDestroyed(MainMenuDummyHexagon dummy)
    {
        ReInitDummy(dummy);
    }
}
