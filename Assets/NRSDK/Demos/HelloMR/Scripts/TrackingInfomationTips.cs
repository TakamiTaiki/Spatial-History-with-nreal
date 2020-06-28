using System.Collections.Generic;
using UnityEngine;

namespace NRKernal.NRExamples
{
    public class TrackingInfomationTips : SingletonBehaviour<TrackingInfomationTips>
    {
        private Dictionary<TipType, GameObject> m_TipsDict = new Dictionary<TipType, GameObject>();
        public enum TipType
        {
            UnInitialized,
            LostTracking,
            None
        }
        private GameObject m_CurrentTip;

        private Camera centerCamera;

        void Start()
        {
            centerCamera = NRSessionManager.Instance.NRHMDPoseTracker.centerCamera;
        }

        private void OnEnable()
        {
            NRHMDPoseTracker.OnHMDLostTracking += OnHMDLostTracking;
            NRHMDPoseTracker.OnHMDPoseReady += OnHMDPoseReady;
        }

        private void OnDisable()
        {
            NRHMDPoseTracker.OnHMDLostTracking -= OnHMDLostTracking;
            NRHMDPoseTracker.OnHMDPoseReady -= OnHMDPoseReady;
        }

        private void OnHMDPoseReady()
        {
            Debug.Log("[NRHMDPoseTracker] OnHMDPoseReady");
        }

        private void OnHMDLostTracking()
        {
            Debug.Log("[NRHMDPoseTracker] OnHMDLostTracking");
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private void Update()
        {
            if (NRFrame.SessionStatus == SessionState.UnInitialize)
            {
                ShowTips(TipType.UnInitialized);
            }
            else if (NRFrame.SessionStatus == SessionState.LostTracking)
            {
                ShowTips(TipType.LostTracking);
            }
            else
            {
                ShowTips(TipType.None);
            }
    }
#endif

        public void ShowTips(TipType type)
        {
            switch (type)
            {
                case TipType.UnInitialized:
                case TipType.LostTracking:
                    GameObject go;
                    m_TipsDict.TryGetValue(type, out go);

                    if (go == m_CurrentTip && go != null)
                    {
                        return;
                    }

                    if (go != null)
                    {
                        go.SetActive(true);
                    }
                    else
                    {
                        go = Instantiate(Resources.Load(type.ToString() + "Tip"), centerCamera.transform) as GameObject;
                        m_TipsDict.Add(type, go);
                    }

                    if (m_CurrentTip != null)
                    {
                        m_CurrentTip.SetActive(false);
                    }
                    m_CurrentTip = go;
                    break;
                case TipType.None:
                    if (m_CurrentTip != null)
                    {
                        m_CurrentTip.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
        }

        new void OnDestroy()
        {
            if (isDirty) return;
            if (m_TipsDict != null)
            {
                foreach (var item in m_TipsDict)
                {
                    if (item.Value != null)
                    {
                        GameObject.Destroy(item.Value);
                    }
                }
            }
        }
    }
}