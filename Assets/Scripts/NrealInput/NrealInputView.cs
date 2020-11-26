using UnityEngine;
using NRKernal;

public class NrealInputView : MonoBehaviour
{
    [SerializeField] private MainManager manager;
    [SerializeField] private StateFactory state;
    public NRPointerRaycaster raycaster { get; private set; }

    private void Start()
    {
        raycaster = GameObject.Find("/NRInput/Right/ControllerTracker/LaserRaycaster").GetComponent<NRPointerRaycaster>();
    }

    public void OnTriggerButtonClicked()
    {
        #region 単純クリック
        if (state.Controller.Model.GameState == manager.ST_Positioning)
        {
            state.Controller.SetState(manager.ST_SetPosition);
        }
        else if (state.Controller.Model.GameState == manager.ST_FirstHalfIntroduction)
        {
            state.Controller.SetState(manager.ST_LatterHalfIntroduction);
        }
        #endregion

        #region オブジェクトクリック
        GameObject target = GetRaycastHit();
        if (target == null) return;

        string tag = target.tag;

        if (state.Controller.Model.GameState == manager.ST_Standby)
        {
            if (tag == "Start")
            {
                manager.OnStartButtonClicked();
            }
        }
        else if (state.Controller.Model.GameState == manager.ST_Play)
        {
            if (tag == "NextWaitNode")
            {
                manager.RunAnimation_Auto();
            }
        }
        else if (state.Controller.Model.GameState == manager.ST_FreeChoice)
        {
            if (tag == "FreeChoiceNode")
            {
                manager.RunAnimation_Free();
            }
            else if (tag == "Reboot")
            {
                manager.OnRebootButtonClicked();
            }
        }
        #endregion
    }
    public void OnTriggerButtonPressing()
    {
        GameObject target = GetRaycastHit();
        if (target == null) return;

        string tag = target.tag;

        if (tag == "Gizmo")
        {
            manager.MoveContent();
        }
    }
    public void OnHomeButtonClicked()
    {

    }
    public void OnAppButtonClicked()
    {

    }

    public Vector3 GetRayTipPosition()
    {
        return raycaster.transform.position + raycaster.transform.forward;
    }

    public GameObject GetRaycastHit()
    {
        raycaster.Raycast();
        return raycaster.FirstRaycastResult().gameObject;
    }
}
