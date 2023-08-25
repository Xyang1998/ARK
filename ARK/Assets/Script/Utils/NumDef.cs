using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumericalDefinition
{
    
    
    [Tooltip("受击时获得的NP")]
    public static float getHitNP = 10.0f;

    [Tooltip("暴击时NP获得倍率")]
    public static float criticalNPGet = 1.5f;
    
    [Tooltip("物理伤害的最小比例")]
    public static float minDamageRate=0.05f;

    [Tooltip("最大角色数")]
    public static int maxCharacters = 4;

    [Tooltip("最大敌人数")]
    public static int maxEnemies = 4;

    [Tooltip("移动耗时")]
    public static float moveCostTime = 0.3f;

    [Tooltip("血条衰减速率")] 
    public static float hpFadeSpeed = 1.0f;

    [Tooltip("初始COST")] public static int startCost = 20;

}