﻿using UnityEngine;

namespace NRKernal.NRExamples
{
    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class HelloMRController : MonoBehaviour
    {
        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject AndyPlanePrefab;

        void Update()
        {
            // If the player doesn't click the trigger button, we are done with this update.
            if (!NRInput.GetButtonDown(ControllerButton.TRIGGER))
            {
                return;
            }

            // Get controller laser origin.
            Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : ControllerAnchorEnum.RightLaserAnchor);

            RaycastHit hitResult;
            if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hitResult, 10))
            {
                if (hitResult.collider.gameObject != null && hitResult.collider.gameObject.GetComponent<NRTrackableBehaviour>() != null)
                {
                    var behaviour = hitResult.collider.gameObject.GetComponent<NRTrackableBehaviour>();
                    if (behaviour.Trackable.GetTrackableType() != TrackableType.TRACKABLE_PLANE)
                    {
                        return;
                    }

                    // Instantiate Andy model at the hit point / compensate for the hit point rotation.
                    Instantiate(AndyPlanePrefab, hitResult.point, Quaternion.identity, behaviour.transform);
                }
            }
        }
    }
}
