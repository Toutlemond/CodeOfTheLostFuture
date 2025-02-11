using UnityEngine;
using UnityEngine.Events;

namespace UI.BotMenuItems
{
    [CreateAssetMenu(fileName = "BotMenuItem", menuName = "Scriptable Objects/BotMenuItem")]
    public class BotMenuItem : ScriptableObject
    {
        public string itemName; // Имя пункта меню
        public string command; // команда 
    }
}
