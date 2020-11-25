using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadposeCanvas : MonoBehaviour
{
    #region Public Variables
    [Tooltip("The forwards distance from the camera that this object should be placed.")]
    public float CanvasDistanceForwards = 1.5f;

    [Tooltip("The upwards distance from the camera that this object should be placed.")]
    public float CanvasDistanceUpwards = 0.0f;

    [Tooltip("The speed at which this object changes its position.")]
    public float PositionLerpSpeed = 5f;

    [Tooltip("The speed at which this object changes its rotation.")]
    public float RotationLerpSpeed = 5f;
    #endregion

    #region Private Varibles

    // The camera this object will be in front of.
    [SerializeField] Camera _camera;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Update position and rotation of this canvas object to face the camera using lerp for smoothness.
    /// </summary>
    void Update()
    {
        // Move the object CanvasDistance units in front of the camera.
        float posSpeed = Time.deltaTime * PositionLerpSpeed;
        Vector3 posTo = _camera.transform.position + (_camera.transform.forward * CanvasDistanceForwards) + (_camera.transform.up * CanvasDistanceUpwards);
        transform.position = Vector3.SlerpUnclamped(transform.position, posTo, posSpeed);

        // Rotate the object to face the camera.
        float rotSpeed = Time.deltaTime * RotationLerpSpeed;
        Quaternion rotTo = Quaternion.LookRotation(transform.position - _camera.transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotTo, rotSpeed);
    }
    #endregion
}
