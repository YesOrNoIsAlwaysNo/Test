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
        Vector3 ballVelocityOffset = Vector3.Lerp(Vector3.zero, Ball.Instance.velocity, Time.deltaTime * _speed);
        transform.position = Ball.Instance.transform.position + _offset;
    }
}
