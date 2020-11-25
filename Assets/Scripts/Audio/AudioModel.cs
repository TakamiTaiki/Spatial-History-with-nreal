using UnityEngine;
public class AudioModel
{
    public delegate void AudioEvent(AudioClip clip);
    public event AudioEvent OnAudioSEChanged;
    public event AudioEvent OnAudioVoiceChanged;
    public AudioClip Clip_Select { get; private set; }
    public AudioClip Clip_PreSelect { get; private set; }
    public AudioClip Clip_worldMake { get; private set; }
    public AudioClip Clip_Intro { get; private set; }
    public AudioClip Clip_OpeExplain { get; private set; }

    public AudioModel()
    {
        Clip_Select      = Resources.Load<AudioClip>("Audio/SE/Select");
        Clip_PreSelect   = Resources.Load<AudioClip>("Audio/SE/Select_pre");
        Clip_worldMake   = Resources.Load<AudioClip>("Audio/SE/WorldMake");
        Clip_Intro       = Resources.Load<AudioClip>("Audio/Voice/Intro");
        Clip_OpeExplain  = Resources.Load<AudioClip>("Audio/Voice/OpeExplain");
    }

    private AudioClip clip_SE;
    public AudioClip Clip_SE
    {
        get { return clip_SE; }
        set
        {
            clip_SE = value;
            OnAudioSEChanged?.Invoke(value);
        }
    }

    private AudioClip clip_Voice;
    public AudioClip Clip_Voice
    {
        get { return clip_Voice; }
        set
        {
            clip_SE = value;
            OnAudioVoiceChanged?.Invoke(value);
        }
    }
}
