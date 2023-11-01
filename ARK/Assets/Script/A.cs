using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Spine.Unity.Playables;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using AnimationState = Spine.AnimationState;

public class A : MonoBehaviour
{
    private UnityEvent action;
    public void Start()
    {

        action = new UnityEvent();
        action.AddListener(S);
        action.Invoke();
    }

    public void Update()
    {
       
    }

    public void FixedUpdate()
    {
        
    }

    public void S()
    {
        
    }
}
