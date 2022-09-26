using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    public static Ball Instance { get; private set; }
    // transform of ball's graphic part 
    [SerializeField]
    private Transform _graphicTransform;
    // parent of ball's graphic part
    [SerializeField]
    private Transform _pivotTransform;
    private Vector3 _positionInLastFrame;
    private float _speedInLastFrame;
    private Vector3 _velocity;
    private float rotationSpeed;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        _positionInLastFrame = transform.position;
    }

    private void Update()
    {
        Move();
        Rotate();
        _velocity -= _velocity * Time.deltaTime;
    }

    private void LateUpdate()
    {
        float speed = Vector3.Distance(transform.position, _positionInLastFrame) / Time.deltaTime;
        _speedInLastFrame = speed;
        _positionInLastFrame = transform.position;
    }
    private void Rotate()
    {
        Vector3 moveDelta = _velocity.normalized * _speedInLastFrame * Time.deltaTime;
        Vector3 rotationAxis = Vector3.Cross(_velocity.normalized, Vector3.down);
        _graphicTransform.RotateAround(_graphicTransform.position, rotationAxis, Mathf.Sin(moveDelta.magnitude * 1 * 2 * Mathf.PI) * Mathf.Rad2Deg / 2);
    }
    private void Move()
    {
        transform.Translate(_velocity * Time.deltaTime);
    }
    public void AddVelocity(Vector3 velocity)
    {
        _velocity += velocity;
    }
    private Vector3 MirrorVelocity(Vector3 velocity, float speed, Vector3 mirrorNormal)
    {
        Vector3 newVelocity = velocity;
        newVelocity.Normalize();
        newVelocity = Vector3.Reflect(newVelocity, mirrorNormal);
        newVelocity *= speed;

        return newVelocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(CollisionHandler(collision));
    }
    private IEnumerator CollisionHandler(Collision collision)
    {
        _graphicTransform.parent = null;
        // setting up pivot
        _pivotTransform.position = collision.contacts[0].point;
        _pivotTransform.forward = collision.contacts[0].normal;

        _graphicTransform.parent = _pivotTransform;
        // calculating scale of ball 
        float scaleValue = 1 - Mathf.Clamp(_speedInLastFrame * Time.deltaTime * 5, 0f, 0.7f);

        Vector3 velocityBeforeCollision = _velocity;
        Vector3 miroredVelocity = MirrorVelocity(_velocity, _speedInLastFrame, collision.contacts[0].normal);
        _velocity = Vector3.zero;

        // scalingAnimation
        float animationTime = 0.2f;
        float startScale = _pivotTransform.localScale.x;

        for (float t = 0; t < animationTime + Time.deltaTime; t += Time.deltaTime)
        {
            float process = Mathf.Sin(t / animationTime * Mathf.PI);
            float newScale = Mathf.Lerp(1, scaleValue, process);

            Vector3 currentScale = _pivotTransform.localScale;
            currentScale.z = newScale;
            _pivotTransform.localScale = currentScale;
            yield return null;
        }

        _graphicTransform.parent = null;

        _pivotTransform.localPosition = Vector3.zero;
        _pivotTransform.rotation = Quaternion.identity;

        _graphicTransform.parent = _pivotTransform;

        _velocity = miroredVelocity;
    }
}
