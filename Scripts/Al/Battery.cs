using System;
using UnityEngine;

namespace Al
{
    public class Battery : MonoBehaviour
    {
        [Range(0, 1)]
        public float charge = 1.0f; // Уровень заряда батареи от 0 до 1
        public float current = 1.0f; // Это не ток, но это  параметр указывающий как много тратится за шаг. чем меньше тем лучше 
        private Renderer _batteryRenderer;
        private Material _material;
        private Rigidbody _batteryRigidbody;
        
        void Start()
        {
            _batteryRenderer = GetComponent<MeshRenderer>();
            if (_batteryRenderer != null)
            {
                _material= _batteryRenderer.material;
                if (_batteryRenderer.material == null)
                {
                    Debug.LogWarning("Battery renderer material not found");
                }
            }else
            {
                Debug.LogWarning("Battery renderer is null!");
            }
            _batteryRigidbody = GetComponent<Rigidbody>();
            UpdateBatteryVisual();
        }

        private void Awake()
        {
            _batteryRigidbody = GetComponent<Rigidbody>();
        }

        public void SetStatic(bool isStatic)
        {
            _batteryRigidbody.useGravity = !isStatic;
            _batteryRigidbody.freezeRotation = isStatic;
            //_batteryRigidbody.detectCollisions = !isStatic;
            
            //_batteryRigidbody.isKinematic = isKinematic;
        }

        private void Update()
        {
            UpdateBatteryVisual();
        }

        public void UseCharge(float amount)
        {
            float chargeAmount = amount * current;
            charge -= chargeAmount;
            charge = Mathf.Clamp(charge, 0f, 1f); // Ограничиваем уровень заряда от 0 до 1
            UpdateBatteryVisual();
        }

        private void UpdateBatteryVisual()
        {
            // Изменяем материал в зависимости от уровня заряда
            if (_material != null)
            {
                _material.SetFloat("_Charge", charge); // Предполагаем, что у вас есть параметр _Charge в материале
            }
            else
            {
                Debug.LogWarning("Battery renderer _material is null!");
            }
        }
        // Метод для получения текущего уровня заряда
        public float GetCharge()
        {
            return charge;
        }

        // Метод для установки уровня заряда
        public void SetCharge(float newCharge)
        {
            charge = Mathf.Clamp(newCharge, 0f, 1f);
            UpdateBatteryVisual();
        }
    }
}
