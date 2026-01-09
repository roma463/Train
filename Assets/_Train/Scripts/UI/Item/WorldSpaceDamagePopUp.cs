using TMPro;
using UnityEngine;

namespace _Train.Scripts.UI.Item
{
    public class WorldSpaceDamagePopUp : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Canvas canvas;
        [SerializeField] private float lifeTime;

        private Transform _cameraTransform;
    
        public void Setup(Camera direction, int damage)
        {
            _cameraTransform = direction.transform;
            canvas.worldCamera = direction;
            text.text = damage.ToString(); 
        
            Invoke(nameof(Hide), lifeTime);
        }

        private void Update()
        {
            transform.LookAt(_cameraTransform);
        }

        private void Hide()
        {
            Destroy(gameObject);
        }
    }
}
