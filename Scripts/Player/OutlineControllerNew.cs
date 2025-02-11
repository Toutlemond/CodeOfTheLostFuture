// OutlineControllerNew.cs

using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UI.BotMenuItems;
using UnityEngine.UI;

namespace Player
{
    public class OutlineControllerNew : MonoBehaviour
    {
        [Header("Настройки")] public float checkInterval = 0.5f;
        public float maxDistance = 12f;
        public LayerMask interactableLayer;
        public Camera mainCamera;
        public RectTransform menuPanel;
        public GameObject menuItemPrefab;

        [Header("Цвета")] public Color selectedColor = Color.yellow;
        public Color normalColor = Color.white;

        private Outline _currentOutline;
        private InteractableMenu _currentMenu;
        private List<GameObject> _menuItems = new List<GameObject>();
        private int _currentSelection;

        void Start()
        {
            menuPanel.gameObject.SetActive(false);
            StartCoroutine(InteractionCheck());
        }

        IEnumerator InteractionCheck()
        {
            while (true)
            {
                Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 15);
                if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
                {
                    Debug.Log("HandleNewInteractable");
                    HandleNewInteractable(hit.collider);
                }
                else
                {
                    if (_currentMenu)
                    {
                        Debug.Log("ClearInteraction");
                        ClearInteraction();
                    }
                }

                yield return new WaitForSeconds(checkInterval);
            }
        }

        void HandleNewInteractable(Collider col)
        {
            var menu = col.GetComponent<InteractableMenu>();
            var outline = col.GetComponent<Outline>();

            if (menu != null && outline != null)
            {
                if (_currentMenu != menu)
                {
                    ClearInteraction();
                    _currentMenu = menu;
                    _currentOutline = outline;
                    _currentOutline.enabled = true;
                    CreateMenuItems(menu.GetMenuItems());
                }
            }
        }

        void CreateMenuItems(List<BotMenuItem> items)
        {
            menuPanel.gameObject.SetActive(true);
            foreach (var item in items)
            {
                var menuItem = Instantiate(menuItemPrefab, menuPanel);
                Debug.Log("item:" + item.itemName);
                menuItem.GetComponent<Text>().text = item.itemName;
                _menuItems.Add(menuItem);
            }

            UpdateSelection(0);
        }
        
        void Update()
        {
            if (_currentMenu == null) return;

            // Обработка скролла
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                int direction = scroll > 0 ? -1 : 1;
                int newSelection = Mathf.Clamp(
                    _currentSelection + direction,
                    0,
                    _menuItems.Count - 1
                );

                UpdateSelection(newSelection);
            }

            // Обработка клика
            if (Input.GetMouseButtonDown(2))
            {
                GameObject hostGo = _currentMenu.gameObject;
                Debug.Log("hostGo:" + hostGo);
                var interactable = hostGo.GetComponentsInParent<IInteractable>();

                Debug.Log("interactable:" + interactable);
                if (interactable != null)
                {
                    foreach (var item in interactable)
                    {
                        string command = _currentMenu.menuItems[_currentSelection].command;
                        Debug.Log("command: " + command);
                        if (item.ExecuteCommand(command) == true)
                        {
                            break;
                        }

                        Debug.Log("No Such Command IN interactable");
                    }
                }
                else
                {
                    Debug.Log("No interactable");
                }
                //_currentMenu.menuItems[_currentSelection].Execute();
            }
        }

        void UpdateSelection(int newIndex)
        {
            if (_menuItems.Count == 0) return;

            // Сброс предыдущего выбора
            _menuItems[_currentSelection].GetComponentInChildren<Text>().color = normalColor;

            // Установка нового выбора
            _currentSelection = newIndex;
            _menuItems[_currentSelection].GetComponentInChildren<Text>().color = selectedColor;
        }

        void ClearInteraction()
        {
            if (_currentOutline != null)
            {
                _currentOutline.enabled = false;
                _currentOutline = null;
            }

            foreach (var item in _menuItems)
            {
                Destroy(item);
            }

            _menuItems.Clear();
            menuPanel.gameObject.SetActive(false);
            _currentMenu = null;
            _currentSelection = 0;
        }

        void OnDestroy()
        {
            ClearInteraction();
            StopAllCoroutines();
        }
    }
}