using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;




[Serializable]

public abstract class BaseBuff : ScriptableObject
{
    /// <summary>
    /// BUFF的创建者
    /// </summary>
    protected BaseCharacter initiator;
    /// <summary>
    /// BUFF的目标
    /// </summary>
    protected BaseCharacter target;

    [Tooltip("buff图标")]
    public Sprite icon;

    [Tooltip("挂图标的物体"),HideInInspector]
    public GameObject iconImage;

    public int buffID;
    [Tooltip("该buff是否永久？")]
    public bool isPermanent=false;
    public BuffType buffType;
    [Tooltip("该buff是否唯一？只能在同一目标同时存在一个")]
    public bool only=true;

    [Tooltip("该buff最大层数")]
    public int maxLayers = 1;

    private int curLayers = 0;

    [Tooltip("自己给自己加buff，当回合不减少回合数")]
    private bool selfBuff=false;

    public int CurLayers
    {
        get => curLayers;
        set
        {
            if (value > maxLayers)
            {
                curLayers = maxLayers;
            }
            else
            {
                curLayers = value;
            }
        }
    }
    //当only=true,maxlayers=1时，重新获得buff会刷新其时间。当only=true，maxlayers>1时，重新获得buff会增加层数并刷新时间。
    //当only=false时，每个buff独立计算时间。
    
    [Tooltip("该buff随着角色回合减少还是随着整体回合减少")]
    public BuffTurnType buffTurnType;

    [Tooltip("该buff持续的回合")]
    public int durationTurns=1;
    protected int remainTurns=1;

    public int RemainTurns
    {
        get => remainTurns;
    }

    public bool IsFinish
    {
        get
        {
            if (isPermanent) return false;
            if (remainTurns <= 0)
            {
                return true;
            }

            return false;
        }
    }

    public SingleBuff[] singleBuffs;
    public DamageBuff[] damageBuffs;
    
    public virtual void BuffReset()
    {
        curLayers = 0;
        remainTurns = durationTurns;
        initiator = null;
        target = null;
    }

    public virtual void ResetRemain(int remainTurn)
    {
        remainTurns = remainTurn>=remainTurns?remainTurn: remainTurns;
    }

    public virtual void AddBuffToTarget(BaseCharacter _initiator,BaseCharacter _target)
    {
        CurLayers += 1;
        remainTurns = durationTurns;
        initiator = _initiator;
        target = _target;
        if (initiator == target) selfBuff = true;
        if (only)
        {
            BaseBuff exist = _target.HasBuff(buffID);
            if (exist!=null)
            {
                
                if (maxLayers == 1)
                {
                    //_target.RemoveBuffByID(buffID);
                    //_target.AddBuff(this);
                    //ChangeTargetProperties();
                    exist.ResetRemain(remainTurns);
                    exist.ResetTargetProperties();
                    exist.ChangeTargetProperties();
                    //Debug.Log(singleBuffs[0].temp);
                }
                else
                {
                    exist.ResetTargetProperties();
                    exist.AddLayer();
                    exist.ChangeTargetProperties();
                }

                return;
            }

        }

        ChangeTargetProperties();
        _target.AddBuff(this);
        UpdateLayer();
    }
    
    /// <summary>
    /// 我的回合开始时调用
    /// </summary>
    public virtual void MyTurnStart()
    {
        if (initiator != null && target != null)
        {
            foreach (var damageBuff in damageBuffs)
            {
                target.GetDamage(initiator, null, DamageBuffCal(damageBuff), true, damageBuff.damageType, false);
            }
        }
    }
    
    /// <summary>
    /// 攻击前调用，用于持续性伤害buff
    /// </summary>
    public virtual void BeforeAttack(BaseCharacter _initiator,BaseCharacter _target,BaseSkill skill)
    {

    }
    /// <summary>
    /// final attack后调用
    /// </summary>
    public virtual void AfterAttack(BaseCharacter _initiator,BaseCharacter _target,BaseSkill skill)
    {


    }
    
    /// <summary>
    /// 角色回合结束后调用
    /// </summary>
    /// <param name="_initiator"></param>
    /// <param name="_target"></param>
    /// <param name="skill"></param>

    public virtual void MyTurnEnd(BaseCharacter _initiator,BaseCharacter _target,BaseSkill skill)
    {
        if (!isPermanent)
        {
            remainTurns -= 1;
        }
    }

    /// <summary>
    /// 玩家回合结束后调用
    /// </summary>
    /// <param name="_initiator"></param>
    /// <param name="_target"></param>
    /// <param name="skill"></param>
    public virtual void PlayerTurnEnd(BaseCharacter _initiator,BaseCharacter _target,BaseSkill skill)
    {
        
    }
    
    

    public virtual float DamageBuffCal(DamageBuff damageBuff)
    {
        
        return (DamageCal.CalDamage(DamageCal.GetValueByAttribute(initiator.BattleCharacterStateData,damageBuff.baseAttribute),initiator.BattleCharacterStateData, target.BattleCharacterStateData, damageBuff.damageRate,
            damageBuff.damageType, false) + damageBuff.fixedDamage)*curLayers;
    }

