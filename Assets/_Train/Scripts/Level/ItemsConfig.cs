using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using EditorUtility = UnityEditor.EditorUtility;
#endif
using Random = UnityEngine.Random;

namespace _Train.Scripts.Level.Items.Data
{
    [CreateAssetMenu(fileName = "ItemsConfig", menuName ="Data/ItemsConfig")]
    public class ItemsConfig : ScriptableObject
    {
        [SerializeField] private List<ItemData> items;
        
#if UNITY_EDITOR
        [Header("ForEditor")]
        [SerializeField] private List<LevelItem> loadPrefabs;
        [SerializeField] private List<Sprite> loadSprites;
        [SerializeField] private Sprite defaultSprite;

        [ContextMenu("Load Prefabs in Items")]
        private void LoadPrefabsInItems()
        {
            for (int i = 0; i < loadPrefabs.Count; i++)
            {
                items[i].Prefab = loadPrefabs[i];
            }
            
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Load Icons in Items")]
        private void LoadIconsInItems()
        {
            foreach (var item in items)
            {
                item.Icon = loadSprites.FirstOrDefault(p=>p.name == item.Prefab.name);
            }
            
            EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("Set Default Icon")]
        private void SetDefaultIcon()
        {
            foreach (var sprite in loadSprites)
            {
                var name = sprite.name.Replace("_", "");

                var item = items.FirstOrDefault(p => p.Prefab.name == name);
                
                if (item != null)
                {
                    item.Icon = sprite;
                }
            }
            
            EditorUtility.SetDirty(this);
        }
#endif

        public ItemData GetItemDataByPrefab(LevelItem obj)
        {
            return items.FirstOrDefault(p=> p.Prefab == obj);
        }

        public int GetIndexByItemData(ItemData item)
        {
            return items.IndexOf(item);
        }

        public ItemData GetItemDataByIndex(int index)
        {
            return items[index];
        }
        
        public ItemData GetRandomItemData()
        {
            if (items.Count == 1)
                return items[0];
            
            return items[Random.Range(0, items.Count)];
        }
    }

    [Serializable]
    public class ItemData
    {
        public Sprite Icon;
        public string Name;
        public bool IsDamageble;
        public int MaxHealth = 100;
        public float Price;
        public float Mass;
        public LevelItem Prefab;
        public Vector3 grabRotate;
        public Vector3 positionOffset;
    }
}
