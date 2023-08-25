using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public enum CharacterState
{
    Live,
    Frost
}





[Serializable]
public enum BuffType
{
   
    Buff,
    DeBuff,
    Event,
    [Tooltip("被动技能")]
    Passive
}
[Serializable]
public enum CharacterCamp
{
    
    Player,
    Enemy,
    Self
}
/// <summary>
/// 该buff随着角色回合减少还是随着整体回合减少
/// </summary>
public enum BuffTurnType
{
    Character,
    Global
}
[Serializable]
public enum DataProperty
{
    ATK,
    [Tooltip("不要在这里修改血量！")]
    HP,
    MaxHP,
    NP,
    MaxNP,
    Defense,
    MagicDefense,
    CritRate,
    CriticalDamage,
    Healing,
    EffectHitRate,
    EffectResistanceRate,
    NPRate,
    TauntValue,
    Speed,
    CurPos,
    DamageRate,
    GetDamageRate,
    Cost,
}

/// <summary>
/// 提升的数值是基于自身还是目标？
/// </summary>
[Serializable]
public enum NumericalFundamental
{
    Initiator,
    Target
}

[Serializable]
public struct SingleBuff
{
    [HideInInspector] public float temp; //修改值，用于buff移除时
    /// <summary>
    /// 目标修改的数值类型
    /// </summary>
    [Tooltip("修改目标的什么值？")]
    public DataProperty targetProperty;
    /// <summary>
    /// 基于什么属性计算增加值
    /// </summary>
    [Tooltip("基于创建者还是目标的什么值？")]
    public DataProperty initiatorProperty;
    [Tooltip("基于创建者还是目标？")]
    public NumericalFundamental numericalFundamental;
    public float modified; //提升的固定值
    public float modified_per; //提升的百分比(基于创建者或目标)
}

[Serializable]
public struct DamageBuff
{
    public DamageType damageType;
    [Tooltip("基于创建者的什么值造成伤害")]
    public DataProperty baseAttribute;
    public float damageRate; //百分比伤害
    public float fixedDamage; //固定伤害
}



public struct SelectResult
{
    /// <summary>
    /// 选择的目标
    /// </summary>
    public BaseCharacter target;
    /// <summary>
    /// 使用的技能
    /// </summary>
    public BaseSkill skill;

    [Tooltip("使用技能该回合是否结束？")]
    public bool turnEnd;

}




[Serializable]
public struct EnemySetting
{
    public int num;
    public int ID;
    public EnemyName enemyName;

}

public static class DeepCopy
{
    public static T DeepCopyByReflect<T>(T obj)
    {
        if (obj == null || (obj is string) || (obj.GetType().IsValueType)) return obj;
 
        object retval = Activator.CreateInstance(obj.GetType());
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (FieldInfo field in fields)
        {
            try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
            catch { }
        }
        return (T)retval;
    }
}
/// <summary>
/// 用于保存群攻技能左右目标个数
/// </summary>
[Serializable]
public struct MultipleTarget
{
    [SerializeField]
    private int leftNum ;
    public int LeftNum
    {
        get => leftNum;
        set
        {
            if (value < 0)
            {
                leftNum = 0;
            }
            else
            {
                leftNum = value;
            }
        }
    }
    [SerializeField]
    private int rightNum ;

    public int RightNum
    {
        get => rightNum;
        set
        {
            if (value < 0)
            {
                rightNum = 0;
            }
            else
            {
                rightNum = value;
            }
        }
    }

}

/// <summary>
/// 职业
/// </summary>
public enum CharacterClass
{
    Warrior,
    Tank,
    Support,
    Special,
    Sniper,
    Pioneer,
    Medic,
    Caster
}

public static class FilePath
{
    public static string battleUIPrefabPath = "UI/Prefabs/Battle/";
    public static string onMapUIPrefabPath = "UI/Prefabs/OnMap/";

}

public static class CostDef
{
    public static Dictionary<int, int> costDict = new Dictionary<int, int>()
    {
        {4,3},
        {5,3},
        {6,3}
    };
}