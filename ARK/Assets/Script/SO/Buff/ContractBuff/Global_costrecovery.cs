using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Global_costrecovery",menuName = "ScriptableObject/Global_costrecovery")]
public class Global_costrecovery : NormalContractBuff
{
    [Tooltip("cost变化？")] public float costChangeRate = 0;
    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        BattleSystem.Instance.CostRate += costChangeRate * BattleSystem.Instance.CostRate;
    }
}
