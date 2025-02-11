using System.Globalization;
using GameSystems;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Indicators : MonoBehaviour
    {
        public Image healthBar, foodBar, waterBar, sleepBar,tempBar;
        public Text tempText;
        public float healthAmount = 100;
        public float foodAmount = 100;
        public float waterAmount = 100;
        public float sleepAmount = 100;
 
        public float secondsToEmptyFood = 500f;
        public float secondsToEmptyWater = 200f;
        private float _emptyWaterTempMultypl = 1f;
        public float secondsToEmptyHealth = 60f;
        public float secondsToEmptySleep = 300f;

        private TemperatureSystem _tempSystem;

        // Start is called before the first frame update
        void Start()
        {
            healthBar.fillAmount = healthAmount / 100;
            foodBar.fillAmount = foodAmount / 100;
            waterBar.fillAmount = waterAmount / 100;
            sleepBar.fillAmount = waterAmount / 100;
            _tempSystem = FindObjectOfType<TemperatureSystem>();
        }
 
        // Update is called once per frame
        void Update()
        {
            if (foodAmount > 0)
            {
                foodAmount -= 100 / secondsToEmptyFood * Time.deltaTime;
                foodBar.fillAmount = foodAmount / 100;
            }
            if (waterAmount > 0)
            {
                waterAmount -= 100 / (secondsToEmptyWater/_emptyWaterTempMultypl) * Time.deltaTime;
                waterBar.fillAmount = waterAmount / 100;
            }        
        
            if (sleepAmount > 0)
            {
                sleepAmount -= 100 / secondsToEmptySleep * Time.deltaTime;
                sleepBar.fillAmount = sleepAmount / 100;
            }
 
            if(foodAmount <= 0)
            {
                healthAmount -= 100 / secondsToEmptyHealth * Time.deltaTime;
            }
            if(waterAmount <= 0)
            {
                healthAmount -= 100 / secondsToEmptyHealth * Time.deltaTime;
            }
         
            
            float currentTemp = _tempSystem.CurrentTemperature;
            float roundTemp = math.round(currentTemp);
            tempText.text = roundTemp.ToString(CultureInfo.InvariantCulture) + " \u00b0C";
            float currentTempNorm = Mathf.Clamp(currentTemp, -20f, 40f);
            if (currentTempNorm > 30)
            {
                _emptyWaterTempMultypl = 1.5f;
            }

            float fillAmount = ConvertTempToFillAmount(currentTempNorm);
            tempBar.fillAmount = fillAmount;
            tempBar.color = InterpolateColor(fillAmount);
            
            if (currentTempNorm < 1f) {
                // Экстремальный холод: быстрый урон
                healthAmount -= (5f - currentTempNorm) * 0.2f * Time.deltaTime;
            }
            else if (currentTempNorm < 10f) {
                // Просто холодно: медленный урон
                healthAmount -= 0.05f * Time.deltaTime;
            }
            healthBar.fillAmount = healthAmount / 100;
            

        }
        
        float ConvertTempToFillAmount(float temperature)
        {
            temperature = Mathf.Clamp(temperature, -20f, 40f);
    
            if(temperature <= 22f)
            {
                float t = Mathf.InverseLerp(-20f, 22f, temperature);
                return Mathf.Pow(t, 0.7f) * 0.5f; // Ускоряем изменение в холодной зоне
            }
            else
            {
                float t = Mathf.InverseLerp(22f, 40f, temperature);
                return 0.5f + Mathf.Pow(t, 1.5f) * 0.5f; // Замедляем изменение в горячей зоне
            }
        }
        
        // Метод для интерполяции цвета
        Color InterpolateColor(float normalizedTemp)
        {
            // Определяем цвета для холодного и теплого диапазонов
            Color coldColor = new Color(0.317f, 1f, 1f, 0.727f); // B7FEFF с альфа 160 (160/255 ≈ 0.627)
            Color hotColor = new Color(1f, 0.843f, 0f, 0.627f); // FFD700 (Gold) с альфа 160

            // Интерполируем цвет в зависимости от normalizedTemp
            Color interpolatedColor = Color.Lerp(coldColor, hotColor, normalizedTemp);

            return interpolatedColor;
        }
        
        // Метод для нормализации температуры
        float NormalizeTemperature(float temp, float minTemp, float maxTemp)
        {
            // Смещаем шкалу так, чтобы 22 градуса соответствовали 0.5
            float midpoint = 22f;
            float range = maxTemp - minTemp;

            // Нормализуем температуру относительно середины
            float normalized = (temp - midpoint) / (range / 2f);

            // Ограничиваем значения от -1 до 1
            normalized = Mathf.Clamp(normalized, -1f, 1f);

            // Применяем нелинейность для более быстрого изменения влево и медленного вправо
            // Используем квадратичную функцию для создания нелинейности
            if (normalized <= 0f)
            {
                normalized = Mathf.Pow(normalized + 1f, 2f) / 4f;
            }
            else
            {
                normalized = 1f - Mathf.Pow(1f - normalized, 2f) / 4f;
            }

            // Нормализуем обратно в диапазон от 0 до 1
            normalized = (normalized + 1f) / 2f;

            return normalized;
        }
        public void IncreaseHealth(float amount)
        {
            healthAmount += amount;
        }

        public float GetHealth()
        {
            return healthAmount;
        }
    }
}