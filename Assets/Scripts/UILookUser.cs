using UnityEngine;

public class UILookUser : MonoBehaviour
{
    [SerializeField] private Transform _centralLocation;
    [SerializeField] private float _canvasForwardDistance;
    private Transform _camera;
    private Transform _canvas;
    private Vector3 forward;
    private void Start()
    {
        _camera = Camera.main.transform;
        _canvas = transform;
    }
    private void Update()
    {
        // UIキャンバスがプレイヤーに向く
        forward = _canvas.position - _camera.position;
        _canvas.localRotation = Quaternion.LookRotation(forward);

        // UIキャンバスがプレイヤー手前に追従する
        forward = _camera.position - _centralLocation.position;
        forward.y *= 0;
        float distance = forward.magnitude - _canvasForwardDistance;
        distance = distance < 0.75f ? 0.75f : distance;
        _canvas.position = _centralLocation.position + forward.normalized * distance;
    }
}
