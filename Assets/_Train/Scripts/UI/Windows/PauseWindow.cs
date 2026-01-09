using _Train.Scripts.UI.Root;
using UnityEngine;

namespace _Train.Scripts.UI.Windows
{
    public class PauseWindow : Window
    {
        public void Leave()
        {
            LobbyManager.Instance.LeaveLobby();
        }

        // public void InviteFriends()
        // {
        //     if (!SteamManager.Initialized)
        //     {
        //         Debug.LogError("Steam не инициализирован!");
        //         return;
        //     }
        //
        //     var lobbyId = LobbyManager.Instance.CurrentLobbyId;
        //
        //     if (!lobbyId.IsValid())
        //     {
        //         Debug.LogError("Сначала создайте лобби!");
        //         return;
        //     }
        //
        //     SteamFriends.ActivateGameOverlayInviteDialog(lobbyId);
        // }
    }
}
