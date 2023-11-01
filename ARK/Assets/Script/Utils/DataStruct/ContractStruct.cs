using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct ContractStruct 
{
    public int id;
    /// <summary>
    /// 图标名及SO名
    /// </summary>
    public string iconName;
    public string name;
    public int group; //组
    public int level; //登记
    public string description;
    public int[] conflictIDs;
    public Sprite icon;
}
