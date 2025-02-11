using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        public float maxHealth = 100f;
        public float health = 100;
        public GameObject indicatorsGo;
        private Indicators _indicators;
    
        [Header("Effects")]
        public AudioClip deathSound;
        public GameObject deathEffect;
    
        [Header("Events")]
        public UnityEvent onDeath;
    
        private bool _isDead = false;
        private AudioSource _audioSource;
    
        void Start()
        {
            _indicators = indicatorsGo.GetComponent<Indicators>();
   
        }

        private void Update()
        {
            if (_isDead) return;
        
            health = _indicators.GetHealth();
            // Твоя существующая логика урона...
        
            if (health <= 0f)
            {
                Die();
            }
        }

        public void Die()
        {
            if (_isDead) return;
            _isDead = true;
        
            // Отключаем управление

            // Эффекты
            if (deathEffect != null)
                Instantiate(deathEffect, transform.position, Quaternion.identity);
        
            if (_audioSource != null && deathSound != null)
                _audioSource.PlayOneShot(deathSound);
        
            // Запускаем события смерти
            onDeath.Invoke();
        
            // Показываем курсор
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        
            // Вызываем окно смерти через 2 секунды
            //Invoke("ShowDeathScreen", 2f);
        }
    
        public void TakeDamage(int damage)
        {
            health -= damage;
            if (_indicators)
            {
                _indicators.IncreaseHealth(-damage);
            }
        }
    }
}