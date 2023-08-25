using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class Spawn : PlayableBehaviour
{
    //TODO:动画播放完会错位
    public Vector3 target=Vector3.zero;
    public Transform owner;
    private float distance;
    private float speed;
    public double time;
    private float x;
    private float z;
    private float movedistance; //每帧移动距离

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //owner.localPosition = new Vector3(0, 5, 0);
        distance = Mathf.Abs(owner.localPosition.y - target.y);
        time=playable.GetDuration();
        //Debug.log(time);
        speed = distance / (float)time;
        //Debug.log(distance);
        //Debug.log(speed);
        
    }
    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        movedistance = speed * info.deltaTime;
        var y = owner.localPosition.y - movedistance;
        owner.localPosition = new Vector3(  x,Math.Max(y, target.y), z);
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
       
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        Debug.Log("停止？");
        owner.localPosition = Vector3.zero;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
      
    }
}
