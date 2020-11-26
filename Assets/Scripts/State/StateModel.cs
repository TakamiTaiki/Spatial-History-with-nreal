using State;

namespace State
{
    public delegate void StateUpdate(bool isFirst);
}
public class StateModel
{
    public delegate void StateEvent(StateUpdate state);
    public event StateEvent OnStateChanged;

    private StateUpdate gameState;
    public StateUpdate GameState
    {
        get { return gameState; }
        set
        {
            if (gameState != value)
            {
                gameState = value;
                OnStateChanged?.Invoke(value);
            }
        }
    }
}
