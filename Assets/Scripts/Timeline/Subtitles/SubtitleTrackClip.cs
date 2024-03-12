using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleTrackClip : PlayableAsset
{
    public string subtitleText;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SubtitleTrackBehaviour>.Create(graph);

        SubtitleTrackBehaviour subtitleTrackBehaviour = playable.GetBehaviour();
        subtitleTrackBehaviour.subtitleText = subtitleText;

        return playable;
    }
}
