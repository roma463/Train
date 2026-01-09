using System;
using _Train.Scripts.Root;
using Mirror;
using Unity.VisualScripting;

namespace _Train.Scripts.Level.Items.Data
{
    public class LevelItem : NetworkBehaviour
    {
        public event Action OnSetup;
        public ItemData ItemData { get; private set; }
        
        [SyncVar(hook = nameof(OnChangeIndexData))]
        private int _idItemData;

        public override void OnStartClient()
        {
            if (ItemData == null)
            {
                ItemData = GameEntryPoint.Instance.ItemsConfig.GetItemDataByIndex(_idItemData);
            }
        }
        
        [Server]
        public void Setup(ItemData itemData)
        {
            ItemData = itemData;
            _idItemData = GameEntryPoint.Instance.ItemsConfig.GetIndexByItemData(itemData);
            OnSetup?.Invoke();
        }

        private void OnChangeIndexData(int oldId, int newId)
        {
            ItemData = GameEntryPoint.Instance.ItemsConfig.GetItemDataByIndex(newId);
            OnSetup?.Invoke();
        }
    }
}
