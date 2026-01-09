using Mirror;
// using Steamworks;
using System;
using Telepathy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public event Action OnStartedServer;

    // public CSteamID CurrentLobbyId;

    private NetworkManager networkManager => NetworkManager.singleton;

    // private Callback<LobbyCreated_t> _lobbyCreated;
    // private Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;
    // private Callback<LobbyEnter_t> _lobbyEntered;
    // private Callback<LobbyChatUpdate_t> _lobbyChatUpdate;

    private const string HostAddressKey = "HostAddress";
    
    private void Start()
    {
        // if (!SteamManager.Initialized)
        //     return;
        
        // SteamNetworkingUtils.GetRelayNetworkStatus(out var status);
        Instance = this;

        Init();
    }

    private void Init()
    {
        // _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        // _gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        // _lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        //
        // _lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
    }

    public void LeaveLobby()
    {
        // if (!SteamManager.Initialized) return;

        if (NetworkServer.active)
        {
            Debug.Log("LeaveServer");

            NetworkManager.singleton.StopHost();
            TryLeaveLobbySteam();
        }
        else if (NetworkClient.active)
        {
            Debug.Log("LeaveClient");
            NetworkManager.singleton.StopClient();
            TryLeaveLobbySteam();
            ReturnToMainMenu();
        }
        
        ClearNetworkState();
    }
    
    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(Scenes.MainMenu);
    }

    private void ClearNetworkState()
    {
        if (NetworkClient.active)
        {
            NetworkClient.Shutdown();
        }
        
        if (NetworkServer.active)
        {
            NetworkServer.Shutdown();
        }
    }

    public void TryLeaveLobbySteam()
    {
        
        // if (CurrentLobbyId.IsValid() || CurrentLobbyId != CSteamID.Nil)
        // {
        //     SteamMatchmaking.LeaveLobby(CurrentLobbyId);
        //     CurrentLobbyId = CSteamID.Nil;
        // }
    }

    // private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    // {
    //     EChatMemberStateChange change = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;
    //
    //     if (change.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeLeft))
    //     {
    //         CSteamID playerId = new CSteamID(callback.m_ulSteamIDUserChanged);
    //         string playerName = SteamFriends.GetFriendPersonaName(playerId);
    //
    //         Debug.Log($"id = {playerId}, name = {playerName} Вышел из лобби");
    //     }
    //     else if (change.HasFlag(EChatMemberStateChange.k_EChatMemberStateChangeEntered))
    //     {
    //         CSteamID playerId = new CSteamID(callback.m_ulSteamIDUserChanged);
    //         string playerName = SteamFriends.GetFriendPersonaName(playerId);
    //
    //         Debug.Log($"id = {playerId}, name = {playerName} присоединился к лобби");
    //     }
    // }
    //
    // public void CreateLobby()
    // {
    //     if (CurrentLobbyId == CSteamID.Nil)
    //         SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    // }
    //
    // private void OnLobbyCreated(LobbyCreated_t callback)
    // {
    //     if (callback.m_eResult != EResult.k_EResultOK)
    //     {
    //         Debug.LogError($"Lobby creation failed: {callback.m_eResult}");
    //         return;
    //     }
    //
    //     CurrentLobbyId = new CSteamID(callback.m_ulSteamIDLobby);
    //     SteamNetworking.AllowP2PPacketRelay(true);
    //     SteamMatchmaking.SetLobbyData(
    //         CurrentLobbyId,
    //         HostAddressKey,
    //         SteamUser.GetSteamID().ToString()
    //     );
    //     
    //     // SteamFriends.SetRichPresence("connect", "");
    //     
    //     networkManager.StartHost();
    //     // SteamFriends.SetRichPresence("status", "In hub");
    //     // SteamFriends.SetRichPresence("map", "Main Menu");
    //     
    //     OnStartedServer?.Invoke();
    // }
    //
    // private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    // {
    //     Debug.Log($"Joining lobby: {callback.m_steamIDLobby} status");
    //     SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    // }
    //
    // private void OnLobbyEntered(LobbyEnter_t callback)
    // {
    //     Debug.Log($"Entered lobby: {callback.m_ulSteamIDLobby}");
    //     CurrentLobbyId = new CSteamID(callback.m_ulSteamIDLobby);
    //
    //     if (NetworkServer.active)
    //     {
    //         Debug.Log("Entered lobby as host");
    //         return;
    //     }
    //
    //     string hostAddress = SteamMatchmaking.GetLobbyData(CurrentLobbyId, HostAddressKey);
    //
    //     if (string.IsNullOrEmpty(hostAddress))
    //     {
    //         Debug.LogError("Host address not found in lobby data");
    //         return;
    //     }
    //
    //     Debug.Log($"Connecting to host: {hostAddress}");
    //     networkManager.networkAddress = hostAddress;
    //     networkManager.StartClient();
    // }
}
