using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class SpawnAsset : PlayableAsset
{
    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<Spawn>.Create(graph);
        var behaviour = playable.GetBehaviour();
        behaviour.owner = go.transform;
        return playable;
    }
}
