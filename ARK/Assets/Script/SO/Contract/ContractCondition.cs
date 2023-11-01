using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable,Tooltip("危机合约条件检查结构体")]
public struct ContractCondition
{
    [Tooltip("该合约是否属于特殊类别？如果是，则无需判定其他条件")] public bool special;
    
    [Tooltip("该合约适用于哪些阵营？")]
    public CharacterCamp[] optionalType;

    [Tooltip("该合约适用于哪些职业？")]
    public CharacterClass[] characterClasses;
    
    [Tooltip("该合约适用于哪些角色？,与上述条件互斥")]
    public int[] ids;

}
[Serializable]
public struct SingleContract
{
    public ContractCondition contractCondition;
    public BaseBuff[] buffs;

}
