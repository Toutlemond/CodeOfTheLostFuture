using UnityEngine;

namespace GameSystems
{
    public class DayNightCycle : MonoBehaviour
    {
        public float dayDuration = 60f; // 10 минут в секундах
        public Gradient skyColorGradient; // Градиент для цвета неба
        public Light sunLight;
        private float _timeOfDay = 0.5f;
        public bool isDaytime;
        public Material skyboxMaterial;
        public Material daySkybox;
        public Material nightSkybox;
        private Material _currentSkyboxMaterial;
    
        void Start() {
            // Создаём динамический материал для плавного перехода
            _currentSkyboxMaterial = new Material(daySkybox);
            RenderSettings.skybox = _currentSkyboxMaterial;
        }
    
        void Update() {
            _timeOfDay += Time.deltaTime / dayDuration;
            _timeOfDay %= 1f; // Зацикливаем от 0 до 1
            isDaytime = _timeOfDay > 0.25f && _timeOfDay < 0.75f;
            UpdateAmbientLight();
            UpdateFog();
            UpdateSkyboxOld();
            // UpdateSunRotation();
        }
        public float NightProgress {
            get {
                if (isDaytime) return 0f;
                // Нормализуем время ночи от 0 до 1
                return Mathf.Clamp01((_timeOfDay - 0.95f) / 0.25f);
            }
        }

        void UpdateSunRotation()
        {
            sunLight.transform.rotation = Quaternion.Euler(new Vector3(
                Mathf.Lerp(-90f, 270f, _timeOfDay), // Угол для плавного движения солнца
                0f,
                0f
            ));
        }

        // В скрипте DayNightCycle
        void UpdateAmbientLight() {
            if (!isDaytime) {
                RenderSettings.ambientIntensity = 0.05f; // Почти полная темнота
            } else {
                RenderSettings.ambientIntensity = 0.8f; // Дневной свет
            }
        }
        
        void UpdateFog() {
            if (!isDaytime) {
                RenderSettings.fog = true;
                RenderSettings.fogDensity = 0.15f; // Густой туман
                RenderSettings.fogColor = Color.black;
            } else {
                RenderSettings.fog = false;
            }
        }
        void UpdateSkybox() {
            // Линейное изменение для проверки
            float blend = _timeOfDay; // 0..1

            // Смешивание цвета неба
            Color dayColor = daySkybox.GetColor("_Tint");
            Color nightColor = nightSkybox.GetColor("_Tint");
            _currentSkyboxMaterial.SetColor("_Tint", Color.Lerp(dayColor, nightColor, blend));

            // Смешивание текстур
            if (daySkybox.HasProperty("_Tex") && nightSkybox.HasProperty("_Tex")) {
                Texture dayTex = daySkybox.GetTexture("_Tex");
                Texture nightTex = nightSkybox.GetTexture("_Tex");
                if (blend < 0.5f) {
                    _currentSkyboxMaterial.SetTexture("_Tex", dayTex);
                } else {
                    _currentSkyboxMaterial.SetTexture("_Tex", nightTex);
                }
            }
            else
            {
                Debug.Log("No Tex Set");
            }
        }
    
        void UpdateSunRotationOld()
        {
            sunLight.transform.rotation = Quaternion.Euler(_timeOfDay * 360f - 90f, 0f, 0f);
        }    
        void UpdateSkyboxOld()
        {
            // Плавное изменение цвета неба и освещения
            RenderSettings.skybox.SetColor("_SkyTint", skyColorGradient.Evaluate(_timeOfDay));
            sunLight.transform.rotation = Quaternion.Euler(_timeOfDay * 360f - 90f, 0f, 0f);
            //skyboxMaterial.SetFloat("_Blend", Mathf.Sin(_timeOfDay * Mathf.PI * 2f) * 0.5f + 0.5f);
        }
    }
}
