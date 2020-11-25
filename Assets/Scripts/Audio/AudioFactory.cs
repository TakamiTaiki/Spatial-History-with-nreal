using UnityEngine;

public class AudioFactory : MonoBehaviour
{
    public AudioController Controller { get; private set; }
    public AudioModel Model { get; private set; }
    [SerializeField] private AudioView view_SE;
    [SerializeField] private AudioView view_Voice;

    private void Start()
    {
        this.Model = new AudioModel();
        this.Controller = new AudioController(Model, view_SE, view_Voice);
    }
}
