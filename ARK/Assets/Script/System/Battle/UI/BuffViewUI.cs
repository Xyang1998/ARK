using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffViewUI : MonoBehaviour
{
    public Image icon;
    public Text buffName;
    public Text buffDescription;
    public Text layerNum;
    public Text remain;
    public Text remainTurn;

    public void BindBuff(Sprite _icon, string _buffName, string _buffDescription, int _layerNum,bool isPermanent,int _remainTurn)
    {
        icon.sprite = _icon;
        buffName.text = _buffName;
        buffDescription.text = _buffDescription;
        layerNum.text = _layerNum.ToString();
        if (isPermanent)
        {
            remain.enabled = false;
            remainTurn.enabled = false;
        }
        else
        {
            remain.enabled = true;
            remainTurn.enabled = true;
            remainTurn.text = _remainTurn.ToString();
        }
    }
}