    public virtual void ChangeTargetProperties()
    {
        for (int i = 0; i < singleBuffs.Length; i++)
        {
            if (singleBuffs[i].targetProperty != DataProperty.CurPos&&singleBuffs[i].targetProperty != DataProperty.Cost)
            {
                //var singleBuff = singleBuffs[i];
                Type baseCharacterType = typeof(CharacterStateData);
                if (singleBuffs[i].modified_per != 0)
                {

                    //获得基于创建者还是目标的某个属性
                    if (singleBuffs[i].numericalFundamental == NumericalFundamental.Initiator)
                    {
                        //基于buff创建者提升数值
                        /*singleBuffs[i].temp = (
                            (float)basePropertyInfo.GetValue(initiator.CharacterStateData) *
                            singleBuffs[i].modified_per +
                            singleBuffs[i].modified) * CurLayers;*/
                        singleBuffs[i].temp=(DamageCal.GetValueByAttribute(initiator.CharacterStateData,singleBuffs[i].initiatorProperty)*
                                            singleBuffs[i].modified_per +
                                            singleBuffs[i].modified) * CurLayers;
                    }
                    else
                    {
                        //基于buff接受者提升数值
                        /*singleBuffs[i].temp = (
                            (float)basePropertyInfo.GetValue(target.CharacterStateData) * singleBuffs[i].modified_per +
                            singleBuffs[i].modified) * CurLayers;*/
                        singleBuffs[i].temp=(DamageCal.GetValueByAttribute(target.CharacterStateData,singleBuffs[i].initiatorProperty)*
                                             singleBuffs[i].modified_per +
                                             singleBuffs[i].modified) * CurLayers;
                    }
                }
                else
                {
                    singleBuffs[i].temp = singleBuffs[i].modified * CurLayers;
                }

                if (singleBuffs[i].targetProperty == DataProperty.NP)
                {
                    target.GetNp(singleBuffs[i].temp);
                }
                else
                {
                    /*PropertyInfo targetPropertyInfo =
                        baseCharacterType.GetProperty(singleBuffs[i].targetProperty.ToString());
                    targetPropertyInfo?.SetValue(target.BattleCharacterStateData,
                        (float)(targetPropertyInfo.GetValue(target.BattleCharacterStateData)) + singleBuffs[i].temp);*/
                    float originalTargetAttribute = DamageCal.GetValueByAttribute(target.BattleCharacterStateData,
                        singleBuffs[i].targetProperty);
                    DamageCal.SetValueByAttribute(target.BattleCharacterStateData, singleBuffs[i].targetProperty,
                        originalTargetAttribute + singleBuffs[i].temp);
                    
                    if (singleBuffs[i].targetProperty == DataProperty.Speed)
                    {
                        BattleUISystem.Instance.UpdateAfterRun();
                    }
                }
            }
            else if(singleBuffs[i].targetProperty == DataProperty.CurPos)
            {

                    //距离
                    Type runnerType = typeof(Runner);
                    PropertyInfo targetEndPos =
                        runnerType.GetProperty("EndPos");
                    PropertyInfo targetPropertyInfo =
                        runnerType.GetProperty(singleBuffs[i].targetProperty.ToString());
                    float moveDistance = (float)targetEndPos.GetValue(target.runner) * singleBuffs[i].modified_per +
                                         singleBuffs[i].modified;
                    targetPropertyInfo.SetValue(target.runner,
                        (float)targetPropertyInfo.GetValue(target.runner) + moveDistance);
                    BattleUISystem.Instance.UpdateAfterRun();
                    
            }
            else if(singleBuffs[i].targetProperty == DataProperty.Cost)
            {
                BattleSystem.Instance.Cost += (int)singleBuffs[i].modified;
            }
        }
    }
    public virtual void ResetTargetProperties()
    {
        if (initiator!=null && target!=null)
        {
            for (int i = 0; i < singleBuffs.Length; i++)
            {
                // var singleBuff = singleBuffs[i];
                if (singleBuffs[i].targetProperty == DataProperty.CurPos ||
                    singleBuffs[i].targetProperty == DataProperty.HP ||
                    singleBuffs[i].targetProperty == DataProperty.NP ||
                    singleBuffs[i].targetProperty == DataProperty.Cost)
                {
                }
                else
                {

                    /*Type baseCharacterType = typeof(CharacterStateData);
                    PropertyInfo targetPropertyInfo =
                        baseCharacterType.GetProperty(singleBuffs[i].targetProperty.ToString());
                    Debug.Log(singleBuffs[i].temp);
                    targetPropertyInfo?.SetValue(target.BattleCharacterStateData,
                        (float)(targetPropertyInfo.GetValue(target.BattleCharacterStateData)) - singleBuffs[i].temp);*/
                    float curTargetAttribute =
                        DamageCal.GetValueByAttribute(target.BattleCharacterStateData, singleBuffs[i].targetProperty);
                    DamageCal.SetValueByAttribute(target.BattleCharacterStateData, singleBuffs[i].targetProperty,
                        curTargetAttribute - singleBuffs[i].temp);
                    if (singleBuffs[i].targetProperty == DataProperty.Speed)
                    {
                        BattleUISystem.Instance.UpdateAfterRun();
                    }
                }
            }
        }
    }
    

    public virtual void BuffRemove()
    {
        ResetTargetProperties();
        BuffReset();
        iconImage = null;
        //TODO:BuffPool
    }

    public virtual void AddLayer()
    {
        remainTurns = durationTurns;
        CurLayers += 1;
        UpdateLayer();

    }

    public virtual void UpdateLayer()
    {
        if (iconImage&& CurLayers>1)
        {
            iconImage.GetComponent<BuffLayer>().SetNum(CurLayers);
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        remainTurns = durationTurns;
    }
#endif





}
