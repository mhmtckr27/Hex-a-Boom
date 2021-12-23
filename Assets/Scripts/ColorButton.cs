using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public void OnClick()
    {
        UIManager.Instance.colorPicker.onConfirmColor.AddListener(color =>
        {
            GetComponent<Image>().color = color;
        });
        UIManager.Instance.ToggleColorPickerVisibility(true, GetComponent<Image>().color);
    }
}
