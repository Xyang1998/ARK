using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[CreateAssetMenu(fileName = "Debilitate_P",menuName = "ScriptableObject/Debilitate_P")]
public class Debilitate_P : BaseBuff
{
    public BaseBuff debilitate;
    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        base.AddBuffToTarget(_initiator, _target);
        BattleSystem.Instance.spawnAction += AddDebilitate;
        
    }
    private void AddDebilitate(BaseCharacter _target)
    {
        if (_target.CharacterDataStruct.characterCamp == CharacterCamp.Enemy)
        {
            BaseBuff buff = Instantiate(debilitate);
            buff.AddBuffToTarget(initiator,_target);
        }
    }
    
    public override void BuffRemove()
    {
        foreach (var enemy in BattleSystem.Instance.enemies)
        {
            if (enemy != null)
            {
                enemy.RemoveBuffByID(debilitate.buffID);
            }
        }
        BattleSystem.Instance.spawnAction -= AddDebilitate;
        base.BuffRemove();
        
    }
}
