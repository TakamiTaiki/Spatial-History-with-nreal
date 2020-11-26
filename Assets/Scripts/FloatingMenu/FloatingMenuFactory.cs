using UnityEngine;

public class FloatingMenuFactory : MonoBehaviour
{
    public FloatingMenuController Controller { get; private set; }
    private FloatingMenuModel model;
    [SerializeField] private FloatingMenuView view;

    private void Awake()
    {
        model = new FloatingMenuModel();
        Controller = new FloatingMenuController(model, view);
    }
}
