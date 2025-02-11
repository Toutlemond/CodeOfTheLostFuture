using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Player
{
    public class NightVision : MonoBehaviour {
        public PostProcessVolume volume;
        private ColorGrading _colorGrading;

        void Start() {
            volume.profile.TryGetSettings(out _colorGrading);
        }

        void Update() {
            if (HasLightSourceNearby()) {
                _colorGrading.postExposure.value = Mathf.Lerp(
                    _colorGrading.postExposure.value, 0f, Time.deltaTime);
            } else {
                _colorGrading.postExposure.value = Mathf.Lerp(
                    _colorGrading.postExposure.value, -3f, Time.deltaTime);
            }
        }

        bool HasLightSourceNearby() {
            // Проверка наличия источников света в радиусе
            Collider[] lights = Physics.OverlapSphere(
                transform.position, 10f, LayerMask.GetMask("Lights"));
            return lights.Length > 0;
        }
    }
}
