using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillText
{
    public int id;
    public int cost;
    public string iconName;
    public string skillName;
    public string description;
}

public struct BuffText
{
    public int id;
    public string buffName;
    public string description;
}

public struct CharacterText
{
    public int id;
    public string name;
    public string camp;
    public string stars;
    public string CName;
    public string characterClass;
    public string description;
    public string skillIDs;
}
/// <summary>
/// 危机合约
/// </summary>
public struct ContractText
{
    public int id;
    public string iconName;
    public string name;
    public string group;
    public string level;
    public string description;
    public string conflictIDs;

}
[Serializable]
public struct MapEnemySetting
{
    public int EnemyID;
    public EnemySetting[] enemySetting;
    public EnemySetting[] reserveEnemySetting;
}