using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CharacterStateData
{
    [SerializeField]
    private int id;
    public int ID
    {
        get => id;
    }

    [SerializeField]
    public bool isDead=false;

    
    [SerializeField]
    private float atk = 100;
    public float ATK
    {
        get => atk;
        set
        {
            if (value < 0)
            {
                atk = 0;
            }
            else
            {
                atk = value;
            }
        }
    }
    
    [SerializeField]
    private float hp=100;
    public float HP
    {
        get => hp;
        set
        {
            
            if (value <= 0)
            {
                hp = 0;
            }
            else if(value>=maxHp)
            {
                hp = maxHp;
            }
            else
            {
                hp = value;
            }
            OnHPChanged.Invoke();
        }
    }

    public UnityAction OnHPChanged = new UnityAction(()=>{});

    [SerializeField]
    private float maxHp=100;
    public float MaxHP
    {
        get => maxHp;
        set
        {
            if (value < 1)
            {
                if (maxHp > value)
                {
                    HP = Math.Clamp(HP, 1, value);
                }
                maxHp = 1;
            }
            else
            {
                if (maxHp > value)
                {
                    HP = Math.Clamp(HP, 1, value);
                }
                maxHp = value;
            }
            OnHPChanged.Invoke();
        }
    }
    
    [SerializeField]
    private float np=0;
    public float NP
    {
        get => np;
        set
        {
            if (value >= MaxNP - 0.5f)
            {
                np = MaxNP;
            }
            else
            {
                np = value;
            }
            OnNPChanged.Invoke();
        }
    }
    public UnityAction OnNPChanged = new UnityAction(()=>{});
    
    [SerializeField]
    private float maxNp=100;
    public float MaxNP
    {
        get => maxNp;
        set
        {
            if (value < 1)
            {
                maxNp = 1;
            }
            else
            {
                maxNp = value;
            }
            OnNPChanged.Invoke();
        }
        
    }
    
    [SerializeField]
    [Tooltip("大于0")]
    private float speed=10;
    public float Speed
    {
        get => speed;
        set
        {
            if (value < 1)
            {
                speed = 1;
            }
            else
            {
                speed = value;
            }
        }
    }
    
    [SerializeField]
    [Tooltip("大于0")]
    private float defense=10;
    public float Defense
    {
        get => defense;
        set
        {
            if (value < 0)
            {
                defense = 0;
            }
            else
            {
                defense = value;
            }
        }
    }
    
    [SerializeField]
    [Tooltip("0-100%")]
    private float magicDefense=10;
    public float MagicDefense
    {
        get => magicDefense;
        set
        {
            if (value < 0)
            {
                magicDefense = 0;
            }
            else if (value > 100)
            {
                magicDefense = 100;
            }
            else
            {
                magicDefense = value;
            }
        }
    }
    
    [SerializeField]
    [Tooltip("%")]
    private float critRate=5;
    public float CritRate
    {
        get => critRate;
        set
        {
            critRate = value;
        }
    }
    
    [SerializeField]
    [Tooltip("%")]
    private float criticalDamage=50;
    public float CriticalDamage
    {
        get => criticalDamage;
        set{
            if (value < 0)
            {
                criticalDamage = 0;
            }
            else
            {
                criticalDamage = value;
            }
        }
    }
    
    [SerializeField]
    [Tooltip("治疗率，%,[-100,无穷)")]
    private float healing=0;
    public float Healing
    {
        get => healing;
        set
        {
            if (value < -100)
            {
                healing = -100;
            }
            else
            {
                healing = value;
            }
        }
    }
    
    [SerializeField]
    [Tooltip("效果命中,%")]
    private float effectHitRate=0;
    public float EffectHitRate
    {
        get => effectHitRate;
        set
        {
            effectHitRate = value;

        }
    }
    
    [SerializeField]
    [Tooltip("效果抵抗,%")]
    private float effectResistanceRate=0;
    public float EffectResistanceRate
    {
        get => effectResistanceRate;
        set
        {
            effectResistanceRate = value;
        }
    }

    [SerializeField]
    [Tooltip("NP率,%,[0,无穷)")]
    private float npRate = 100f;
    public float NPRate
    {
        get => npRate;
        set
        {
            if (value < 0)
            {
                npRate = 0;
            }
            else
            {
                npRate = value;
            }
        }
    }

    [SerializeField]
    [Tooltip("嘲讽基础值，(0,无穷)")]
    private float tauntValue = 100;
    public float TauntValue
    {
        get => tauntValue;
        set
        {
            if (value < 0)
            {
                tauntValue = 0.1f;
                
            }
            else
            {
                tauntValue = value;
            }
        }
    }
    
    [SerializeField]
    [Tooltip("[0,无穷)%")]
    private float damageRate = 100.0f; //造成伤害的倍率
    public float DamageRate
    {
        get => damageRate;
        set {
            if (value < 0)
            {
                damageRate = 0;
            }
            else
            {
                damageRate = value;
            }
        }
    }
    
    [SerializeField]
    [Tooltip("[0,无穷)%")]
    private float getDamageRate = 100f; //收到伤害的倍率
    public float GetDamageRate
    {
        get => getDamageRate;
        set
        {
            if (value < 0)
            {
                getDamageRate = 0;
            }
            else
            {
                getDamageRate = value;
            }
        }
    }
    
    [SerializeField]
    private float actionNum = 1;

    public float ActionNum
    {
        get => actionNum;
    }

 
    public CharacterStateData ToBattleState()
    {
        return  DeepCopy.DeepCopyByReflect<CharacterStateData>(this);
    }
    
    public void ToWorldState(float _hp, float _np)
    {
        HP = _hp;
        NP = _np;
    }
}