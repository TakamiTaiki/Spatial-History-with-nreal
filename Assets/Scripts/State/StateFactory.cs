using UnityEngine;

public class StateFactory : MonoBehaviour
{
    public StateController Controller { get; private set; }
    private StateModel model;
    [SerializeField] private StateView view;
    private void Awake()
    {
        this.model = new StateModel();
        this.Controller = new StateController(model, view);
    }
}
