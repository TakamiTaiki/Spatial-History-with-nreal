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
}