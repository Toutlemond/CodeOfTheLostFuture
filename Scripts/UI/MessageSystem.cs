using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class MessageSystem : MonoBehaviour
    {
        public static MessageSystem Instance { get; private set; }

        public enum MessageType { Normal, Important, Note }

        [System.Serializable]
        public class Message
        {
            public string text;
            public MessageType type;
        }

        private Queue<Message> _messageQueue = new Queue<Message>();
        public GameObject subtitlePanel; // Префаб или объект, который будет отображать текст
        private TextMeshPro _subtitleText;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            if (subtitlePanel != null)
            {
                _subtitleText = subtitlePanel.GetComponentInChildren<TextMeshPro>();
                subtitlePanel.SetActive(false);
            }
        }

        public void AddMessage(string text, MessageType type)
        {
            Message newMessage = new Message { text = text, type = type };
            _messageQueue.Enqueue(newMessage);
            Debug.Log("Got message: " + text);
            if (subtitlePanel != null && !subtitlePanel.activeInHierarchy)
            {
                ShowNextMessage();
            }
        }

        private void ShowNextMessage()
        {
            Debug.Log("_messageQueue.Count:" + _messageQueue.Count);
            if (_messageQueue.Count > 0)
            {
                Message currentMessage = _messageQueue.Dequeue();
                
                if (_subtitleText != null)
                {
                    _subtitleText.text = currentMessage.text;
                    Debug.Log(currentMessage.text);
                    subtitlePanel.SetActive(true);
                }
            }
        }

        public void AcknowledgeMessage()
        {
            if (subtitlePanel != null)
            {
                subtitlePanel.SetActive(false);
                ShowNextMessage();
            }
        }
    }
}