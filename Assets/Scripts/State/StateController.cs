using State;

public class StateController
{
    public StateModel Model { get; private set; }
    public StateView View { get; private set; }

    public StateController(StateModel model, StateView view)
    {
        this.Model = model;
        this.View = view;

        this.Model.OnStateChanged += OnStateChanged;
    }

    private void OnStateChanged(StateUpdate nextState)
    {
        this.View.SetState(nextState);
    }

    public void SetState(StateUpdate nextState)
    {
        this.Model.GameState = nextState;
    }
}
