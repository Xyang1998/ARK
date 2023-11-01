using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



[Serializable]
public class Enemy_OnMap : MonoBehaviour
{

    public int EnemyID;
    public MapEnemySetting mapEnemySetting;
    public AudioClip bgm;

    public void Start()
    {
        mapEnemySetting = TextSystem.enemySettingTextLoader.GetTextStruct(EnemyID);
    }
}
