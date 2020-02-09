using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Project
{
    public class Portal : MonoBehaviour
    {
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

#if UNITY_EDITOR
        private static Texture2D _disabledPortalTexture;
#endif

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

            EditorCreateDisabledPortalTexture();
        }

        /// <summary>
        /// Renders the portal.
        /// </summary>
        /// <remarks>
        /// Called just before the player camera is rendered.
        /// </remarks>
        public void Render()
        {
            if (!VisibleFromCamera(linkedPortal.screen, _playerCamera))
            {
                EditorColorDisabledPortal();
                return;
            }

            screen.enabled = false;
            CreateViewTexture();

            // Make the portal camera's relative position and rotation to the portal
            // the same as the player's relative position and rotation to the linked portal.
            var m = transform.localToWorldMatrix * linkedPortal.transform.worldToLocalMatrix * _playerCamera.transform.localToWorldMatrix;
            _portalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);

            // Render the camera (to the texture).
            // TODO: Can we limit the camera's rendering to only the section covered by the portal itself?
            _portalCamera.Render();

            screen.enabled = true;
        }

        private static bool VisibleFromCamera(Renderer renderer, Camera camera)
        {
            var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
        }

        [Conditional("UNITY_EDITOR")]
        private static void EditorCreateDisabledPortalTexture()
        {
#if UNITY_EDITOR
            _disabledPortalTexture = new Texture2D(1, 1);
            _disabledPortalTexture.SetPixel(0, 0, Color.red);
            _disabledPortalTexture.Apply();
#endif
        }

        [Conditional("UNITY_EDITOR")]
        private void EditorColorDisabledPortal()
        {
#if UNITY_EDITOR
            linkedPortal.screen.material.SetTexture(MainTex, _disabledPortalTexture);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        private void EditorReestablishViewTextureForPortal()
        {
            if (_viewTexture == null) return;
#if UNITY_EDITOR
            linkedPortal.screen.material.SetTexture(MainTex, _viewTexture);
#endif
        }

        private void CreateViewTexture()
        {
            var viewTextureExists = _viewTexture != null;
            if (viewTextureExists && _viewTexture.width == Screen.width && _viewTexture.height == Screen.height)
            {
                // Since we're using hint textures for disabled portals,
                // we need to ensure that a disabled portal is re-enabled again.
                EditorReestablishViewTextureForPortal();
                return;
            }
            if (viewTextureExists) _viewTexture.Release();

            _viewTexture = new RenderTexture(Screen.width, Screen.height, 0);

            // Render the view from the portal camera to the view texture.
            _portalCamera.targetTexture = _viewTexture;

            // Display the view texture on the screen of the linked portal.
            linkedPortal.screen.material.SetTexture(MainTex, _viewTexture);
        }
    }
}
