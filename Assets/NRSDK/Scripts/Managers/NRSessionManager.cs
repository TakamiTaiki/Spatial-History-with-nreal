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
    using System.IO;
    using UnityEngine;
    using System.Collections;
    using NRKernal.Record;

    /// <summary>
    ///  Manages AR system state and handles the session lifecycle.
    ///  this class, application can create a session, configure it, start/pause or stop it.
    /// </summary>
    public class NRSessionManager
    {
        private static NRSessionManager m_Instance;
        private bool m_IsInitialized = false;
        private bool m_IsDestroyed = false;

        public static NRSessionManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new NRSessionManager();
                }
                return m_Instance;
            }
        }

        /// <summary>
        /// Current lost tracking reason.
        /// </summary>
        public LostTrackingReason LostTrackingReason
        {
            get
            {
                if (m_IsInitialized)
                {
                    return NativeAPI.NativeHeadTracking.GetTrackingLostReason();
                }
                else
                {
                    return LostTrackingReason.INITIALIZING;
                }
            }
        }

        public NRSessionBehaviour NRSessionBehaviour { get; private set; }

        public NRHMDPoseTracker NRHMDPoseTracker { get; private set; }

        internal NativeInterface NativeAPI { get; private set; }

        private NRRenderer NRRenderer { get; set; }

        public NRVirtualDisplayer VirtualDisplayer { get; set; }

        public bool IsInitialized
        {
            get
            {
                return m_IsInitialized;
            }
        }

        public void CreateSession(NRSessionBehaviour session)
        {
            if (IsInitialized || session == null)
            {
                return;
            }
            if (NRSessionBehaviour != null)
            {
                NRDebugger.LogError("Multiple SessionBehaviour components cannot exist in the scene. " +
                  "Destroying the newest.");
                GameObject.DestroyImmediate(session.gameObject);
                return;
            }
            NRDevice.Instance.Init();
            NativeAPI = new NativeInterface();
            NativeAPI.NativeTracking.Create();
            NRSessionBehaviour = session;

            NRHMDPoseTracker = session.GetComponent<NRHMDPoseTracker>();
            TrackingMode mode = NRHMDPoseTracker.TrackingMode == NRHMDPoseTracker.TrackingType.Tracking3Dof ? TrackingMode.MODE_3DOF : TrackingMode.MODE_6DOF;
            SetTrackingMode(mode);
            this.DeployData();
            NRKernalUpdater.Instance.StartCoroutine(OnUpdate());
        }

        private IEnumerator OnUpdate()
        {
            while (!m_IsDestroyed)
            {
                NRFrame.OnUpdate();
                yield return new WaitForEndOfFrame();
            }
        }

        private void DeployData()
        {
            if (NRSessionBehaviour.SessionConfig == null)
            {
                return;
            }
            var database = NRSessionBehaviour.SessionConfig.TrackingImageDatabase;
            if (database == null)
            {
                NRDebugger.Log("augmented image data base is null!");
                return;
            }
            string deploy_path = database.TrackingImageDataOutPutPath;
            NRDebugger.Log("[TrackingImageDatabase] DeployData to path :" + deploy_path);
            ZipUtility.UnzipFile(database.RawData, deploy_path, NativeConstants.ZipKey);
        }

        public void SetConfiguration(NRSessionConfig config)
        {
            if (config == null)
            {
                return;
            }
            NativeAPI.Configration.UpdateConfig(config);
        }

        private void SetTrackingMode(TrackingMode mode)
        {
            NativeAPI.NativeTracking.SetTrackingMode(mode);
        }

        public void Recenter()
        {
            if (!m_IsInitialized)
            {
                return;
            }
            NativeAPI.NativeTracking.Recenter();
        }

        public static void SetAppSettings(bool useOptimizedRendering)
        {
            Application.targetFrameRate = 240;
            QualitySettings.maxQueuedFrames = -1;
            QualitySettings.vSyncCount = useOptimizedRendering ? 0 : 1;
            Screen.fullScreen = true;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void StartSession()
        {
            if (m_IsInitialized)
            {
                return;
            }

            var config = NRSessionBehaviour.SessionConfig;
            if (config != null)
            {
                SetAppSettings(config.OptimizedRendering);
#if !UNITY_EDITOR
                if (config.OptimizedRendering)
                {
                    if (NRSessionBehaviour.gameObject.GetComponent<NRRenderer>() == null)
                    {
                        NRRenderer = NRSessionBehaviour.gameObject.AddComponent<NRRenderer>();
                        NRRenderer.Initialize(NRHMDPoseTracker.leftCamera, NRHMDPoseTracker.rightCamera, NRHMDPoseTracker.GetHeadPose);
                    }
                }
#endif
            }
            else
            {
                SetAppSettings(false);
            }
            NativeAPI.NativeTracking.Start();

            NativeAPI.NativeHeadTracking.Create();

#if UNITY_EDITOR
            InitEmulator();
#endif
            m_IsInitialized = true;
        }

        public void DisableSession()
        {
            if (!m_IsInitialized)
            {
                return;
            }
            if (NRVirtualDisplayer.RunInBackground)
            {
                if (VirtualDisplayer != null)
                {
                    VirtualDisplayer.Pause();
                }
                NativeAPI.NativeTracking.Pause();
                if (NRRenderer != null)
                {
                    NRRenderer.Pause();
                }
            }
            else
            {
                NRDevice.Instance.ForceKill();
            }
        }

        public void ResumeSession()
        {
            if (!m_IsInitialized)
            {
                return;
            }
            NativeAPI.NativeTracking.Resume();
            if (NRRenderer != null)
            {
                NRRenderer.Resume();
            }
            if (VirtualDisplayer != null)
            {
                VirtualDisplayer.Resume();
            }
        }

        public void DestroySession()
        {
            if (!m_IsInitialized && m_IsDestroyed)
            {
                return;
            }
            m_IsDestroyed = true;
            NativeAPI.NativeHeadTracking.Destroy();
            NativeAPI.NativeTracking.Destroy();
            VirtualDisplayer?.Destory();
            NRRenderer?.Destroy();
            NRDevice.Instance.Destroy();
            FrameCaptureContextFactory.DisposeAllContext();
            m_IsInitialized = false;
        }

        private void InitEmulator()
        {
            if (!NREmulatorManager.Inited && !GameObject.Find("NREmulatorManager"))
            {
                NREmulatorManager.Inited = true;
                GameObject.Instantiate(Resources.Load("Prefabs/NREmulatorManager"));
            }
            if (!GameObject.Find("NREmulatorHeadPos"))
            {
                GameObject.Instantiate(Resources.Load("Prefabs/NREmulatorHeadPose"));
            }
        }
    }
}
