using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class Storyteller : MonoBehaviour
    {
        public enum TriggerType { Proximity, KeyPress }

        [System.Serializable]
        public class StoryMessage
        {
            public string text;
            public MessageSystem.MessageType type; 
            public TriggerType trigger;
            public float proximityRadius = 5f; // Радиус для триггера приближения
        }

        public List<StoryMessage> messages = new List<StoryMessage>();
        private int _currentMessageIndex = 0;

        private bool _isPlayerNearby = false;
        private GameObject _player;

        void Start()
        {
            // Находим игрока в сцене (предполагается, что у игрока есть тег "Player")
            _player = GameObject.FindGameObjectWithTag("Player");
            if (_player == null)
            {
                Debug.LogWarning("Игрок не найден! Убедитесь, что у игрока установлен тег 'Player'.");
            }
        }

        void Update()
        {
            if (!_player)
                return;

            // Проверяем, есть ли еще сообщения для отправки
            if (_currentMessageIndex < messages.Count)
            {
                StoryMessage currentMessage = messages[_currentMessageIndex];

                switch (currentMessage.trigger)
                {
                    case TriggerType.Proximity:
                        CheckProximity(currentMessage);
                        break;
                    case TriggerType.KeyPress:
                        CheckKeyPress(currentMessage);
                        break;
                }
            }
        }

        void CheckProximity(StoryMessage message)
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance <= message.proximityRadius && !_isPlayerNearby)
            {
                _isPlayerNearby = true;
                SendMessageToSystem(message);
            }
            else if (distance > message.proximityRadius && _isPlayerNearby)
            {
                _isPlayerNearby = false;
            }
        }

        void CheckKeyPress(StoryMessage message)
        {
            if (Input.GetKeyDown(KeyCode.E) && IsPlayerInRange())
            {
                Debug.Log("send message");
                SendMessageToSystem(message);
            }
        }

        bool IsPlayerInRange()
        {
            if (_player == null)
                return false;

            float distance = Vector3.Distance(transform.position, _player.transform.position);
            // Предполагаем, что радиус для ключевого нажатия равен 3
            return distance <= 3f;
        }

        void SendMessageToSystem(StoryMessage message)
        {
            MessageSystem.Instance.AddMessage(message.text, message.type);
            _currentMessageIndex++;
        }
    }
}