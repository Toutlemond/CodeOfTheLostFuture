using UnityEngine;

namespace Environment
{
    public class CampfireLight : MonoBehaviour {
        public Light fireLight;
        public float flickerSpeed = 0.1f;
        public float minIntensity = 3f;
        public float maxIntensity = 5f;

        void Update() {
            fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, 
                Mathf.PingPong(Time.time * flickerSpeed, 1));
        }
    }
}
