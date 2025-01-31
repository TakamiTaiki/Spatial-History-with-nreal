﻿/****************************************************************************
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
    /// A plane in the real world detected by NRInternel.
    /// </summary>
    public class NRTrackablePlane : NRTrackable
    {
        internal NRTrackablePlane(UInt64 nativeHandle, NativeInterface nativeInterface)
          : base(nativeHandle, nativeInterface)
        {
        }

        /// <summary>
        /// Get the plane type.
        /// </summary>
        /// <returns>Plane type.</returns>
        public TrackablePlaneType GetPlaneType()
        {
            return NativeInterface.NativePlane.GetPlaneType(TrackableNativeHandle);
        }

        /// <summary>
        /// Gets the position and orientation of the plane's center in Unity world space.
        /// </summary>
        /// <returns></returns>
        public override Pose GetCenterPose()
        {
            return NativeInterface.NativePlane.GetCenterPose(TrackableNativeHandle);
        }

        /// <summary>
        /// Gets the extent of plane in the X dimension, centered on the plane position.
        /// </summary>
        public float ExtentX
        {
            get
            {
                return NativeInterface.NativePlane.GetExtentX(TrackableNativeHandle);
            }
        }

        /// <summary>
        /// Gets the extent of plane in the Z dimension, centered on the plane position.
        /// </summary>
        public float ExtentZ
        {
            get
            {
                return NativeInterface.NativePlane.GetExtentZ(TrackableNativeHandle);
            }
        }

        /// <summary>
        /// Gets a list of points(in clockwise order) in plane coordinate representing a boundary polygon for the plane.
        /// </summary>
        /// <param name="polygonList">polygonList A list used to be filled with polygon points.</param>
        public void GetBoundaryPolygon(List<Vector3> polygonList)
        {
            polygonList.Clear();
            int size = NativeInterface.NativePlane.GetPolygonSize(TrackableNativeHandle);
            float[] temp_data = NativeInterface.NativePlane.GetPolygonData(TrackableNativeHandle);
            float[] point_data = new float[size * 2];
            Array.Copy(temp_data, point_data, size * 2);

            Pose centerPos = GetCenterPose();
            var unityWorldTPlane = Matrix4x4.TRS(centerPos.position, centerPos.rotation, Vector3.one);
            for (int i = 2 * size - 2; i >= 0; i -= 2)
            {
                var point = unityWorldTPlane.MultiplyPoint3x4(new Vector3(point_data[i], 0, -point_data[i + 1]));
                polygonList.Add(point);
            }
        }
    }
}
