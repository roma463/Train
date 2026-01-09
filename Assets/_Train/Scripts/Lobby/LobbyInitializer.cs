using Mirror;
using UnityEngine;

namespace _Train.Scripts.Lobby
{
    public class LobbyInitializer : NetworkBehaviour
    {
        public static LobbyInitializer Instance;

        private void Awake()
        {
            Debug.Log("<color=green>Awake</color>");
        
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public override void OnStartClient()
        {        
            base.OnStartClient();
        }
    }
}