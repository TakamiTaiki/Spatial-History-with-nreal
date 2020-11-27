using UnityEngine;

public class HeadposeCanvas : MonoBehaviour
{
    public float CanvasDistanceForwards = 1.5f;

    public float CanvasDistanceUpwards = 0.0f;

    public float PositionLerpSpeed = 5f;

    public float RotationLerpSpeed = 5f;

    private Camera camera;
    private void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        // Move the object CanvasDistance units in front of the camera.
        float posSpeed = Time.deltaTime * PositionLerpSpeed;
        Vector3 posTo = camera.transform.position + (camera.transform.forward * CanvasDistanceForwards) + (camera.transform.up * CanvasDistanceUpwards);
        transform.position = Vector3.SlerpUnclamped(transform.position, posTo, posSpeed);

        // Rotate the object to face the camera.
        float rotSpeed = Time.deltaTime * RotationLerpSpeed;
        Quaternion rotTo = Quaternion.LookRotation(transform.position - camera.transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotTo, rotSpeed);
    }
}
