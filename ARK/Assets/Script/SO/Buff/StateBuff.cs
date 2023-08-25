using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBuff : BaseBuff
{
    //StateBuff必唯一
    protected BaseBuff exist;
    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        initiator = _initiator;
        target = _target;
        exist=_target.HasBuff(buffID);
        if (exist)
        {
            exist.ResetRemain(durationTurns);
        }
        else
        {
            remainTurns = durationTurns;
            _target.AddBuff(this);
            _target.AddStateBuff(this);
        }
    }

    public override void BuffRemove()
    {
        target.RemoveStateBuff(this);
        base.BuffRemove();
    }
}
