using System.Collections;
using System.Collections.Generic;
using _Train.Scripts.Character.Data;
using _Train.Scripts.Character.MovementStateMachine;
using _Train.Scripts.UI;
using Mirror;
using UnityEngine;

namespace _Train.Scripts.Character
{
    public class Character : NetworkBehaviour, IDamageable
    {
        [field: SerializeField] public CharacterColllision Collision { get; private set; }
        [field: SerializeField] public float Gravity { get; private set; } = -9.81f;
        [field: SerializeField] public bool IsLockPlayer { get; private set; }

        [SerializeField] private CharacterBaseData characterBaseData;
        [SerializeField] private CapsuleCollider characterCollider;
        [SerializeField] private Rigidbody characterRigidbody;
        [SerializeField] private CharacterStateMachine _stateMachine;
        [SerializeField] private GameObject _localClient;
        [SerializeField] private GameObject _remoteClient;
        [SerializeField] private GameUi gameUi;
        [SerializeField] private SkinnedMeshRenderer[] _remoteSkinnedMeshRenderer;
        [SerializeField] private SkinnedMeshRenderer[] _localSkinnedMeshRenderer;
        [SerializeField] private CharacterContext characterContext;

        private Vector3 _startPosition;
        private Vector3 _externalVelocity;
        private Coroutine _lockStaminaRoutine;

        public float VerticalVelocity { get; set; }
        public bool IsLockRestoreStamina { get; set; }
        public bool IsWalking => _stateMachine.CurrentStateType == CharacterStateType.Walk;
        public bool IsRunning => _stateMachine.CurrentStateType == CharacterStateType.Run;
        public bool IsSitMoving => _stateMachine.CurrentStateType == CharacterStateType.SitMove;
        public CharacterContext CharacterContext => characterContext;
        
        public Rigidbody CharacterRigidbody => characterRigidbody;

        public CapsuleCollider CharacterCollider => characterCollider;
        
#if UNITY_EDITOR
        public bool IsInvisible { get; private set; }
#endif

        private void Awake()
        {
            _startPosition = transform.position;
        }
        
        public void SetExternalVelocity(Vector3 velocity) => _externalVelocity = velocity;  

        public void LockPlayer()
        {
            IsLockPlayer = true;
        }

        public void UnlockPlayer()
        {
            IsLockPlayer = false;
        }
        
        private void Start()
        {
            _remoteClient.SetActive(!isLocalPlayer);
            _localClient.SetActive(isLocalPlayer);
            characterRigidbody.isKinematic = !isLocalPlayer;

            if (isLocalPlayer)
            {
                Collision.OnDamagedByGround += DamageByGround;
                foreach (var skinnedMeshRenderer in _remoteSkinnedMeshRenderer)
                {
                    skinnedMeshRenderer.enabled = false;
                }
                
                Initialize();
            }
            else
            {
                characterRigidbody.useGravity = false;
            }
        }

        private void LockRestoreStamina(bool isFull)
        {
            if (!isFull && _lockStaminaRoutine == null)
            {
                _lockStaminaRoutine = StartCoroutine(DelayStartRestoreStamina());
            }
        }

        private IEnumerator DelayStartRestoreStamina()
        {
            IsLockRestoreStamina = true;
            
            yield return new WaitForSeconds(characterBaseData.TimeForStartRestoreStamina);
            
            IsLockRestoreStamina = false;
            _lockStaminaRoutine = null;
        }

        private void OnDestroy()
        {
            if (isLocalPlayer)
            {
                Collision.OnDamagedByGround -= DamageByGround;
                
                if (_lockStaminaRoutine != null)
                    StopCoroutine(_lockStaminaRoutine);
            }
        }

        private void DamageByGround(int groundDamage)
        {
            CmdTakeDamage(groundDamage);
        }

        public void ChangeMaterial(Material material)
        {
            foreach (var skinnedMeshRenderer in _remoteSkinnedMeshRenderer)
            {
                skinnedMeshRenderer.material = material;
            }

            foreach (var skinnedMeshRenderer in _localSkinnedMeshRenderer)
            {
                skinnedMeshRenderer.material = material;
            }
        }

