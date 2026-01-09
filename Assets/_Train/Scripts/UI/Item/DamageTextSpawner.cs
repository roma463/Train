using System;
using UnityEngine;

namespace _Train.Scripts.UI.Item
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private WorldSpaceDamagePopUp prefab;
        [SerializeField] private float distanceForSpawn;

        [SerializeField] private Camera camera;

        public void TrySpawn(Vector3 pos, int damage)
        {
            if (Vector3.Distance(pos, camera.transform.position) <= distanceForSpawn)
            {
                var damageViewer = Instantiate(prefab, pos, Quaternion.identity);
                damageViewer.Setup(camera, damage);
            }
        }
    }
}
