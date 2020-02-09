using UnityEngine;

namespace Project
{
    internal class PortalTraveller : MonoBehaviour
    {
        public Vector3 PreviousOffsetFromPortal { get; set; }

        public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
        {
            var tf = transform;
            tf.position = pos;
            tf.rotation = rot;
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
