using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StateSwitch",menuName = "ScriptableObject/Others/StateSwitch")]
public class StateSwitch : BaseSkill
{
    public override void ManualApply(BaseCharacter initiator, BaseCharacter target)
    {
        BaseBuff baseBuff = initiator.HasBuff(selfBuffs[0].buffID);
        if (baseBuff != null)
        {
            initiator.RemoveBuffByID(baseBuff.buffID);
        }
        else
        {
            ApplySelfBuff(initiator,initiator);
        }
    }
}
