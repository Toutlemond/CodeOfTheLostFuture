using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.AI;

//https://youtu.be/-ctJjlZl2s8
namespace Al
{
    public class OldBot : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        [SerializeField] private float movementSpeed;

        [SerializeField] private float changePositionTime = 5f;
        [SerializeField] private float moveDistance = 10f;

        private bool _isIdle = true;
        private bool _isFirstStand = true;
        private bool _isAttack = false;
        public Transform player; // Ссылка на объект игрока
        public float detectionRadius = 10f; // Радиус обнаружения
        public float attackDistance = 2.5f; // Дистанция для атаки
        public float attackCooldown = 1f; // Время между атаками
        public int damage = 10; // Урон от атаки
        private float _attackTimer;
        private float _distanceToPlayer;

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.speed = movementSpeed;
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            // Проверяем расстояние до игрока
            Vector3 playerPos = player.position;
            _distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (_distanceToPlayer <= detectionRadius)
            {
                _isIdle = false;
                if (_isFirstStand)
                {
                    Debug.Log("FirstStandCoroutine : " + StartCoroutine(FirstStandCoroutine()));
                }else
                {
                    // Если игрок в радиусе обнаружения, прекращаем случайное движение
                    _navMeshAgent.SetDestination(player.position);
                    _animator.SetFloat("Speed", 0.55f);
                    if (_distanceToPlayer <= attackDistance)
                    {
                        // Если игрок в пределах дистанции атаки
                        AttackPlayer();
                    }
                }
            }
            else
            {
                _animator.SetFloat("Speed", 0);
                _isIdle = true;
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
                _animator.SetTrigger("Attack");
            }
            else
            {
                _attackTimer -= Time.deltaTime;
            }
        }

        Vector3 RandomNavSphere(float distance)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
            randomDirection += transform.position;
            NavMeshHit navHit;

            NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);

            return navHit.position;
        }

        private IEnumerator FirstStandCoroutine()
        {
            _animator.SetBool("IsActivated", true);
            _animator.SetBool("FirstStand", true);
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(1f);
            }
            _animator.SetBool("FirstStand", false);
            _isFirstStand = false;
            yield return true;
        }
    }
}