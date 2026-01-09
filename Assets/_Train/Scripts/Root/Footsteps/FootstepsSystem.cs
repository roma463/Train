using System;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Train.Scripts.Root.Footsteps
{
    public class FootstepsSystem : NetworkBehaviour
    {
        [SerializeField] private SurfaceAudioPairsConfig audioPairsConfig;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Character.Character character;

        [SerializeField] private bool isSoundable;
        [SerializeField] private float raycastDistance = 1.2f;
        [SerializeField] private LayerMask groundLayerMask = 1;
        [SerializeField] private float footstepVolume = 30f;
        [SerializeField] private float jumpVolume = 60f;
        
        private bool _isGrounded;
        private Texture2D _currentTexture;
        private Renderer _lastHitRenderer;
        private Terrain _lastHitTerrain;
        
        private void Start()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (character != null)
            {
                character.Collision.OnGroundedPerformed += PlayGroundedSound;
            }
        }

        private void OnDestroy()
        {
            if (character != null)
            {
                character.Collision.OnGroundedPerformed -= PlayGroundedSound;
            }
        }
        
        private void Update()
        {
            if (character != null)
            {
                _isGrounded = character.Collision.IsGrounded;
            }

            // Определяем текстуру под ногами
            DetectSurfaceTexture();
        }

        private void DetectSurfaceTexture()
        {
            var rayStart = transform.position + Vector3.up * 0.1f;
            
            if (Physics.Raycast(rayStart, Vector3.down, out var hit, raycastDistance, groundLayerMask))
            {
                var detectedTexture = GetTextureFromHit(hit);
                
                if (detectedTexture != null)
                {
                    _currentTexture = detectedTexture;
                }
            }
        }

        private Texture2D GetTextureFromHit(RaycastHit hit)
        {
            // Проверяем Terrain first
            var terrain = hit.collider.GetComponent<Terrain>();
            
            if (terrain != null)
            {
                return GetTerrainTexture(terrain, hit.point);
            }

            // Проверяем обычный Mesh Renderer
            var meshRenderer = hit.collider.GetComponent<Renderer>();
            
            if (meshRenderer != null && meshRenderer.sharedMaterial != null)
            {
                return GetMeshTexture(meshRenderer, hit);
            }

            return null;
        }

        private Texture2D GetTerrainTexture(Terrain terrain, Vector3 worldPos)
        {
            // Получаем координаты текстуры в terrain splat map
            var terrainPos = worldPos - terrain.transform.position;
            var splatMapPos = new Vector3(
                terrainPos.x / terrain.terrainData.size.x,
                0,
                terrainPos.z / terrain.terrainData.size.z
            );

            var x = Mathf.FloorToInt(splatMapPos.x * terrain.terrainData.alphamapWidth);
            var z = Mathf.FloorToInt(splatMapPos.z * terrain.terrainData.alphamapHeight);

            x = Mathf.Clamp(x, 0, terrain.terrainData.alphamapWidth - 1);
            z = Mathf.Clamp(z, 0, terrain.terrainData.alphamapHeight - 1);

            // Получаем массив альфа-карт
            var alphaMaps = terrain.terrainData.GetAlphamaps(x, z, 1, 1);

            // Находим доминирующую текстуру
            var maxAlpha = 0f;
            var dominantIndex = 0;

            for (var i = 0; i < alphaMaps.GetLength(2); i++)
            {
                if (alphaMaps[0, 0, i] > maxAlpha)
                {
                    maxAlpha = alphaMaps[0, 0, i];
                    dominantIndex = i;
                }
            }

            // Возвращаем текстуру из материала terrain
            if (dominantIndex < terrain.terrainData.terrainLayers.Length)
            {
                return terrain.terrainData.terrainLayers[dominantIndex].diffuseTexture;
            }

            return null;
        }

        private Texture2D GetMeshTexture(Renderer renderer, RaycastHit hit)
        {
            // Получаем UV координаты точки попадания
            var uv = hit.textureCoord;

            // Получаем основную текстуру из материала
            var mainTexture = renderer.sharedMaterial.mainTexture as Texture2D;

            // Если есть дополнительная карта текстур (например, для смешивания)
            if (renderer.sharedMaterial.HasProperty("_DetailAlbedoMap"))
            {
                var secondaryTexture = renderer.sharedMaterial.GetTexture("_DetailAlbedoMap") as Texture2D;
            }

            // Здесь можно добавить логику для определения доминирующей текстуры
            // на основе UV координат и дополнительных карт

            return mainTexture; // Возвращаем основную текстуру по умолчанию
        }

        private bool IsMoving()
        {
            if (character != null)
            {
                return character.IsWalking;
            }
            return false;
        }
        
        private bool IsSitting()
        {
            if (character != null)
            {
                return character.IsSitMoving;
            }
            return false;
        }
        
        private bool IsRunning()
        {
            if (character != null)
            {
                return character.IsRunning;
            }
            return false;
        }
        
        private void PlayGroundedSound()
        {
            DetectSurfaceTexture();
            
            var currentSurface = GetSurfaceByTexture(_currentTexture);
            
            if (currentSurface != null)
            {
                audioSource.PlayOneShot(currentSurface.audioClips[Random.Range(0, currentSurface.audioClips.Length)], 1f);
            }
            else
            {
                // Fallback на первую поверхность, если текстура не найдена
                if (audioPairsConfig.SurfaceAudioPairs.Count > 0)
                {
                    var clip = audioPairsConfig.SurfaceAudioPairs[0].audioClips[Random.Range(0, audioPairsConfig.SurfaceAudioPairs[0].audioClips.Length)];
                    audioSource.PlayOneShot(clip, 1f);
                }
            }
        }

        private void PlayFootstep()
        {
            var currentSurface = GetSurfaceByTexture(_currentTexture);
            
            if (currentSurface != null)
            {
                audioSource.PlayOneShot(currentSurface.audioClips[Random.Range(0, currentSurface.audioClips.Length)]);
            }
            else
            {
                // Fallback на первую поверхность, если текстура не найдена
                if (audioPairsConfig.SurfaceAudioPairs.Count > 0)
                {
                    var clip = audioPairsConfig.SurfaceAudioPairs[0].audioClips[Random.Range(0, audioPairsConfig.SurfaceAudioPairs[0].audioClips.Length)];
                    audioSource.PlayOneShot(clip);
                }
            }
        }

        private SurfaceAudioPair GetSurfaceByTexture(Texture2D texture)
        {
            if (texture == null) return null;

            foreach (var surface in audioPairsConfig.SurfaceAudioPairs)
            {
                if (surface.surfaceTexture == texture)
                    return surface;
            }

            return null;
        }

        // Метод для отладки (можно вызвать из инспектора)
        public void DebugCurrentTexture()
        {
            if (_currentTexture != null)
            {
                Debug.Log($"Current texture: {_currentTexture.name}");
            }
            else
            {
                Debug.Log("No texture detected");
            }
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}