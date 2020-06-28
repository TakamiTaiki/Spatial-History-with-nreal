using UnityEngine;

namespace NRKernal.NRExamples
{
    [DisallowMultipleComponent]
    public class AppManager : MonoBehaviour
    {
        private void OnEnable()
        {
            NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Left, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Right, ControllerButton.APP, OnAppButtonClick);
            NRInput.AddClickListener(ControllerHandEnum.Left, ControllerButton.APP, OnAppButtonClick);
        }

        private void OnDisable()
        {
            NRInput.RemoveClickListener(ControllerHandEnum.Right, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.RemoveClickListener(ControllerHandEnum.Left, ControllerButton.HOME, OnHomeButtonClick);
            NRInput.RemoveClickListener(ControllerHandEnum.Right, ControllerButton.APP, OnAppButtonClick);
            NRInput.RemoveClickListener(ControllerHandEnum.Left, ControllerButton.APP, OnAppButtonClick);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Escape))
                QuitApplication();
#endif
        }

        private void OnHomeButtonClick()
        {
            NRHomeMenu.Toggle();
        }

        private void OnAppButtonClick()
        {
            NRHomeMenu.Hide();
        }

        public static void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            NRDevice.Instance.QuitApp();
#endif
        }
    }
}
