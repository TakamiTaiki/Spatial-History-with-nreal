using UnityEngine;

public class NrealInputFactory : MonoBehaviour
{
    public NrealInputController Controller { get; private set; }
    [SerializeField] private NrealInputView view;

    private void Awake()
    {
        Controller = new NrealInputController(view);
    }
}
