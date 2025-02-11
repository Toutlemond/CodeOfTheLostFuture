using Player;
using UnityEngine;
using UnityEngine.AI;

//https://youtu.be/-ctJjlZl2s8
namespace Al
{
    public class WolfAi : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        [SerializeField] private float movementSpeed;
   
        [SerializeField] private float changePositionTime = 5f;
        [SerializeField] private float moveDistance = 10f;
   
        private bool _isRandomMoving;
        public Transform player; // Ссылка на объект игрока
        public float detectionRadius = 10f; // Радиус обнаружения
        public float attackDistance = 2.5f; // Дистанция для атаки
        public float attackCooldown = 1f; // Время между атаками
        public int damage = 10; // Урон от атаки
        private float _attackTimer;
 
        private void Start()
        {
            player = GameObject.FindWithTag("Player").transform; // Убедитесь, что тег установлен правильно
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = movementSpeed;
            _animator = GetComponent<Animator>();
            InvokeRepeating(nameof(MoveAnimal),changePositionTime,changePositionTime);
            _isRandomMoving = true;
        }
 
        private void Update()
        {
            // Проверяем расстояние до игрока
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRadius)
            {
                // Если игрок в радиусе обнаружения, прекращаем случайное движение
                //_navMeshAgent.isStopped = false;
                _isRandomMoving = false;
                CancelInvoke(nameof(MoveAnimal));
                _navMeshAgent.SetDestination(player.position);
                _animator.SetFloat("Speed",0.55f);

                if (distanceToPlayer <= attackDistance)
                {
                    // Если игрок в пределах дистанции атаки
                    AttackPlayer();
                }
            }
            else
            {
                // Если игрок вне радиуса обнаружения, продолжаем случайное движение
                _animator.SetFloat("Speed",_navMeshAgent.velocity.magnitude/movementSpeed);
                if (_isRandomMoving == false)
                {
                    InvokeRepeating(nameof(MoveAnimal),changePositionTime,changePositionTime);
                    _isRandomMoving = true;
                }
            }
        }
        private void AttackPlayer()
        {
            if (_attackTimer <= 0f)
            {
                // Здесь вы можете вызвать метод для уменьшения здоровья игрока
                player.GetComponent<PlayerHealth>().TakeDamage(damage);
                _attackTimer = attackCooldown;

                // Здесь можно добавить анимацию атаки
                // animator.SetTrigger("Attack");
            }
            else
            {
                _attackTimer -= Time.deltaTime;
            }
        }
        Vector3 RandomNavSphere (float distance) {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
            randomDirection += transform.position;
            NavMeshHit navHit;
           
            NavMesh.SamplePosition (randomDirection, out navHit, distance, -1);
           
            return navHit.position;
        }

        private void MoveAnimal()
        {
            _navMeshAgent.SetDestination(RandomNavSphere(moveDistance));
        }
    }
}