using SimpleCharacterController.Controller.Camera;
using SimpleCharacterController.Controller.Movement;
using SimpleCharacterController.General;
using UnityEngine;

namespace SimpleCharacterController.Controller
{
    public class InputManager : MonoBehaviour
    {
        private CharacterMotor _motor;
        private CameraHandler _cameraHandler;

        private PlayerHandler _pHandler;

        private void Awake()
        {
            _motor = GetComponent<CharacterMotor>();
            _cameraHandler = GetComponentInChildren<CameraHandler>();
        }

        private void Start()
        {
            _pHandler = Manager.Instance.PlayerHandler;
        }

        private void Update()
        {
            _pHandler.Horizontal = Input.GetAxisRaw("Horizontal");
            _pHandler.Vertical = Input.GetAxisRaw("Vertical");

            // TODO: Replace with Input.GetAxis().
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _pHandler.ThirdPerson = !_pHandler.ThirdPerson;
                _cameraHandler.OnViewChange();
            }

            // TODO: Replace with Input.GetAxis().
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (_pHandler.IsCrouched && _motor.CanUncrouch() || _pHandler.IsCrouched == false)
                {
                    _pHandler.IsCrouched = !_pHandler.IsCrouched;
                    _motor.OnToggleCrouch();
                    _cameraHandler.OnViewChange();
                }
            }

            if (_pHandler.IsCrouched == false)
            {
                // TODO: Replace with Input.GetAxis().
                _pHandler.Jump = Input.GetKey(KeyCode.Space);

                // TODO: Replace with Input.GetAxis().
                _pHandler.IsRunning = Input.GetKey(KeyCode.LeftShift);
            }

            _pHandler.MouseX = Input.GetAxisRaw("Mouse X");
            _pHandler.MouseY = Input.GetAxisRaw("Mouse Y");
        }
    }
}