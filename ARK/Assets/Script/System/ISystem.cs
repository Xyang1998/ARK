using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISystem : MonoBehaviour
{
    protected SystemMediator _systemMediator;

    public void setMediator(SystemMediator systemMediator)
    {
        _systemMediator = systemMediator;
    }

    public abstract void Init();


    public abstract void Tick();

}