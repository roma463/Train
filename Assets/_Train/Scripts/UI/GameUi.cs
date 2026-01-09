using System.Collections.Generic;
using System.Linq;
using _Train.Scripts.Root;
using _Train.Scripts.UI.Item;
using _Train.Scripts.UI.Root;
using _Train.Scripts.UI.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Train.Scripts.UI
{
    public class GameUi : MonoBehaviour
    {
        public static GameUi Instance {get; private set;}
        
        [SerializeField] private Canvas canvas;
        [FormerlySerializedAs("itemDamageViewer")] [SerializeField] private DamageTextSpawner damageTextSpawner;
        [SerializeField] private GameObject grabItemNameObject;
        [SerializeField] private TMP_Text grabItemNameText;
        
        private List<Window> _windows = new List<Window>();

        private PauseWindow _pauseWindow;
        private Window _currentOpenedWindow;

        public DamageTextSpawner DamageTextSpawner => damageTextSpawner;

        public void Initialize()
        {
            Instance = this;
            _windows = GetComponentsInChildren<Window>().ToList();

            foreach (var window in _windows)
            {
                if (window is PauseWindow pauseWindow)
                    _pauseWindow = pauseWindow;
                
                window.Initialize();
                window.OnOpened += PerformOpenedWindow;
                window.OnClosed += PerformClosedWindow;
            }
            
            INPUTE.instance.OnPerformedEscape += PerformEscape;
        }

        private void OnDestroy()
        {
            foreach (var window in _windows)
            {
                window.OnOpened -= PerformOpenedWindow;
                window.OnClosed -= PerformClosedWindow;
            }
            
            INPUTE.instance.OnPerformedEscape -= PerformEscape;
        }
        
        private void PerformEscape()
        {
            if (_currentOpenedWindow != null)
            {
                _currentOpenedWindow.Hide();
            }
            else
            {
                _pauseWindow.Show();
            }
        }

        private void PerformOpenedWindow(Window window)
        {
            if (_currentOpenedWindow != null)
                _currentOpenedWindow.Hide();
            
            // GameStateController.Instance.ChangePausedState(true);
            _currentOpenedWindow = window;
        }

        private void PerformClosedWindow(Window window)
        {
            // GameStateController.Instance.ChangePausedState(false);
            
            if (_currentOpenedWindow == window)
                _currentOpenedWindow = null;
        }

        public void ShowGrabText(string text)
        {
            grabItemNameObject.SetActive(true);
            grabItemNameText.text = text;
        }

        public void HideGrabText()
        {
            grabItemNameObject.SetActive(false);
        }
    }
}
