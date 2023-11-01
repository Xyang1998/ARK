using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Enemy_mslime",menuName = "ScriptableObject/Enemy_mslime")]
public class Enemy_mslime : NormalContractBuff
{
    [Tooltip("需要添加的buff")]
    public BaseBuff addedBuff;

    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        base.AddBuffToTarget(_initiator, _target);
        if (_initiator.attack.buffs == null)
        {
            _initiator.attack.buffs = new List<BaseBuff>(){addedBuff};
        }
        else
        {
            _initiator.attack.buffs.Add(addedBuff);
        }
    }
}
