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
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Holds information about NR Device's pose in the world coordinate, trackables, etc..
    /// Through this class, application can get the infomation of current frame.
    /// It contains session status, lost tracking reason, device pose, trackables, etc..
    /// </summary>
    public partial class NRFrame
    {
        private static SessionState m_TrackingStatus = SessionState.UnInitialize;

        /// <summary>
        /// Get the tracking state of HMD.
        /// </summary>
        public static SessionState SessionStatus
        {
            get
            {
                return m_TrackingStatus;
            }
        }

        /// <summary>
        /// Get the lost tracking reason of HMD.
        /// </summary>
        public static LostTrackingReason LostTrackingReason
        {
            get
            {
                return NRSessionManager.Instance.LostTrackingReason;
            }
        }

        private static Pose m_HeadPose;

        /// <summary>
        /// Get the pose of device in unity world coordinate.
        /// </summary>
        /// <returns>Pose of device.</returns>
        public static Pose HeadPose
        {
            get
            {
                return m_HeadPose;
            }
        }

        public static bool GetHeadPoseByTime(ref Pose pose, UInt64 timestamp = 0, UInt64 predict = 0)
        {
            if (NRSessionManager.Instance.IsInitialized)
            {
                return NRSessionManager.Instance.NativeAPI.NativeHeadTracking.GetHeadPose(ref pose, timestamp, predict);
            }
            return false;
        }

        /// <summary>
        /// Get the pose of center camera between left eye and right eye.
        /// </summary>
        public static Pose CenterEyePose
        {
            get
            {
                if (NRSessionManager.Instance.IsInitialized)
                {
                    return HeadPose;
                }
                Transform leftCamera = NRSessionManager.Instance.NRHMDPoseTracker.leftCamera.transform;
                Transform rightCamera = NRSessionManager.Instance.NRHMDPoseTracker.rightCamera.transform;

                Vector3 centerEye_pos = (leftCamera.position + rightCamera.position) * 0.5f;
                Quaternion centerEye_rot = Quaternion.Lerp(leftCamera.rotation, rightCamera.rotation, 0.5f);

                return new Pose(centerEye_pos, centerEye_rot);
            }
        }

        private static EyePoseData m_EyePosFromHead;

        /// <summary>
        /// Get the offset position between eye and head.
        /// </summary>
        public static EyePoseData EyePosFromHead
        {
            get
            {
                if (NRSessionManager.Instance.IsInitialized)
                {
                    m_EyePosFromHead.LEyePose = NRDevice.Instance.NativeHMD.GetEyePoseFromHead(NativeEye.LEFT);
                    m_EyePosFromHead.REyePose = NRDevice.Instance.NativeHMD.GetEyePoseFromHead(NativeEye.RIGHT);
                    m_EyePosFromHead.RGBEyePos = NRDevice.Instance.NativeHMD.GetEyePoseFromHead(NativeEye.RGB);
                }
                return m_EyePosFromHead;
            }
        }

        /// <summary>
        /// Get the project matrix of camera in unity.
        /// </summary>
        /// <returns>project matrix of camera.</returns>
        public static EyeProjectMatrixData GetEyeProjectMatrix(out bool result, float znear, float zfar)
        {
            result = false;
            EyeProjectMatrixData m_EyeProjectMatrix = new EyeProjectMatrixData();
            result = NRDevice.Instance.NativeHMD.GetProjectionMatrix(ref m_EyeProjectMatrix, znear, zfar);
            return m_EyeProjectMatrix;
        }

        public static void OnUpdate()
        {
            // Update head pos
            Pose pose = Pose.identity;
            bool getHeadPoseSuccess = false;
            if (GetHeadPoseByTime(ref pose))
            {
                m_HeadPose = pose;
                getHeadPoseSuccess = true;
            }

            if (getHeadPoseSuccess)
            {
                if (LostTrackingReason == LostTrackingReason.NONE)
                {
                    m_TrackingStatus = SessionState.Tracking;
                }
                else if (LostTrackingReason == LostTrackingReason.INITIALIZING)
                {
                    m_TrackingStatus = SessionState.UnInitialize;
                }
                else
                {
                    m_TrackingStatus = SessionState.LostTracking;
                }
            }
            else
            {
                m_TrackingStatus = SessionState.UnInitialize;
            }
        }

        /// <summary>
        /// Get the list of trackables with specified filter.
        /// </summary>
        /// <param name="trackables">A list where the returned trackable stored.The previous values will be cleared.</param>
        /// <param name="filter">Query filter.</param>
        public static void GetTrackables<T>(List<T> trackables, NRTrackableQueryFilter filter) where T : NRTrackable
        {
            trackables.Clear();
#if !UNITY_EDITOR
            if (SessionStatus != SessionState.Tracking)
            {
                return;
            }
#endif
            NRSessionManager.Instance.NativeAPI.TrackableFactory.GetTrackables<T>(trackables, filter);
        }
    }
}
