using System;
using UnityEngine;

namespace Project
{
    public class PlayerPortalRenderer : MonoBehaviour
    {
        private Portal[] _portals;

        private void Start()
        {
            _portals = FindObjectsOfType<Portal>();
        }

        // TODO: This is probably not how it should be done, but I just can't get it to work differently.
        private void OnPostRender()
        {
            for (var i = 0; i < _portals.Length; ++i)
            {
                var portal = _portals[i];
                portal.Render();
            }
        }
    }
}
