using TMPro.EditorUtilities;
using UnityEngine;

namespace Project
{
    public class Portal : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        [SerializeField]
        private Portal linkedPortal;

        [SerializeField]
        private MeshRenderer screen;

        private Camera _playerCamera;
        private Camera _portalCamera;
        private RenderTexture _viewTexture;

        private void Awake()
        {
            Debug.Assert(screen != null, "screen != null");
            Debug.Assert(linkedPortal != null, "linkedPortal != null");

            _playerCamera = Camera.main;
            _portalCamera = GetComponentInChildren<Camera>();

            // Ensure the camera settings are identical to the player's camera.
            _portalCamera.CopyFrom(_playerCamera);

            _portalCamera.enabled = false;
        }

        /// <summary>
        /// Renders the portal.
        /// </summary>
        /// <remarks>
        /// Called just before the player camera is rendered.
        /// </remarks>
        public void Render()
        {
            screen.enabled = false;
            CreateViewTexture();

            // Make the portal camera's relative position and rotation to the portal
            // the same as the player's relative position and rotation to the linked portal.
            var m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * _playerCamera.transform.localToWorldMatrix;
            _portalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

            // Render the camera (to the texture).
            _portalCamera.Render();

            screen.enabled = true;
        }

        private void CreateViewTexture()
        {
            var viewTextureExists = _viewTexture != null;
            if (viewTextureExists && _viewTexture.width == Screen.width && _viewTexture.height == Screen.height) return;
            if (viewTextureExists) _viewTexture.Release();

            _viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

            // Render the view from the portal camera to the view texture.
            _portalCamera.targetTexture = _viewTexture;

            // Display the view texture on the screen of the linked portal.
            linkedPortal.screen.material.SetTexture(MainTex, _viewTexture);
        }
    }
}
