using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallController : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private Camera Camera;
    private Vector2 _startTouchPosition;
    private Vector3 _powerVector;
    // ignore touch if it was not taped on ball on begin phase 
    private bool _ignoreTouch;
    
    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        Camera = CameraMovement.Instance.Camera;

        _ignoreTouch = true;
    }
    private void Update()
    {
        if (Input.touchCount == 0) return;

        int indexOfTouch = 0;
        Touch touch = Input.GetTouch(indexOfTouch);
        Ray ray = Camera.ScreenPointToRay(touch.position);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        if (!hit.collider)
            return;

        Vector3 powerVector = Vector3.zero;

        if (touch.phase == TouchPhase.Began)
        {
            if (hit.collider.GetComponent<Ball>())
            {
                _lineRenderer.enabled = true;
                _startTouchPosition = touch.position;
                _ignoreTouch = false;
            }
        }

        if (_ignoreTouch) return;

        if (touch.phase == TouchPhase.Moved)
        {
            powerVector = CalculatePower(touch.position);
            Vector3 secondPoint = _lineRenderer.GetPosition(0) + powerVector;

            _lineRenderer.SetPosition(1, secondPoint);
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            powerVector = CalculatePower(touch.position);
            Ball.Instance.AddVelocity(powerVector);

            _ignoreTouch = true;
            _lineRenderer.enabled = false;
        }

        _powerVector = powerVector;
    }

    private Vector3 CalculatePower(Vector2 end)
    {
        Vector2 power = (end - _startTouchPosition) * 0.01f;
        Vector3 transformedPower = new Vector3(power.x, 0, power.y) * -1;
        return transformedPower;
    }
    private void LateUpdate()
    {
        Vector3 ballPosition = Ball.Instance.transform.position;
        _lineRenderer.SetPosition(0, ballPosition + Vector3.up * 0.1f);
    }
}
