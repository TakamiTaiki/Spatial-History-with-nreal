using UnityEngine;

public class AudioFactory : MonoBehaviour
{
    public AudioController Controller { get; private set; }
    private AudioModel model;
    [SerializeField] private AudioView view_SE;
    [SerializeField] private AudioView view_Voice;

    private void Awake()
    {
        this.model = new AudioModel();
        this.Controller = new AudioController(model, view_SE, view_Voice);
    }
}
