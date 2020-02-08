using System;
using UnityEngine;

namespace CharacterController
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyCharacter : MonoBehaviour
    {
        public float speed = 5f;
        public float jumpHeight = 2f;
        public float groundDistance = 0.2f;
        public float dashDistance = 5f;
        public LayerMask ground;

        private Rigidbody _body;
        private Vector3 _inputs = Vector3.zero;
        private bool _isGrounded = true;
        private Transform _groundChecker;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _groundChecker = transform.GetChild(0);
        }

        private void Update()
        {
            _isGrounded = Physics.CheckSphere(_groundChecker.position, groundDistance, ground, QueryTriggerInteraction.Ignore);

            _inputs = Vector3.zero;
            _inputs.x = Input.GetAxis("Horizontal");
            _inputs.z = Input.GetAxis("Vertical");
            if (_inputs != Vector3.zero)
            {
                transform.forward = _inputs;
            }

            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _body.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            }

            if (Input.GetButtonDown("Dash"))
            {
                var dashVelocity = Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime)));
                _body.AddForce(dashVelocity, ForceMode.VelocityChange);
            }
        }

        private void FixedUpdate()
        {
            var distance = speed * Time.fixedDeltaTime;
            _body.MovePosition(_body.position + _inputs * distance);
        }
    }
}
