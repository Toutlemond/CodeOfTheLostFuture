using UI.BotMenuItems;
using UnityEngine;

namespace Al
{
    public class BotEnergy : MonoBehaviour, IInteractable
    {
        public GameObject batteryPrefab; // Префаб батареи, который будет прикреплен
        public GameObject batteryNoPhPrefab; // Префаб батареи, без физики
        public Transform batteryAttachPoint; // Точка крепления батареи на роботе
        private Battery _currentBattery;

        [ContextMenu("NewBattery")]
        public void NewBattery()
        {
            if (_currentBattery != null)
            {
                Debug.LogWarning("Уже прикреплена батарея!");
                return;
            }

            // Создаем экземпляр префаба батареи
            GameObject batteryObject = Instantiate(batteryNoPhPrefab, batteryAttachPoint.position,
                batteryAttachPoint.rotation, batteryAttachPoint);

            Debug.Log(transform.localScale.x);
            // Устанавливаем исходный масштаб батареи тут 0.2 это прописано в батареи
            batteryObject.transform.localScale = new Vector3(0.2f / transform.localScale.x,
                0.2f / transform.localScale.y,
                0.2f / transform.localScale.z); // Устанавливаем масштаб в 1, чтобы избежать уменьшения
            _currentBattery = batteryObject.GetComponent<Battery>();

            Debug.Log("Батарея прикреплена: " + batteryObject.name);
        }

        [ContextMenu("AttachBattery")]
        public void AttachBattery(Battery battery)
        {
            if (_currentBattery != null)
            {
                Debug.LogWarning("Уже прикреплена батарея!");
                return;
            }

            // Создаем экземпляр префаба батареи
            GameObject batteryObject = Instantiate(batteryNoPhPrefab, batteryAttachPoint.position,
                batteryAttachPoint.rotation, batteryAttachPoint);

            _currentBattery = batteryObject.GetComponent<Battery>();
            _currentBattery.SetCharge(battery.GetCharge());

            Debug.Log("Батарея прикреплена: " + batteryObject.name);
        }

        [ContextMenu("DetachBattery")]
        public void DetachBattery()
        {
            if (_currentBattery == null)
            {
                Debug.LogWarning("Нет прикрепленной батареи для удаления!");
                return;
            }

            float charge = _currentBattery.GetCharge();
            Destroy(_currentBattery.gameObject); // Удаляем батарею из игры
            _currentBattery = null; // Обнуляем ссылку на текущую батарею


            // Создаем экземпляр префаба батареи
            GameObject batteryObject = Instantiate(batteryPrefab, batteryAttachPoint.position,
                batteryAttachPoint.rotation, batteryAttachPoint);
            Battery freeBattery = batteryObject.GetComponent<Battery>();
            freeBattery.SetCharge(charge);
            batteryObject.transform.SetParent(null); // Убираем батарею из иерархии робота
            batteryObject.transform.localScale =
                new Vector3(0.2f, 0.2f, 0.2f); // Устанавливаем масштаб в 1, чтобы избежать уменьшения
            batteryObject.transform.position = new Vector3(batteryAttachPoint.transform.position.x + 0.5f,
                batteryAttachPoint.position.y,
                batteryAttachPoint.transform.position.z); // Устанавливаем высоту над полом

            Debug.Log("Батарея удалена: ");
        }

        public float GetEnergyLevel()
        {
            if (_currentBattery != null)
            {
                return _currentBattery.GetCharge();
            }

            return 0f; // Если батареи нет, возвращаем 0
        }

        public void UseEnergy(float amount)
        {
            if (_currentBattery != null)
            {
                _currentBattery.UseCharge(amount);
            }
        }

        public bool ExecuteCommand(string command)
        {
                switch (command)
                {
                    case "NewBattery":
                        NewBattery();
                        return true;
                        break;
                    case "AttachBattery":
                        //AttachBattery();
                        Debug.Log("Доработай аттач через инвернтарь или ячейки доступа");
                        return true;
                        break;
                    case "DetachBattery":
                        DetachBattery();
                        return true;
                        break;
                }
                return false;
        }
    }
}