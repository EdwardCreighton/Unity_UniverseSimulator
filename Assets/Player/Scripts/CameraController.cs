using PocketUniverse.Input;
using UnityEngine;

namespace PocketUniverse.PlayerControls
{
    public class CameraController : MonoBehaviour
    {
        #region Fields

        [Header("Debug Fields")] public bool rotate;
        
        [Space]
        [SerializeField] private float yawSensitivity;
        [SerializeField] private float pitchSensitivity;

        [SerializeField] private float moveAcceleration;
        private float currentMovementMultiplier;
        
        private float currentYaw;
        private float currentPitch;
        
        private InputData inputData;

        private const float lookThreshold = 0.01f;
        private const float moveThreshold = 0.01f;

        #endregion

        private void Awake()
        {
            inputData = GetComponent<InputListener>().GetInput();
        }

        private void Update()
        {
            MoveCamera();
        }

        private void LateUpdate()
        {
            if (rotate)
            {
                RotateCamera();
            }
        }

        private void MoveCamera()
        {
            Vector3 moveDirection = transform.forward * inputData.move.y + transform.right * inputData.move.x;

            if (moveDirection.sqrMagnitude >= moveThreshold)
            {
                transform.position += moveDirection * (currentMovementMultiplier * Time.deltaTime);

                currentMovementMultiplier += moveAcceleration * Time.deltaTime;
                currentMovementMultiplier = Mathf.Clamp(currentMovementMultiplier, 1f, 5f);
            }
            else
            {
                currentMovementMultiplier = 1;
            }
        }

        private void RotateCamera()
        {
            Vector2 look = inputData.look;
            
            if (look.sqrMagnitude >= lookThreshold)
            {
                currentYaw += look.x * yawSensitivity * Time.deltaTime;
                currentPitch += look.y * pitchSensitivity * Time.deltaTime;
            }

            currentYaw = ClampValue(currentYaw);
            currentPitch = ClampValue(currentPitch);
            
            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }

        private float ClampValue(float value)
        {
            if (value >= 360f)
            {
                return value -= 360f;
            }

            if (value <= -360f)
            {
                return value += 360f;
            }

            return value;
        }
    }
}
