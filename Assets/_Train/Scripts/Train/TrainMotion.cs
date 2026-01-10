using UnityEngine;

public class TrainMotion : MonoBehaviour
{
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float braking = 14f;
    [SerializeField] private Rigidbody trainRb;
    
    private float targetSpeed;

    public Vector3 Velocity { get; private set; }

    private Vector3 prevPos;
    
    private void Awake()
    {
        trainRb = GetComponent<Rigidbody>();
        trainRb.interpolation = RigidbodyInterpolation.Interpolate;
        prevPos = trainRb.position;
    }
    public void SetTargetSpeed(float speed)
    {
        targetSpeed = Mathf.Max(0f, speed);
    }

    private void FixedUpdate()
    {
        float currentSpeed = Vector3.Dot(Velocity, transform.forward);

        float desiredSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            (targetSpeed > currentSpeed ? acceleration : braking) * Time.fixedDeltaTime
        );

        trainRb.MovePosition(trainRb.position + transform.forward * desiredSpeed * Time.fixedDeltaTime);

        Velocity = (trainRb.position - prevPos) / Time.fixedDeltaTime;
        prevPos = trainRb.position;
    }
}
