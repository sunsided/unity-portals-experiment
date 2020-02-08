using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof(UnityEngine.CharacterController))]
    public class Character : MonoBehaviour
    {
        public float speed = 5f;
        public float jumpHeight = 2f;
        public float gravity = -9.81f;
        public float groundDistance = 0.2f;
        public float dashDistance = 5f;
        public LayerMask ground;
        public Vector3 drag;

        private UnityEngine.CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded = true;
        private Transform _groundChecker;

        private void Awake()
        {
            _controller = GetComponent<UnityEngine.CharacterController>();
        }

        private void Start()
        {
            _groundChecker = transform.GetChild(0);
        }

        private void Update()
        {
            _isGrounded = Physics.CheckSphere(_groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = 0f;
            }

            var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            var moveDistance = Time.deltaTime * speed;

            _controller.Move(moveDirection * moveDistance);
            if (moveDirection != Vector3.zero)
            {
                transform.forward = moveDirection;
            }

            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            if (Input.GetButtonDown("Dash"))
            {
                Debug.Log("Dash");
                _velocity += Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * drag.x + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * drag.z + 1)) / -Time.deltaTime)));
            }

            _velocity.y += gravity * Time.deltaTime;

            _velocity.x /= 1 + drag.x * Time.deltaTime;
            _velocity.y /= 1 + drag.y * Time.deltaTime;
            _velocity.z /= 1 + drag.z * Time.deltaTime;

            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}
