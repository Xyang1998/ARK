using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "frost",menuName = "ScriptableObject/frost")]
public class frost : StateBuff
{
    private GameObject bingkuai;
    public override void AddBuffToTarget(BaseCharacter _initiator, BaseCharacter _target)
    {
        base.AddBuffToTarget(_initiator, _target);
        if (exist==null)
        {
            _target.AnimAndDamageController.animationState.TimeScale = 0;
            //_target.CharacterState = CharacterState.Frost;
            bingkuai = Instantiate(Resources.Load<GameObject>("Effect/Prefabs/bingkuai"));
            Vector3 pos = _target.AnimAndDamageController.magicPoint.transform.position;
            pos.y += 0.3f;
            pos.z -= 0.05f;
            bingkuai.transform.position = pos;
        }
        
        
    }

    public override void BuffRemove()
    {
        
        target.AnimAndDamageController.animationState.TimeScale = 1;
        //TODO:添加BUFF类型来控制多个眩晕buff
        Destroy(bingkuai);
        base.BuffRemove();
        
    }
}
