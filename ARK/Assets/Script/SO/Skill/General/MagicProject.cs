using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Spine;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "Project",menuName = "ScriptableObject/Project")]
public class MagicProject : BaseSkill
{
    //创建魔法弹的骨骼
    private Bone start;
    private SkeletonAnimation skeletonAnimation;

    [Tooltip("如果勾选，则只对主目标生成投掷物，否则对每个目标生成投掷物")]
    public bool generateOnce=true;
    [Tooltip("创建魔法弹的骨骼名字")]
    public string boneName;
    //魔法弹物体
    public GameObject project;
    //X轴移动速度(Y轴根据距离所定)
    public float costTime;
    

    //判断出发点与目标点X轴关系
    private bool X;
    //出发点与目标点纵坐标差距
    private float Y;
    public override void OnAttack(BaseCharacter initiator, BaseCharacter target, float damage, bool critical, bool isFirst, bool isFinal,
        bool isDamage, float initiatorNP, float targetNP,bool isMainTarget)
    {
        skeletonAnimation ??= initiator.CharacterGO.GetComponent<SkeletonAnimation>();
        start ??= skeletonAnimation.Skeleton.FindBone(boneName);
        if (start == null)
        {
            Debug.Log($"未找到名为{boneName}的骨骼！");
            return;
        }
        //TODO:对象池优化
        if (!generateOnce||(generateOnce && isMainTarget))
        {
            GameObject go = Instantiate(project);
            Vector3 startPos = start.GetWorldPosition(skeletonAnimation.transform);
            Vector3 endPos = target.AnimAndDamageController.magicPoint.transform.position;
            //以我方视角考虑，X为真
            X = endPos.x > startPos.x;
            Y = startPos.y - endPos.y;
            go.transform.position = startPos;
            LinearMoveToEnd(go, endPos, X, Y, initiator, target, damage, critical, isFirst, isFinal, isDamage,
                initiatorNP, targetNP,isMainTarget).Forget();
        }
        else
        {
            DelayAndApply(initiator, target, damage, critical, isFirst, isFinal, isDamage,
                initiatorNP, targetNP,isMainTarget).Forget();
        }
    }

    public async UniTaskVoid LinearMoveToEnd(GameObject go,Vector3 endPos ,bool X,float Y,BaseCharacter initiator, BaseCharacter target, float damage, bool critical, bool isFirst, bool isFinal,
        bool isDamage, float initiatorNP, float targetNP,bool isMainTarget)
    {
        Vector3 curPos = go.transform.position;
        //float costTime = Mathf.Abs(curPos.x - endPos.x)/speedX;
        float speedX = Mathf.Abs(curPos.x - endPos.x) / costTime;
        float speedY = Y / costTime;
        float distanceX = Time.fixedDeltaTime * speedX;
        float distanceY = Time.fixedDeltaTime * speedY;
        curPos.z -= 0.1f;
        while ((endPos.x>go.transform.position.x)==X)
        {
            
            curPos.x += X ? distanceX : -distanceX;
            curPos.y -= distanceY;
            if (endPos.x > curPos.x != X) break;
            go.transform.position = curPos;
            await UniTask.WaitForFixedUpdate();
        }
        Destroy(go);
        ApplyAttack(initiator,target,damage,critical,isFirst,isFinal,isDamage,initiatorNP,targetNP,isMainTarget);
        
    }

    public async UniTaskVoid DelayAndApply(BaseCharacter initiator, BaseCharacter target, float damage, bool critical, bool isFirst, bool isFinal,
        bool isDamage, float initiatorNP, float targetNP,bool isMainTarget)
    {
        await UniTask.Delay((int)(costTime*1000));
        ApplyAttack(initiator,target,damage,critical,isFirst,isFinal,isDamage,initiatorNP,targetNP,isMainTarget);
    }
    
    
    
    
    
}
