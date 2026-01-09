using _Train.Scripts.Level.Items.Data;
using UnityEngine;

namespace _Train.Scripts.Root
{
    public class GameEntryPoint : MonoBehaviour
    {
        public static GameEntryPoint Instance { get; private set; }
        
        [SerializeField] private ItemsConfig itemsConfig;
        
        public ItemsConfig ItemsConfig => itemsConfig;
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 80;
        }

        private void Start()
        {
            SceneLoader.Instance.StartLoadingGame();
        }
    }
}
