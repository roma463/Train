using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

namespace _Train.Scripts.Train
{
    public class TrainMotion : MonoBehaviour
    {
        public Vector3 Velocity { get; private set; }

        [SerializeField] private Rails.Rails rails;
        [SerializeField] private float acceleration = 8f;
        [SerializeField] private float braking = 14f;
        [SerializeField] private Rigidbody trainRb;
        [SerializeField] private float maxSpeed = 10f;
        
        [SerializeField, Range(0f, 1f)] private float range;
    
        private float targetSpeed;

        private Vector3 prevPos;
    
        private void Awake()
        {
            trainRb = GetComponent<Rigidbody>();
            trainRb.interpolation = RigidbodyInterpolation.Interpolate;
            var startPosition = rails.GetStartPosition();
            
            transform.position = startPosition;
            prevPos = startPosition;
            
        }
        public void SetTargetSpeed(float speed)
        {
            targetSpeed = speed;
        }

        private void FixedUpdate()
        {
            // float currentSpeed = Vector3.Dot(Velocity, transform.forward);
            //
            // float desiredSpeed = Mathf.MoveTowards( currentSpeed, targetSpeed,
            //     (targetSpeed > currentSpeed ? acceleration : braking) * Time.fixedDeltaTime
            // );

            var position = rails.GetPosition(targetSpeed / maxSpeed);
            
            trainRb.MovePosition(position);
            trainRb.MoveRotation(rails.GetRotationOnCurrentRange());

            Velocity = (position - prevPos) / Time.fixedDeltaTime;
            prevPos = position;
        }
    }
}
