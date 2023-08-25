using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Debilitate",menuName = "ScriptableObject/Debilitate")]
public class Debilitate : BaseBuff
{
    [Tooltip("hp小于等于多少时生效")]
    public float hpRate;

    [Tooltip("生效中？")]
    private bool isEffect=false;

    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        CurLayers += 1;
        initiator = _initiator;
        target = _target;
        _target.AddBuff(this);
        GetHitAction(null,null,null);
        target.hpChangeAction += GetHitAction;

    }

    public void GetHitAction(BaseCharacter _1,BaseCharacter _2,BaseSkill _3)
    {
        if ((target.BattleCharacterStateData.HP / target.BattleCharacterStateData.MaxHP) <= hpRate)
        {
            if (isEffect == false)
            {
                isEffect = true;
                ChangeTargetProperties();
            }
        }
        else
        {
            if (isEffect == true)
            {
                isEffect = false;
                ResetTargetProperties();
            }
        }
        
    }

    public override void BuffRemove()
    {
        if (isEffect == true)
        {
            ResetTargetProperties();
        }
        target.hpChangeAction -= GetHitAction;
        BuffReset();
        iconImage = null;
    }
}
