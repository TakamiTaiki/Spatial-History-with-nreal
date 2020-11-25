using UnityEngine;

public class ExplainTextFactory : MonoBehaviour
{
    public ExplainTextController Controller { get; private set; }
    private TextModel model;
    [SerializeField] private TextView view_Text;
    [SerializeField] private TextIndicatorView view_Indicator;
    private void Start()
    {
        this.model = new TextModel();
        this.Controller = new ExplainTextController(model, view_Text, view_Indicator);
    }
}
