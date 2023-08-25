using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using AnimationState = Spine.AnimationState;

public class B : MonoBehaviour
{
    public GameObject go;
    private PlayableDirector director;
    private AnimationState animationState;
    private Vector3 startPos;
    private PlayableAsset attack;
    private PlayableAsset skill;
    private bool IsAttack = true;

    private void Awake()
    {
        //Debug.log(Time.frameCount);
    }

    private void Start()
    {
        //Debug.log(Time.frameCount);
        attack = Instantiate(Resources.Load<PlayableAsset>("Timelines/BattleCharacter/Amiya_0/Attack"));
        skill = Instantiate(Resources.Load<PlayableAsset>("Timelines/BattleCharacter/Amiya_0/Skill"));
        animationState = GetComponent<SkeletonAnimation>().AnimationState;
        director = GetComponent<PlayableDirector>();
        director.stopped += Stop;
        startPos = transform.position;
        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0,"Idle",true);
        director.playableAsset = attack;
        PlayableBinding? pd = DirectorExtend.GetPlayableBinding(director, "AA");
        director.SetGenericBinding(pd.Value.sourceObject,gameObject.GetComponent<SkeletonAnimation>());


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayableBinding? pd1 = DirectorExtend.GetPlayableBinding(director, "BB");
            object go1 = pd1.Value.sourceObject;
            PlayableTrack track=go1 as PlayableTrack;
            foreach (var clip in track.GetClips())
            {
                
                if (clip.displayName=="MoveBeforeAttack")
                {
                    //Debug.log("set pos!");
                    (clip.asset as Attack_PreMoveAsset).target = go.transform.position;
                    
                }
                if (clip.displayName=="MoveAfterAttack")
                {
                    //Debug.log("set pos!");
                    (clip.asset as Attack_PreMoveAsset).target = startPos;
                    
                }

            }
            director.Play();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (IsAttack)
            {
                director.playableAsset = skill;
                PlayableBinding? pd = DirectorExtend.GetPlayableBinding(director, "AA");
                director.SetGenericBinding(pd.Value.sourceObject, gameObject.GetComponent<SkeletonAnimation>());
                IsAttack = false;
            }
            else
            {
                director.playableAsset = attack;
                PlayableBinding? pd = DirectorExtend.GetPlayableBinding(director, "AA");
                director.SetGenericBinding(pd.Value.sourceObject, gameObject.GetComponent<SkeletonAnimation>());
                IsAttack = true;
            }
            
        }
    }
    public void Stop(PlayableDirector director)
    {
        //Debug.log("Stop");
        GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0,"Idle",true);
    }
}
