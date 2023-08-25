using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class BuffLayer : MonoBehaviour
{
    public Text layerNum;

    public void SetNum(int n)
    {

        layerNum.text = n.ToString();
        
        
    }
}
