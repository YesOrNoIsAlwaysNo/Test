using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    public static Ball Instance { get; private set; }
    [SerializeField]
    private GameObject _collisionEffectPrefab;
    // transform of ball's graphic part 
    [SerializeField]
    private Transform _graphicTransform;
    // parent of ball's graphic part
    [SerializeField]
    private Transform _pivotTransform;
    [SerializeField]
    private MeshRenderer _meshRenderer;
    private Vector3 _positionInLastFrame;
    private float _speedInLastFrame;
    public Vector3 velocity { get; private set; }
    private float rotationSpeed;
    private bool _ignoreInput;
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
        // adding friction
        velocity -= velocity * Time.deltaTime;

        float speed = Vector3.Distance(transform.position, _positionInLastFrame) / Time.deltaTime;
        _speedInLastFrame = speed;
        _positionInLastFrame = transform.position;
    }

    private void Rotate()
    {
        Vector3 moveDelta = velocity.normalized * _speedInLastFrame * Time.deltaTime;
        Vector3 rotationAxis = Vector3.Cross(velocity.normalized, Vector3.down);
        float ballRadious = transform.localScale.x / 2;
        _graphicTransform.RotateAround(_graphicTransform.position, rotationAxis, Mathf.Sin(moveDelta.magnitude * ballRadious * 2 * Mathf.PI) * Mathf.Rad2Deg);
    }
    private void Move()
    {
        transform.Translate(velocity * Time.deltaTime);
    }
    public void AddVelocity(Vector3 velocity)
    {
        if (_ignoreInput) return;

        this.velocity += velocity;
    }
    private Vector3 ReflectVelocity(Vector3 velocity, float speed, Vector3 normal)
    {
        Vector3 newVelocity = velocity;
        newVelocity.Normalize();
        newVelocity = Vector3.Reflect(newVelocity, normal);
        newVelocity *= speed;

        return newVelocity;
    }
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(CollisionHandler(collision));
    }
    private IEnumerator CollisionHandler(Collision collision)
    {
        _ignoreInput = true;
        // create smoke effect
        GameObject effect = Instantiate(_collisionEffectPrefab);
        effect.transform.position = collision.contacts[0].point;
        effect.transform.forward = collision.contacts[0].normal;

        _graphicTransform.parent = null;
        // setting up pivot
        _pivotTransform.position = collision.contacts[0].point;
        _pivotTransform.forward = collision.contacts[0].normal;

        _graphicTransform.parent = _pivotTransform;

        float collisionPower = _speedInLastFrame * Time.deltaTime * 2;
        // calculating diferent values of collision effect 
        float scaleValue = 1 - Mathf.Clamp(collisionPower, 0f, 0.7f);

        float redColorValue = Mathf.Clamp(collisionPower, 0f, 1f);
        Color collisionPowerColor = Color.Lerp(Color.white, Color.red, redColorValue);

        Vector3 velocityBeforeCollision = velocity;
        Vector3 ReflectedVelocity = ReflectVelocity(velocity, _speedInLastFrame, collision.contacts[0].normal);
        velocity = Vector3.zero;

        // scalingAnimationParamethers
        float animationTime = 0.1f;
        float startScale = _pivotTransform.localScale.x;

        velocity = ReflectedVelocity;

        // animation start
        for (float t = 0; t < animationTime / 2; t += Time.deltaTime)
        {
            // number from 0 to 1 then from 1 to 0
            // in the middle of animation number equals 1 and 0 in start and end
            float value = Mathf.Sin(t / animationTime * Mathf.PI);

            _meshRenderer.material.color = Color.Lerp(Color.white, collisionPowerColor, value);
            float newScale = Mathf.Lerp(1, scaleValue, value);

            Vector3 currentScale = _pivotTransform.localScale;
            currentScale.z = newScale;
            _pivotTransform.localScale = currentScale;
            yield return null;
        }

        // in the middle of scale animation we reflect the velocity

        // continue animation
        for (float t = animationTime / 2; t < animationTime + Time.deltaTime; t += Time.deltaTime)
        {
            float value = Mathf.Sin(t / animationTime * Mathf.PI);

            _meshRenderer.material.color = Color.Lerp(Color.white, collisionPowerColor, value);
            float newScale = Mathf.Lerp(1, scaleValue, value);

            Vector3 currentScale = _pivotTransform.localScale;
            currentScale.z = newScale;
            _pivotTransform.localScale = currentScale;
            yield return null;
        }

        // reset to defoult values
        _graphicTransform.parent = null;

        _pivotTransform.localPosition = Vector3.zero;
        _pivotTransform.rotation = Quaternion.identity;

        _graphicTransform.parent = _pivotTransform;

        _ignoreInput = false;
    }
}
