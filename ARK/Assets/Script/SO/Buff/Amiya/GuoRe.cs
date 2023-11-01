using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//[CreateAssetMenu(fileName = "GuoRe",menuName = "ScriptableObject/GuoRe")]
public class GuoRe : BaseBuff
{
    [Tooltip("每层增加伤害")]
    public float layerInDamage;
    private BaseCharacter preTarget;

    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        base.AddBuffToTarget(_initiator, _target);
        CurLayers = 0;
        iconImage.GetComponent<BuffLayer>().SetNum(CurLayers);
    }

    public override void BeforeAttack(BaseCharacter _initiator, BaseCharacter _target, BaseSkill skill)
    {
        if (_target == preTarget)
        {
            if (CurLayers < maxLayers)
            {
                CurLayers += 1;
                _initiator.BattleCharacterStateData.DamageRate += layerInDamage;
            }
        }
        else
        {
            preTarget = _target;
            _initiator.BattleCharacterStateData.DamageRate -= CurLayers*layerInDamage;
            CurLayers = 0;
        }
        
        iconImage.GetComponent<BuffLayer>().SetNum(CurLayers);
    }
}
