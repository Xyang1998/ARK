using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "BaseCharacterState",menuName = "ScriptableObject/NewBaseCharacterState")]
public class BaseCharacterState : ScriptableObject
{
    [SerializeField] private CharacterStateData characterStateData;
    /// <summary>
    /// 多血条？
    /// </summary>
    [SerializeField]
    public List<float> hpList;
    
    

    public CharacterStateData CharacterStateData
    {
        get => characterStateData;
    }
    
    public BaseAI AI;
    
    






}
