using UnityEngine;

namespace Inventory
{
   [CreateAssetMenu(fileName = "FoodItem", menuName = "Inventory/Items/FoodItem")]
   public class FoodItem : ItemScriptableObject
   {
      public float healAmonut;

      private void Start()
      {
       itemType = ItemType.Food;
      }
   }
}
