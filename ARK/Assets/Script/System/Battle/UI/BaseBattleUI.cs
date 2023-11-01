using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseBattleUI : BaseUI
{

    public Text Name;//人物名称
    //public Image HP;

    public Image NPBackground;

    
    //protected float npTargetPer;
    //protected float npCurPer;
    //protected bool updatingNP=false;
    /// <summary>
    /// target>Cur时,flag=ture,否则为false
    /// </summary>
    //protected bool npFlag = false;
    
    
    
    public GameObject buffLine;
    private List<GameObject> bufflist;
    private int maxBuffIcons = 0;
    private Vector2 buffIconSize;

    //private float targetPer;
    //private float curPer;
    //private bool updatingHP=false;
    /// <summary>
    /// target>Cur时,flag=ture,否则为false
    /// </summary>
    //private bool flag = false;
    
    
    public override void Init(CharacterStateData data, CharacterDataStruct dataStruct)
    {
        base.Init(data, dataStruct);
        //outline = HP.GetComponent<Outline>();
        buffIconSize = new Vector2(buffLine.GetComponent<RectTransform>().sizeDelta.y,buffLine.GetComponent<RectTransform>().sizeDelta.y);
        maxBuffIcons = (int)Math.Floor(buffLine.GetComponent<RectTransform>().sizeDelta.x /
                       (buffLine.GetComponent<HorizontalLayoutGroup>().padding.left +
                        buffLine.GetComponent<RectTransform>().sizeDelta.y));
        bufflist = new List<GameObject>();
    }

    public virtual void Selected()
    {
        //outline.enabled = true;
    }

    public virtual void UnSelected()
    {
        //outline.enabled = false;
    }
   

    public void AddBuff(GameObject icon)
    {
        icon.transform.SetParent(buffLine.transform);
        icon.GetComponent<RectTransform>().sizeDelta = buffIconSize;
        bufflist.Add(icon);
        if (bufflist.Count > maxBuffIcons)
        {
            for (int i = maxBuffIcons; i < bufflist.Count; i++)
            {
                bufflist[i].SetActive(false);
            }
        }

    }

    public void RemoveBuff(GameObject go)
    {
        bufflist.Remove(go);
    }

    public virtual void UltimateReady()
    {
        
    }
   
    
    
}
