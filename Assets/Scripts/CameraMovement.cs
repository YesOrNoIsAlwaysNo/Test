using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance { get; private set; }
    [SerializeField]
    private float _speed;
    // Y offset 
    [SerializeField]
    private float _height = 5;
    private Vector3 _offset;
    public Camera Camera { get; private set; }
    private void Awake()
    {
        Instance = this;
        Camera = GetComponent<Camera>();
    }

    private void Start()
    {
        _offset = Vector3.up * _height;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, Ball.Instance.transform.position + _offset, Time.deltaTime * _speed);
    }
}
