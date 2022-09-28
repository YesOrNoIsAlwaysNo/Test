using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance { get; private set; }
    [SerializeField]
    private float _speed;
    // camera offset
    [SerializeField]
    private Vector3 _offset;
    public Camera Camera { get; private set; }
    private void Awake()
    {
        Instance = this;
        Camera = GetComponent<Camera>();
    }

    private void Start()
    {
        transform.position = Ball.Instance.transform.position + _offset;
        transform.LookAt(Ball.Instance.transform);
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = Ball.Instance.transform.position + Ball.Instance.velocity + _offset;
        
        transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * _speed);
    }
}
