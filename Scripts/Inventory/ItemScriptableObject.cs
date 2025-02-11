using UnityEngine;

namespace Inventory
{
    public enum ItemType{Default,Food,Weapon,Instrumend}
    [CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/ItemScriptableObject")]
    public class ItemScriptableObject : ScriptableObject
    {
        public ItemType itemType;
        public string itemName;
        public int maximumAmount;
        public string itemDescription;
    }
}