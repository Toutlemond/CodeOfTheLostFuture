using Al.States;
using UnityEngine;
using UnityEngine.AI;

namespace Al
{
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] protected float movementSpeed;
        [SerializeField] protected float changePositionTime = 5f;
        [SerializeField] protected float moveDistance = 10f;

        public IEnemyState _currentState;
        protected NavMeshAgent _navMeshAgent;
        protected Animator _animator;
        protected MeshCollider _collider;

        public Transform player = null; // Ссылка на объект игрока
        public float detectionRadius = 10f; // Радиус обнаружения
        public float attackDistance = 2.5f; // Дистанция для атаки
        public float attackCooldown = 1f; // Время между атаками
        public int damage = 2; // Урон от атаки
        protected float _attackTimer;
        protected float _distanceToPlayer;
        public AudioSource StepsAudio;

        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform; // Убедитесь, что тег установлен правильно
            if (player == null)
            {
                throw new System.ArgumentNullException("player");
            }
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = movementSpeed;
            _animator = GetComponent<Animator>();
            _collider = GetComponent<MeshCollider>();
            _distanceToPlayer = Vector3.Distance(transform.position, player.position);
            // Пример использования IdleState с разными действиями для разных врагов
            GoToDeactivated();
        }

        private void Update()
        {
            _distanceToPlayer = Vector3.Distance(transform.position, player.position);
            _currentState?.Update();
        }

        public void TransitionToState(IEnemyState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter(this);
        }

        // Методы для переходов между состояниями
        public void GoToDeactivated() => TransitionToState(new DeactivatedState(this, OnDeactivated, StartDeactivated));
        public void GoToIdle() => TransitionToState(new IdleState(this, OnIdle, StartIdle));
        public void GoToWalking() => TransitionToState(new WalkingState(this, OnWalking, StartWalking));
        public void GoToRunning() => TransitionToState(new RunningState(this, OnRunning, StartRunning));
        public void GoToAttacking() => TransitionToState(new AttackingState(this, OnAttacking, StartAttacking));
        public abstract void OnDeactivated();
        public abstract void StartDeactivated();
        public abstract void OnIdle();
        public abstract void StartIdle();
        public abstract void OnWalking();
        public abstract void StartWalking();
        public abstract void OnRunning();
        public abstract void StartRunning();
        public abstract void OnAttacking();
        public abstract void StartAttacking();
    }
}