using System;
using UnityEngine;

namespace _Train.Scripts.UI.Root
{
    public class Window : MonoBehaviour
    {
        public event Action<Window> OnOpened;
        public event Action<Window> OnClosed;
        
        [SerializeField] private GameObject elements;
        [SerializeField] private bool showOnStart;

        public virtual void Initialize()
        {
            elements.SetActive(showOnStart);
        }

        public void Show()
        {
            elements.SetActive(true);
            OnShowAfter();
            
            OnOpened?.Invoke(this);
        }
        
        public void Hide()
        {
            elements.SetActive(false);
            OnHideAfter();
            
            OnClosed?.Invoke(this);
        }

        protected virtual void OnShowAfter()
        {
            UnlockCursor();
        }

        protected virtual void OnHideAfter()
        {
            LockCursor();
        }
        
        private void LockCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void UnlockCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
