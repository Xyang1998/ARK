using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public static class DirectorExtend 
{
    public static PlayableBinding? GetPlayableBinding(PlayableDirector playableDirector, string trackName)
    {
        foreach (PlayableBinding pb in playableDirector.playableAsset.outputs)//每次都用循环获取 这里性能可能会有影响
        {
            if (pb.streamName.Equals(trackName))
            {
                return pb;
            }
        }

        return null;
    }


}
