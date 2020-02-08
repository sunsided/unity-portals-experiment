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
            //Get a reference to the character motor component.
            _motor = GetComponent<CharacterMotor>();

            //Get a reference to the camera handler component.
            _cameraHandler = GetComponentInChildren<CameraHandler>();

            //Get a reference to the player handler.
            _pHandler = Manager.Instance.PlayerHandler;
        }

        private void Update()
        {
            _pHandler.Horizontal = Input.GetAxisRaw("Horizontal");
            _pHandler.Vertical = Input.GetAxisRaw("Vertical");

            //Toggle third person mode when pressing the key.
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                //Toggle the value.
                _pHandler.ThirdPerson = !_pHandler.ThirdPerson;

                //Event for view change.
                _cameraHandler.OnViewChange();
            }

            //Toggle crouching when pressing the key.
            if (Input.GetKeyDown(KeyCode.C))
            {
                //Check if we can stop crouching if we need to, or just crouch.
                if (_pHandler.IsCrouched && _motor.CanUncrouch() || _pHandler.IsCrouched == false)
                {
                    //Toggle the value.
                    _pHandler.IsCrouched = !_pHandler.IsCrouched;

                    //Let the character motor know we changed the state.
                    _motor.OnToggleCrouch();

                    //Change the camera view.
                    _cameraHandler.OnViewChange();
                }
            }

            //If the player is standing.
            if (_pHandler.IsCrouched == false)
            {
                //Jump & Run Inputs.
                _pHandler.Jump = Input.GetKey(KeyCode.Space);
                _pHandler.IsRunning = Input.GetKey(KeyCode.LeftShift);
            }

            _pHandler.MouseX = Input.GetAxisRaw("Mouse X");
            _pHandler.MouseY = Input.GetAxisRaw("Mouse Y");
        }
    }
}