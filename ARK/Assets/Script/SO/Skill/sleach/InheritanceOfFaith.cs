using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

//[CreateAssetMenu(fileName = "InheritanceOfFaith",menuName = "ScriptableObject/InheritanceOfFaith")]
public class InheritanceOfFaith : BaseSkill
{
    public GameObject drop;
    private GameObject _drop;
    public float speed = 12;
    public override void OnAttack(BaseCharacter initiator, BaseCharacter target, float damage, bool critical, bool isFirst, bool isFinal,
        bool isDamage, float initiatorNP, float targetNP, bool isMainTarget)
    {
        if (isMainTarget)
        {
            _drop = Instantiate(drop, target.CharacterGO.transform.parent.transform);
            _drop.transform.rotation=Quaternion.Euler(0,0,0);
            _drop.AddComponent<LookAtCamera>();
           
        }
        Drop(initiator, target, damage, critical, isFirst, isFinal, isDamage, initiatorNP, targetNP, isMainTarget)
            .Forget();

    }

    private async UniTaskVoid Drop(BaseCharacter initiator, BaseCharacter target, float damage, bool critical, bool isFirst, bool isFinal,
        bool isDamage, float initiatorNP, float targetNP, bool isMainTarget)
    {
        
        Vector3 endPos = new Vector3(0, 2.5f, 0.3f);
        Vector3 startPos = endPos+ new Vector3(0, 6, 0);
        if (isMainTarget)
        {
            _drop.transform.localPosition = startPos;
        }

        while (true)
        {
            Vector3 curPos=Vector3.MoveTowards(_drop.transform.localPosition,endPos,speed*Time.fixedDeltaTime);
            if (curPos.y <= endPos.y) break;
            if (isMainTarget)
            {
                _drop.transform.localPosition = curPos;
            }
            await UniTask.WaitForFixedUpdate();
        }

        if (isMainTarget)
        {
            _drop.transform.localPosition = endPos;
        }

        ApplyAttack(initiator,target,damage,critical,isFirst,isFinal,isDamage,initiatorNP,targetNP,isMainTarget);
        
    }

    public void RemoveDrop()
    {

        if (_drop)
        {
            Destroy(_drop);
        }
    }
}
