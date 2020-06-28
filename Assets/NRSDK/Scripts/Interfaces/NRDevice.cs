/****************************************************************************
* Copyright 2019 Nreal Techonology Limited. All rights reserved.
*                                                                                                                                                          
* This file is part of NRSDK.                                                                                                          
*                                                                                                                                                           
* https://www.nreal.ai/        
* 
*****************************************************************************/

namespace NRKernal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// Manage the hmd device and quit 
    /// </summary>
    public class NRDevice : SingleTon<NRDevice>
    {
        internal NativeHMD NativeHMD { get; set; }
        private bool m_IsInit = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject m_UnityActivity;
#endif

        /// <summary>
        /// Init hmd device.
        /// </summary>
        public void Init()
        {
            if (m_IsInit)
            {
                return;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            // Init before all actions.
            AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            m_UnityActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            NativeApi.NRSDKInitSetAndroidActivity(m_UnityActivity.GetRawObject());
            
            NativeHMD = new NativeHMD();
            NativeHMD.Create();
#endif
            m_IsInit = true;
        }

        /// <summary>
        /// Quit the app.
        /// </summary>
        public void QuitApp()
        {
            Debug.Log("Start To Quit Application...");
            NRSessionManager.Instance.DestroySession();
            Application.Quit();
        }

        /// <summary>
        /// Force kill the app.
        /// </summary>
        public void ForceKill()
        {
            Debug.Log("Start To kill Application...");
            NRSessionManager.Instance.DestroySession();
#if UNITY_ANDROID && !UNITY_EDITOR
            if (m_UnityActivity != null)
            {
                m_UnityActivity.Call("finish");
            }

            AndroidJavaClass processClass = new AndroidJavaClass("android.os.Process");
            int myPid = processClass.CallStatic<int>("myPid");
            processClass.CallStatic("killProcess", myPid);
#endif
        }

        /// <summary>
        /// Destory hmd resource.
        /// </summary>
        public void Destroy()
        {
            if (NativeHMD != null)
            {
                NativeHMD.Destroy();
                NativeHMD = null;
            }
        }

        private struct NativeApi
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            [DllImport(NativeConstants.NRNativeLibrary)]
            public static extern NativeResult NRSDKInitSetAndroidActivity(IntPtr android_activity);
#endif
        }
    }
}
