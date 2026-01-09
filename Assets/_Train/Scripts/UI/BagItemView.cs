using UnityEngine;
using UnityEngine.UI;

namespace _Train.Scripts.UI
{
    public class BagItemView : MonoBehaviour
    {
        public PickupObject CurrentPickup { get; private set; }
        
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        
        public Button Button => button;
        
        public void Setup(PickupObject slot = null)
        {
            CurrentPickup = slot;

            if (slot != null)
            {
                icon.sprite = slot.LevelItem.ItemData.Icon;
            }
            icon.gameObject.SetActive(slot != null);
        }
    }
}