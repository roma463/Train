using UnityEngine;
using UnityEngine.UI;

namespace _Train.Scripts.UI.Root
{
    public class PopupView : MonoBehaviour
    {
        [SerializeField] private GameObject elements;
        [SerializeField] private Button backgroundHide;
        [SerializeField] private Button closeButton;

        private void Start()
        {
            backgroundHide.onClick.AddListener(Hide);
            closeButton.onClick.AddListener(Hide);
        }

        public void Show()
        {
            elements.SetActive(true);
            OnShowAfter();
        }

        public void Hide()
        {
            elements.SetActive(false);
            OnHideAfter();
        }
        
        protected virtual void OnShowAfter()
        {

        }

        protected virtual void OnHideAfter()
        {

        }
    }
}
