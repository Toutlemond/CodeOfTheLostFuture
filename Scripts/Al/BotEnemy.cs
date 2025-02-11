using System.Collections;
using Al.States;
using Player;
using UnityEngine;

namespace Al
{
    public class BotEnemy : Enemy
    {
        private bool _isDeactivated;
        private bool _isMoving;
        private bool _isAttacking;
        private bool _isCoroutine = false;


        public override void StartDeactivated()
        {
            _isDeactivated = true;
            _isMoving = false;
            _isAttacking = false;
        }

        public override void OnDeactivated()
        {
            // Проверяем расстояние до игрока
            if (_distanceToPlayer <= detectionRadius)
            {
                if (_isDeactivated)
                {
                    StartCoroutine(FirstStandCoroutine());
                }

                if (_isCoroutine == false)
                {
                    Debug.Log("StartCoroutine is over");
                    GoToRunning();
                }
            }
            else
            {
                if (!_isDeactivated && _isCoroutine == false)
                {
                    GoToIdle();
                }
            }
        }

        public override void OnIdle()
        {
            if (_distanceToPlayer <= detectionRadius)
            {
                GoToRunning();
            }
        }

        public override void StartIdle()
        {
            _isMoving = false;
            _isAttacking = false;
            _animator.SetBool("moving", false);
            _animator.SetBool("attack", false);
            _animator.SetFloat("speed", 0);
        }

        public override void OnWalking()
        {
            MoveToPlayer();
        }

        public override void StartWalking()
        {
            throw new System.NotImplementedException();
        }

        public override void OnRunning()
        {
            MoveToPlayer();
        }

        private void MoveToPlayer()
        {
            if (_distanceToPlayer <= detectionRadius)
            {
                if (_distanceToPlayer > attackDistance)
                {
                    _navMeshAgent.SetDestination(player.position);
                }
                else
                {
                    //_animator.SetFloat("speed", 0);
                    _navMeshAgent.ResetPath();
                    // TransitionToState(new IdleState(this, OnIdle, StartIdle));
                    Vector3 direction = (player.position - transform.position).normalized;
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation =
                        Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Плавный поворот
                    GoToAttacking();
                }
            }
            else
            {
                GoToIdle();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.tag);
        }

        public override void StartRunning()
        {
            _isMoving = true;
            _isAttacking = false;
            _animator.SetFloat("speed", 0.95f);
            _animator.SetBool("moving", true);
            _animator.SetBool("attack", false);
        }

        private void AttackPlayer()
        {
            if (_attackTimer <= 0f)
            {
                _animator.SetBool("attack", true);
                _animator.SetBool("moving", false);
                _attackTimer = attackCooldown;
                _isMoving = false;
                _isAttacking = true;
                // todo унеси в константы
            }
            else
            {
                _attackTimer -= Time.deltaTime;
            }

            if (_attackTimer <= 1.6f && _attackTimer >= 0.6f && _distanceToPlayer < 1.6f)
            {
                player.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }

        public override void OnAttacking()
        {
            if (_distanceToPlayer >= attackDistance)
            {
                _attackTimer = 0;
                GoToRunning();
            }
            else
            {
                AttackPlayer();
            }
        }

        public override void StartAttacking()
        {
            _isAttacking = true;
        }

        private IEnumerator FirstStandCoroutine()
        {
            _isDeactivated = false;
            _isCoroutine = true;

            _animator.SetBool("deactivated", false);
            _animator.SetBool("firstStand", true);
            for (int i = 0; i < 6; i++)
            {
                yield return new WaitForSeconds(1f);
            }

            _animator.SetBool("firstStand", false);
            _isCoroutine = false;
            yield return true;
        }
    }
}