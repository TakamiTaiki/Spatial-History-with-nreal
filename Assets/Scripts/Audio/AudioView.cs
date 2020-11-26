using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioView : MonoBehaviour
{
    private AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    public void Play(AudioClip clip)
    {
        audio.PlayOneShot(clip);
    }

    public bool IsPlaying()
    {
        return audio.isPlaying;
    }

    public void Stop()
    {
        audio.Stop();
    }
}