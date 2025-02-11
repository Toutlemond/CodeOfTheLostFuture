using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UI
{
    [CreateAssetMenu(fileName = "DialogEvent", menuName = "Scriptable Objects/DialogEvent")]
    public class DialogEvent : ScriptableObject
    {
        public UnityEvent onSendMessage;

        public void Send() {
            onSendMessage.Invoke();
        }
    }
}
