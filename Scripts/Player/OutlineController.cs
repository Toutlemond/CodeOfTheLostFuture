
using UnityEngine;

namespace Player
{
    public class OutlineController : MonoBehaviour
    {
        [Header("Настройки")]
        public float checkInterval = 0.5f; // Проверка не каждый кадр, а каждые 0.1 сек
        public float maxDistance = 12f;
        public LayerMask interactableLayer; // Слой для роботов
        public Camera mainCamera;
        private Outline _currentOutline;
        public GameObject menuPanel; // Панель с меню
        private bool _isPanelShowed = false;
        void Start() {
            if (!mainCamera)
            {
                Debug.LogError("MainCamera is null");
                return;
            }

            if (menuPanel)
            {
                menuPanel.SetActive(false);
            }

            StartCoroutine(CheckForTarget());
        }

        System.Collections.IEnumerator CheckForTarget() {
            while (true) {
                Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Луч из центра экрана
                RaycastHit hit;
           
                if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer)) {
                    Outline outline = hit.collider.GetComponent<Outline>();
                    if (outline != null) {
                        // Если навели на новый объект
                        if (_currentOutline != outline) {
                            ResetCurrentOutline();
                            _currentOutline = outline;
                            _currentOutline.enabled = true;
                        }
                        _isPanelShowed = true;
                    } else {
                        ResetCurrentOutline();
                    }
                 
                    
                } else {
                    ResetCurrentOutline();
                }

                MenuOperate();
                yield return new WaitForSeconds(checkInterval); // Оптимизация!
            }
        }

        private void MenuOperate()
        {
            if (_isPanelShowed)
            {
                menuPanel.SetActive(true);
            }
            else
            {
                menuPanel.SetActive(false);
            }
        }

        void ResetCurrentOutline() {
            _isPanelShowed = false;
            if (_currentOutline != null) {
                _currentOutline.enabled = false;
                _currentOutline = null;
            }
        }

        void OnDestroy() {
            StopAllCoroutines();
        }
    }
}
