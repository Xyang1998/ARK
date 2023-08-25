using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class Attack_Move : PlayableBehaviour
{
    //TODO:动画播放完会错位
    public Vector3 target;
    public Transform owner;
    public bool flipped;
    private float distance;
    private float speed;
    public double time;
    private float y;
    private float z;
    private float movedistance; //每帧移动距离

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log("be begin");
        Vector3 temp = owner.position;
        distance = Mathf.Abs(temp.x - target.x);
        y = temp.y;
        z = temp.z;
        time=playable.GetDuration();
        //Debug.log(time);
        speed = distance / (float)time;
        //Debug.log(distance);
        //Debug.log(speed);
        
    }
    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {

    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        movedistance = speed * info.deltaTime*(flipped?-1:1);
        var x = owner.position.x + movedistance;
        owner.position = new Vector3( flipped ? Math.Max(x, target.x) : Math.Min(x, target.x), y, z);
    }

    
}
