using SimpleCharacterController.General;
using UnityEngine;

namespace SimpleCharacterController.Controller.Movement
{
    public class CharacterMotor : MonoBehaviour
    {
        [SerializeField]
        private float speed = 5;

        [Range(0, 4)]
        [SerializeField]
        private float runMultiplier = 2;

        [SerializeField]
        private float crouchSpeedMultiplier = 1;

        [Header("Crouching")]
        [Space(5f)]

        [SerializeField]
        private Lerper crouchLerp;

        [SerializeField]
        private float crouchHeightMultiplier = 0.5f;

        [Header("Uncrouching detection")]
        [Space(5f)]

        [SerializeField]
        private float uncrouchDetectionSphereHeightMultiplier = 1.8f;

        [SerializeField]
        private LayerMask uncrouchDetectionMask;

        [Header("In air settings")]
        [Space(5f)]

        [Tooltip("Determines how hard the gravity pushes down on the player")]
        [SerializeField]
        private float gravity = 10;

        [SerializeField]
        private float stickToGroundForce = 20;

        [Tooltip("Determines how far we can jump")]
        [SerializeField]
        private float jumpForce = 6;

        [Tooltip("Determines how fast we change directions while in the air")]
        [SerializeField]
        private float airAccelerationSpeed = 5;

        [Header("Debugs")]
        [Space(5f)]

        [SerializeField]
        private bool debugCrouchDetectionSphere;

        private Vector3 _moveVector;
        private Vector3 _currentVelocity;

        private PlayerHandler _pHandler;
        private CharacterController _controller;

        private bool _inTransition;
        private Vector2 _normalControllerValues;

        private Vector3 _cameraNormalPosition;
        private Transform _mainCamera;

        private void Start()
        {
            //Get a reference to the player handler.
            _pHandler = Manager.Instance.PlayerHandler;
            //Get a reference to the controller component.
            _controller = GetComponent<CharacterController>();

            //Get the camera object's transform.
            _mainCamera = GetComponentInChildren<UnityEngine.Camera>().transform;
            //Save the camera's standing position.
            _cameraNormalPosition = _mainCamera.localPosition;

            //Store the default standing values.
            _normalControllerValues.x = _controller.height;
            _normalControllerValues.y = _controller.center.y;
        }

        private void UpdateMoveVector()
        {
            //Vector that moves the character along its x axis.
            var rightVector = _pHandler.Horizontal * transform.GetChild(0).right;
            //Vector that moves the character along its z axis.
            var forwardVector = _pHandler.Vertical * transform.GetChild(0).forward;

            //Vector that moves the character.
            _moveVector = rightVector + forwardVector;
            _moveVector.Normalize();
        }

        private void OnDrawGizmosSelected()
        {
            if (debugCrouchDetectionSphere)
                Gizmos.DrawWireSphere(transform.position + Vector3.up * uncrouchDetectionSphereHeightMultiplier, _controller.radius);
        }

        public bool CanUncrouch()
        {
            //Check if there are colliders over the controller.
            var hitColliders = Physics.OverlapSphere(transform.position + Vector3.up * uncrouchDetectionSphereHeightMultiplier,
                _controller.radius, uncrouchDetectionMask);

            //If there's nothing on top of the player, we can uncrouch.
            if (hitColliders.Length == 0)
                return true;

            return false;
        }

        public void OnToggleCrouch()
        {
            //We start the transition.
            _inTransition = true;

            //Reset the lerp.
            crouchLerp.Reset();
        }

        private void Update()
        {
            //If we need to transition.
            if (_inTransition)
            {
                //Update the easing.
                crouchLerp.Update(Time.deltaTime);

                //Stop the transition if it's done.
                if (crouchLerp.IsDone(true))
                    _inTransition = false;

                //Get the correct values to transition to.
                var gHeight = _pHandler.IsCrouched ? _normalControllerValues.x * crouchHeightMultiplier : _normalControllerValues.x;
                var gCenter = _pHandler.IsCrouched ? _normalControllerValues.y * crouchHeightMultiplier : _normalControllerValues.y;

                //Lerp to those values.
                _controller.height = Mathf.Lerp(_controller.height, gHeight, crouchLerp.InterpolatedValue);
                _controller.center = Vector3.Lerp(_controller.center, gCenter * Vector3.up, crouchLerp.InterpolatedValue);
            }
        }

        private void FixedUpdate()
        {
            UpdateMoveVector();

            //Get the current fixed delta time.
            var deltaTime = Time.fixedDeltaTime;

            //If we're grounded.
            if (_controller.isGrounded)
            {
                //Get the new movement direction.
                _currentVelocity = _moveVector * GetSpeed();
                //Stick the character to the ground.
                _currentVelocity.y -= stickToGroundForce;

                //If we jump, apply some force.
                if (_pHandler.Jump)
                    _currentVelocity = Vector3.up * jumpForce;
            }
            //If we're in the air.
            else
            {
                //Calculate the gravity vector.
                var gravityVector = Vector3.down * (gravity * deltaTime);
                //Add it to the current velocity.
                _currentVelocity += gravityVector;

                //In air movement velocity.
                var inAirMoveVector = _moveVector * GetSpeed();
                //This subtraction allows for the nice curvature of the jump by factoring in our gravity.
                inAirMoveVector -= _currentVelocity;

                //Project the velocity so it's perpendicular to the controller.
                var velocityDiff = Vector3.ProjectOnPlane(inAirMoveVector, gravityVector);

                //Add it to the current velocity.
                _currentVelocity += velocityDiff * (airAccelerationSpeed * deltaTime);
            }

            //Set the current velocity value.
            _pHandler.CurrentVelocity = _currentVelocity;

            //Move the controller.
            _controller.Move(_currentVelocity * deltaTime);
        }

        private float GetSpeed()
        {
            var speed = this.speed;
            speed *= _pHandler.IsRunning ? runMultiplier : 1f;
            speed *= _pHandler.IsCrouched ? crouchSpeedMultiplier : 1f;

            return speed;
        }
    }
}