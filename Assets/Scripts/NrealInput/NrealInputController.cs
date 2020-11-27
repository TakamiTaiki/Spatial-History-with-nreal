using NRKernal;

public class NrealInputController
{
    public NrealInputView View { get; private set; }

    public NrealInputController(NrealInputView view)
    {
        this.View = view;

        NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.TRIGGER, OnTriggerButtonClicked);
        NRInput.AddPressingListener(ControllerHandEnum.Right, ControllerButton.TRIGGER, OnTriggerButtonPressing);
        NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.HOME, OnHomeButtonClicked);
        NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.APP, OnAppButtonClicked);
    }

    private void OnTriggerButtonClicked()
    {
        this.View.OnTriggerButtonClicked();
    }
    private void OnTriggerButtonPressing()
    {
        this.View.OnTriggerButtonPressing();
    }
    private void OnHomeButtonClicked()
    {
        this.View.OnHomeButtonClicked();
    }
    private void OnAppButtonClicked()
    {
        this.View.OnAppButtonClicked();
    }
}
