using TMPro;
using UnityEngine;

namespace _Train.Scripts.UI
{
    public class InteractableView : MonoBehaviour
    {
        public static InteractableView Instance { get; private set; }
    
        [SerializeField] private GameObject elements;
        [SerializeField] private TMP_Text title;

        private void Start()
        {
            Instance = this;
        }

        public void Show(string info)
        {
            title.text = info;
            elements.SetActive(true);
        }

        public void Hide()
        {
            elements.SetActive(false);
        }
    }
}
