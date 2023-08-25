using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class Attack_PreMoveAsset : PlayableAsset
{
    public Vector3 target;
    public Transform move; //移动的物体
    public bool flipped = false;
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<Attack_Move>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.flipped = flipped;
        behaviour.target = target;
        if (move == null)
        {
            behaviour.owner = go.transform;
        }
        else
        {
            behaviour.owner = move;
        }

        return playable;
    }
}
