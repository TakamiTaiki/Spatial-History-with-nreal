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
    using UnityEngine;

    /// <summary>
    /// A Trackable in the real world detected by NRInternel.
    /// The base class of TrackablePlane and TrackableImage.Through this class, 
    /// application can get the infomation of a trackable object.
    /// </summary>
    public abstract class NRTrackable
    {
        internal UInt64 TrackableNativeHandle = 0;

        internal NativeInterface NativeInterface;

        internal NRTrackable(UInt64 trackableNativeHandle, NativeInterface nativeinterface)
        {
            TrackableNativeHandle = trackableNativeHandle;
            NativeInterface = nativeinterface;
        }

        /// <summary>
        /// Get the id of trackable.
        /// </summary>
        public int GetDataBaseIndex()
        {
            UInt32 identify = NativeInterface.NativeTrackable.GetIdentify(TrackableNativeHandle);
            identify &= 0X0000FFFF;
            return (int)identify;
        }

        /// <summary>
        /// Get the tracking state of current trackable.
        /// </summary>
        public TrackingState GetTrackingState()
        {
            return NativeInterface.NativeTrackable.GetTrackingState(TrackableNativeHandle);
        }

        /// <summary>
        /// Get the tracking type of current trackable.
        /// </summary>
        public TrackableType GetTrackableType()
        {
            return NativeInterface.NativeTrackable.GetTrackableType(TrackableNativeHandle);
        }

        /// <summary>
        /// Get the center pose of current trackable.
        /// </summary>
        public virtual Pose GetCenterPose()
        {
            return Pose.identity;
        }

        /// <summary>
        /// Creates an anchor attached to current trackable at given pose.
        /// </summary>
        internal NRAnchor CreateAnchor()
        {
            return NRAnchor.Factory(this);
        }
    }
}
