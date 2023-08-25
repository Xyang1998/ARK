using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


public enum DamageType
{
    Healing, //治疗
    Physical, //物理
    Magic, //魔法
    Real, //真实
    Buff //纯buff
}



public abstract class BaseSkill : ScriptableObject
{
    [Tooltip("技能ID")] public int skillID;

    //TODO:根据ID加载
    //[Tooltip("技能图标")] public Sprite icon;
    [HideInInspector]
    public SkillDataStruct skillDataStruct;

    [Tooltip("可选择的目标")] public CharacterCamp[] optionalType = new CharacterCamp[] { CharacterCamp.Enemy };
    
    [Tooltip("该技能花费的Cost"),HideInInspector]
    public int cost = -5;

    public bool canUse = true;
    [Tooltip("使用该技能回合是否结束？")]
    public bool turnEnd = true;

    [Tooltip("基于发起者的什么属性造成伤害？")] public DataProperty baseAttribute;
    public DamageType damageType;
    public int hitNum = 1;
    private int prehitNum = 0;
    [Tooltip("每段攻击的比例")] public float[] hitsRate = new float[] { 1 };

    [Tooltip("伤害倍率")] public float damageRate = 1.0f;


    [Tooltip("释放时获得的基础NP")] public float NPGet = 10.0f;
    [Tooltip("每段攻击NP获得比例")] public float[] NPHitsRate = new float[] { 1 };

    public MultipleTarget multipleTarget;


    //[Tooltip("群攻技能主目标左次目标个数")]
    //public int leftAttackNum = 0;
    private int preLeftAttackNum = -1;
    [Tooltip("群攻技能主目标左次目标伤害比例")] public float[] leftAttackRates;

    //[Tooltip("群攻技能主目标右次目标个数")]
    //public int rightAttackNum = 0;
    private int preRightAttackNum = -1;
    [Tooltip("群攻技能主目标右次目标伤害比例")] public float[] rightAttackRates;

    [Tooltip("该技能添加的buff")] public List<BaseBuff> buffs;
    [Tooltip("该技能给自身添加的buff")] public List<BaseBuff> selfBuffs;

    private PlayableAsset asset;

    public PlayableAsset Asset
    {
        get => asset;
        set { asset = value; }
    }

    public virtual bool SkillCondition(BaseCharacter initiator, BaseCharacter target)
    {
        return true;
    }

    public virtual void OnAttack(BaseCharacter initiator, BaseCharacter target, float damage, bool critical,
        bool isFirst, bool isFinal,bool isDamage, float initiatorNP, float targetNP,bool isMainTarget)
    {
        
        ApplyAttack(initiator,target,damage,critical,isFirst,isFinal,isDamage,initiatorNP,targetNP,isMainTarget);
        
    }

    public virtual void ApplyAttack(BaseCharacter initiator, BaseCharacter target, float damage, bool critical,
        bool isFirst, bool isFinal,bool isDamage, float initiatorNP, float targetNP,bool isMainTarget)
    {
        if (isFirst)
        {
            ApplyBuff(initiator, target);
            ApplySelfBuff(initiator, initiator);
        }
        if (isDamage)
        {
            ApplyDamage(initiator, target, damage, critical, isFinal);
            if (isMainTarget && isFinal)
            {
                initiator.afterAttackAction(initiator, target, this);
            }
            
        }
        ApplyNP(initiator,target,initiatorNP,targetNP);
    }

    

    public virtual void ApplyDamage(BaseCharacter initiator, BaseCharacter target, float damage, bool critical,
        bool isFinal)
    {
        target.GetDamage(initiator, this, damage, isFinal, damageType, critical);
    }

    public virtual void ApplyNP(BaseCharacter initiator, BaseCharacter target, float initiatorNP, float targetNP)
    {
        if (initiatorNP != 0)
        {
            initiator.GetNp(initiatorNP);
        }
        if (targetNP != 0)
        {

            target.GetNp(targetNP);
        }
    }

    public virtual void ApplyBuff(BaseCharacter initiator, BaseCharacter target)
    {
        if (buffs != null)
        {
            foreach (var buff in buffs)
            {
                if (buff != null)
                {
                    BaseBuff buffInstance = Instantiate(buff);
                    buffInstance.AddBuffToTarget(initiator, target);
                }
            }
        }
    }

    public virtual void ApplySelfBuff(BaseCharacter initiator, BaseCharacter target)
    {
        if (selfBuffs != null)
        {
            foreach (var buff in selfBuffs)
            {
                if (buff != null)
                {
                    BaseBuff buffInstance = Instantiate(buff);
                    buffInstance.AddBuffToTarget(initiator, target);
                }
            }
        }
    }

    /// <summary>
    /// 用于手动触发（一些技能可能没有动画，如切换状态）
    /// </summary>
    public virtual void ManualApply(BaseCharacter initiator, BaseCharacter target)
    {
        
    }


#if UNITY_EDITOR



    private void OnValidate()
    {
        if (prehitNum != hitNum)
        {
            float avg = 1.0f / hitNum;
            hitsRate = new float[hitNum];
            NPHitsRate= new float[hitNum];
            for (int i = 0; i < hitNum; i++)
            {
                hitsRate[i] = avg;
                NPHitsRate[i] = avg;
            }

            prehitNum = hitNum;
        }

        if (preLeftAttackNum != multipleTarget.LeftNum)
        {
            leftAttackRates = new float[multipleTarget.LeftNum];
            for (int i = 0; i < multipleTarget.LeftNum; i++)
            {
                leftAttackRates[i] = 1.0f;
            }

            preLeftAttackNum = multipleTarget.LeftNum;

        }

        if (preRightAttackNum != multipleTarget.RightNum)
        {
            rightAttackRates = new float[multipleTarget.RightNum];
            for (int i = 0; i < multipleTarget.RightNum; i++)
            {
                rightAttackRates[i] = 1.0f;
            }

            preRightAttackNum = multipleTarget.RightNum;
        }
    }
#endif
    

}
