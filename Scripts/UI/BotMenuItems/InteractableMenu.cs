using System.Collections.Generic;
using UnityEngine;

namespace UI.BotMenuItems
{
    public class InteractableMenu : MonoBehaviour
    {
        public List<BotMenuItem> menuItems = new List<BotMenuItem>(); // Список пунктов меню
        public List<BotMenuItem> GetMenuItems()
        {
            return menuItems;
        }
        // Для дополнительной логики при необходимости
        public void HandleMenuAction(string actionName)
        {
            Debug.Log($"Выполняется действие: {actionName}");
        }
    }
}
