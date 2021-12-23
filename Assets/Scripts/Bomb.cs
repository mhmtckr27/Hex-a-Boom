using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Text remainingMovesText;
    private int remainingMovesToExplode;

    public void Init(int bombCountdown)
    {
        remainingMovesToExplode = bombCountdown;
        remainingMovesText.text = remainingMovesToExplode.ToString();
        GridManager.Instance.onMovesCountChanged.AddListener(OnMovesIncremented);
    }

    void OnMovesIncremented(int increment)
    {
        remainingMovesToExplode -= increment;
        remainingMovesText.text = remainingMovesToExplode.ToString();

        if (remainingMovesToExplode <= 0)
        {
            UIManager.Instance.ToggleSettingsPanelVisibility(true, true);
        }
    }
}
