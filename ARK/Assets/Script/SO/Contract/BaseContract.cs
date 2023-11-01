using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public abstract class BaseContract : ScriptableObject
{
    [Tooltip("危机合约ID"),HideInInspector] public int contractID;

    //TODO:根据ID加载
    //[Tooltip("技能图标")] public Sprite icon;
    [HideInInspector]
    public ContractStruct contractStruct;

    public SingleContract singleContract;

    public void Init(ContractStruct _contractStruct)
    {
        contractID = _contractStruct.id;
        contractStruct = _contractStruct;
    }
    
    
    public virtual void ApplyContract(BaseCharacter c)
    {

        if (!singleContract.contractCondition.special)
        {
            if (singleContract.contractCondition.ids.Length == 0)
            {
                if (singleContract.contractCondition.optionalType.Length != 0)
                {
                    if (!singleContract.contractCondition.optionalType.Contains(c.CharacterDataStruct.characterCamp))
                    {
                        return;
                    }
                }
                if (singleContract.contractCondition.characterClasses.Length != 0)
                {
                    if(!singleContract.contractCondition.characterClasses.Contains(
                           (c.CharacterDataStruct.characterClass)))
                    {
                        return;
                    }

                }
            }
            else
            {
                if (!singleContract.contractCondition.ids.Contains(
                        (c.CharacterDataStruct.id)))
                {
                    return;
                }
            }

        }
        foreach (var buff in singleContract.buffs)
        {
            BaseBuff instance = Instantiate(buff);
            instance.AddBuffToTarget(c,c);
        }
        

    }
    
    
    
}
