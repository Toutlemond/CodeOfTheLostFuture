using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class TemperatureSystem : MonoBehaviour {
        // Основные настройки
        public float baseDayTemp = 25f;    // Дневная температура
        public float baseNightTemp = 10f;  // Ночная температура
        public float tempChangeSpeed = 0.1f; // Скорость изменения (0.1 = плавно)
    
        // Температурные зоны
        public float caveTemp = 5f;
        public float mountainTemp = 0f;
    
        // Кривая для суточных колебаний (в Inspector можно "рисовать" график)
        public AnimationCurve dailyTemperatureCurve;

        private float _currentTemp;
        private float _targetTemp;
        private bool _isInCave, _isInMountain;
    
        // Список активных источников тепла
        private List<HeatSource> _activeHeatSources = new List<HeatSource>();
        private DayNightCycle _dayNight;
        private Transform _player;

        void Start()
        {
            _dayNight = GetComponent<DayNightCycle>();
            _currentTemp = baseDayTemp;
            _player = GameObject.FindWithTag("Player").transform; // Убедитесь, что тег установлен правильно

        }

        void Update() {
            UpdateTargetTemperature(); // Пересчёт целевой температуры
            _currentTemp = Mathf.Lerp(_currentTemp, _targetTemp, tempChangeSpeed * Time.deltaTime);
        }

        void UpdateTargetTemperature() {
            // Базовый расчёт (день/ночь + зоны)
            float environmentTemp = CalculateEnvironmentTemp();
        
            // Эффект от источников тепла
            float heatEffect = CalculateHeatEffect();
        
            _targetTemp = environmentTemp + heatEffect;
        }

        float CalculateEnvironmentTemp() {
            float temp = baseDayTemp;
        
            // Если ночь, интерполируем между днём и ночью
            if (!_dayNight.isDaytime) {
                //float nightProgress = _dayNight.NightProgress; // 0-1
               // temp = Mathf.Lerp(baseDayTemp, baseNightTemp, nightProgress);
                temp = baseNightTemp;
            }
        
            // Приоритет зон: пещера > горы > общая температура
            if (_isInCave) return caveTemp;
            if (_isInMountain) return mountainTemp;
            return temp;
        }

        float CalculateHeatEffect() {
            float totalHeat = 0f;
            foreach (HeatSource source in _activeHeatSources) {
                if (source.isActive) {
                    float distance = Vector3.Distance(_player.position, source.transform.position);
                    //Debug.Log("dist : " + distance);
                    if (distance <= source.radius) {
                        // Линейное затухание с расстоянием
                        float effect = source.heatPower * (1 - distance / source.radius);
                        totalHeat += effect;
                    }
                }
            }
            //Debug.Log("totalHeat : " + totalHeat);
            return totalHeat;
        }

        // Методы для зон (вызываются через триггеры)
        public void EnterCave() => _isInCave = true;
        public void ExitCave() => _isInCave = false;
        public void EnterMountain() => _isInMountain = true;
        public void ExitMountain() => _isInMountain = false;

        // Методы для источников тепла
        public void AddHeatSource(HeatSource source) => _activeHeatSources.Add(source);
        public void RemoveHeatSource(HeatSource source) => _activeHeatSources.Remove(source);

        public float CurrentTemperature => _currentTemp;
    }
}