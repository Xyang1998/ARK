using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PriRunnerUI : MonoBehaviour
{
    public Image icon;

    public void Init(Sprite _icon)
    {
        if (_icon != null)
        {
            icon.sprite = _icon;
        }
    }
}
