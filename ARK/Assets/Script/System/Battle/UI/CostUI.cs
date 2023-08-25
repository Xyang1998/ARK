using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CostUI : MonoBehaviour
{
    public Text costNum;

    public void SetCostNum(int num)
    {
        costNum.text = num.ToString();
    }
}
