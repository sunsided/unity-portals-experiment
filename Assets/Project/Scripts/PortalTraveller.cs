using System;
using UnityEngine;

namespace Project
{
    internal class PortalTraveller : MonoBehaviour
    {
        [SerializeField]
        private Transform entityToTransform;

        public Transform EntityTransform => entityToTransform;

        public Vector3 PreviousOffsetFromPortal { get; set; }

        private void Awake()
        {
            if (entityToTransform == null)
            {
                entityToTransform = transform;
            }
        }

        public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
        {
            entityToTransform.position = pos;
            entityToTransform.rotation = rot;
        }

        /// <summary>
        /// Called once the player is first touching the portal.
        /// </summary>
        public virtual void EnterPortalThreshold()
        {
        }

        /// <summary>
        /// Called once the player is no longer touching the portal.
        /// </summary>
        public virtual void ExitPortalThreshold()
        {
        }
    }
}
