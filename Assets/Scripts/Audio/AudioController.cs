using System;
using UnityEngine;

public class AudioController
{
    public AudioModel Model { get; private set; }
    public AudioView View_SE { get; private set; }
    public AudioView View_Voice { get; private set; }

    public AudioController(AudioModel model, AudioView view_SE, AudioView view_Voice)
    {
        this.Model = model;
        this.View_SE = view_SE;
        this.View_Voice = view_Voice;

        this.Model.OnAudioSEChanged += OnAudioSEChanged;
        this.Model.OnAudioVoiceChanged += OnAudioVoiceChanged;
    }

    private void OnAudioSEChanged(AudioClip clip)
    {
        this.View_SE.Play(clip);
    }

    private void OnAudioVoiceChanged(AudioClip clip)
    {
        this.View_Voice.Play(clip);
    }

    public void SetAudioSE(AudioClip clip)
    {
        this.Model.Clip_SE = clip;
    }

    public void SetAudioVoice(AudioClip clip)
    {
        this.Model.Clip_Voice = clip;
    }
}
