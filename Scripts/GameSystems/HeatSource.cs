using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameSystems
{
    public class HeatSource : MonoBehaviour {
        public float heatPower = 15f;   // +15°C в центре
        public float radius = 5f;       // Радиус действия
        public bool isActive = true;

        private TemperatureSystem _tempSystem;

        void Start() {
            _tempSystem = FindObjectOfType<TemperatureSystem>();
            _tempSystem.AddHeatSource(this);
        }

        void OnDestroy() {
            if (_tempSystem != null)
                _tempSystem.RemoveHeatSource(this);
        }

        // Визуализация радиуса в редакторе
        void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}