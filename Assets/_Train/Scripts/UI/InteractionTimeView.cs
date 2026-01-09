using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Train.Scripts.UI
{
    public class InteractionTimeView : MonoBehaviour
    {
        public event Action OnStartRetention;
        public event Action OnCompleteInteraction;
        
        public bool StartRetantion { get; private set; }
        
        [SerializeField] private Image image;
        [SerializeField] private GameObject elements;
        [SerializeField] private bool showOnStart;
        
        private Coroutine _coroutine;
        private bool _withComplete;

        private void Awake()
        {
            elements.SetActive(showOnStart);
        }

        public void Setup(float timeForStart, float time, bool withComplete)
        {
            _withComplete = withComplete;
            _coroutine = StartCoroutine(Animation(timeForStart, time));
        }

        private void OnDestroy()
        {
            Abort();
        }

        public void Abort()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            StartRetantion = false;
            Hide();
        }

        private IEnumerator Animation(float timeForStart, float time)
        {
            yield return new WaitForSeconds(timeForStart);
            
            Show();
            StartRetantion = true;
            OnStartRetention?.Invoke();
            float currentTime = 0;
            
            while (time >= currentTime)
            {
                currentTime += Time.deltaTime;
                image.fillAmount = currentTime / time;
                
                yield return null;
            }

            if (_withComplete)
            {
                OnCompleteInteraction?.Invoke();
                StartRetantion = false;
                Hide();
            }
        }
        
        private void Show()
        {
            elements.SetActive(true);
            
        }
        
        private void Hide()
        {
            elements.SetActive(false);
        }
    }
}