        [Command(requiresAuthority = false)]
        public void ChangeToDeathMaterial()
        {
            RpcChangeToDeathMaterial();
        }

        [ClientRpc]
        private void RpcChangeToDeathMaterial()
        {
            ChangeMaterial(_stateMachine.Context.DeathMaterial);
        }

        public void Initialize()
        {
            _stateMachine.Initialize();
            gameUi.Initialize();
        }

        [Command(requiresAuthority = false)]
        private void CmdTakeDamage(int damage)
        {
            TakeDamage(damage);
        }

        [Server]
        public void TakeDamage(int damageAmount)
        {
            
            if (!isLocalPlayer)
                TrgTakeDamage(connectionToClient, damageAmount);
        }
        
        [TargetRpc]
        private void TrgTakeDamage(NetworkConnection target, int damage)
        {
        }
        
        private Vector3 CalculateCorrectParabolicVelocity(Vector3 direction, float totalForce, float height)
        {
            // Нормализуем направление и получаем горизонтальную компоненту
            var horizontalDir = new Vector3(direction.x, 0, direction.z).normalized;

            // Вертикальная скорость для достижения максимальной высоты
            var verticalSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * height);

            // Горизонтальная скорость (сохраняем общую силу через теорему Пифагора)
            var horizontalSpeed = Mathf.Sqrt(totalForce * totalForce - verticalSpeed * verticalSpeed);

            // Если verticalSpeed больше totalForce, ограничиваем ее
            if (float.IsNaN(horizontalSpeed))
            {
                horizontalSpeed = totalForce * 0.7f; // 70% силы на горизонталь
                verticalSpeed = totalForce * 0.3f; // 30% силы на вертикаль
            }

            var horizontalVelocity = horizontalDir * horizontalSpeed;
            var verticalVelocity = Vector3.up * verticalSpeed;

            return horizontalVelocity + verticalVelocity;
        }

        public void Move(Vector3 velocity, bool applyGravity = false)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            velocity += _externalVelocity;
            
            if (applyGravity)
            {
                ApplyGravity(velocity);
                characterRigidbody.linearVelocity = new Vector3(velocity.x, VerticalVelocity, velocity.z);
            }
            else
            {
                characterRigidbody.linearVelocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -characterBaseData.MaxVerticalVelocity, characterBaseData.MaxVerticalVelocity), velocity.z);
            }
            
            VerticalVelocity = characterRigidbody.linearVelocity.y;
        }
        
        private void ApplyGravity(Vector3 targetVeloсity)
        {
            var gravityEffect = _stateMachine.Context.Character.Gravity * Time.fixedDeltaTime;
            VerticalVelocity = targetVeloсity.y + gravityEffect;
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -characterBaseData.MaxVerticalVelocity, characterBaseData.MaxVerticalVelocity);
        }

        private IEnumerator StartRespawn()
        {
            yield return new WaitForSeconds(10f);
            
            Respawn();
        }
        
        public void Respawn()
        {
            CmdRespawnResetData();
        }

        [Command(requiresAuthority = false)]
        private void CmdRespawnResetData()
        {
            RpcRespawnResetData();
        }

        [ClientRpc]
        private void RpcRespawnResetData()
        {
            if (isLocalPlayer)
            {
                _stateMachine.Context.Character.UnlockPlayer();
                _stateMachine.Context.CameraController.UnlockCamera();
                _stateMachine.ChangeStateByType(CharacterStateType.Idle);
            }
            
            _stateMachine.Context.Character.ChangeMaterial(_stateMachine.Context.DefaultMaterial);
            characterRigidbody.position = _startPosition;
        }
        
#if UNITY_EDITOR
        public void ChangeInvisibilityState(bool isInvisible)
        {
            IsInvisible = isInvisible;
        }

        public void ChangeLocalCharacterView(bool isOn)
        {
            foreach (var skinnedMeshRenderer in _remoteSkinnedMeshRenderer)
            {
                skinnedMeshRenderer.enabled = isOn;
            }
        }
#endif

        #region Event Handlers

        private void OnDead()
        {
            StartCoroutine(StartRespawn());
        }

        #endregion
    }
}
