using UnityEngine;

namespace SimpleCharacterController.General
{
    public class Manager : MonoBehaviour
    {
        private static Manager _instance;

        public static Manager Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<Manager>();
                if (_instance == null)
                {
                    _instance = new GameObject("Manager").AddComponent<Manager>();
                }

                return _instance;
            }
        }

        public PlayerHandler PlayerHandler { get; private set; }

        private void Awake()
        {
            PlayerHandler = FindObjectOfType<PlayerHandler>();
        }
    }
}