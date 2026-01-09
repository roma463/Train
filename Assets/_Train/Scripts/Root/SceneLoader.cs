using _Train.Scripts.UI;
using _Train.Scripts.UI.Windows;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Train.Scripts.Root
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartLoadingGame()
        {
            SceneManager.LoadScene(1);
        }
    
        public void NetworkLoadScene(string name)
        {
            if (!NetworkServer.active)
                return;
        
            LoadingWindow.Instance.Show();
        
            NetworkManager.singleton.ServerChangeScene(name);
        }
    }
}
