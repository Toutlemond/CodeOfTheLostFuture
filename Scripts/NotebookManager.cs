using System;
using System.IO;
using InGameCodeEditor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public GameObject msCanvas; // Ссылка на ваш Canvas
    public GameObject player; // Ссылка на ваш Player
    public GameObject joint;
    public Camera playerCamera;
    public GameObject noteBookCamera;
    public GameObject msCamera;
    public GameObject notebookPrefab; // Префаб блокнота
    private GameObject _currentNotebook; // Текущий экземпляр блокнота
    private bool _isHolding = false;
    private Camera _camera;
    private string _curentFile = "default.txt";

    public GameObject codeEditorGameObject;
    private CodeEditor _codeEditor;
    public Button saveButton; // Ссылка на кнопку сохранения
    public Button openButton;
    public Button newButton;

    void Start()
    {
        _camera = Camera.main;
        msCanvas.SetActive(false);
        noteBookCamera.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        player.SetActive(true);
        saveButton.onClick.AddListener(SaveText);
        _codeEditor = codeEditorGameObject.GetComponent<CodeEditor>();
        LoadText();
    }


    void SaveText()
    {
        string textToSave = _codeEditor.Text;
        string path = Path.Combine(Application.persistentDataPath, _curentFile);
        File.WriteAllText(path, textToSave);
        Debug.Log("Text saved to: " + path);
    }

    void LoadText()
    {
        Debug.Log("Loading text from: " + _curentFile);
        string textToLoad = "";
        try
        {
            textToLoad = File.ReadAllText(Path.Combine(Application.persistentDataPath, _curentFile));
        }
        catch (Exception e)
        {
            string path = Path.Combine(Application.persistentDataPath, _curentFile);
            File.WriteAllText(path, textToLoad);
            Debug.Log("Text saved to: " + path);
        }
        _codeEditor.Text = textToLoad;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isHolding)
        {
            // Создаем луч из камеры в центр экрана
            if (_camera)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Ray ray = _camera.ScreenPointToRay(screenCenter);

                // Рисуем луч в редакторе (только для отладки)
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 56);

                // Если нужно выполнить проверку на столкновение
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.CompareTag("Notebook")) // Убедитесь, что у вашего префаба есть тег "Notebook"
                    {
                        PickUpNotebook(hit.transform.parent.gameObject);
                    }

                    // Обработка столкновения
                    Debug.Log("Hit: " + hit.collider.name);
                }
            }
            else
            {
                Debug.Log("no camera:");
            }
        }

        if (_isHolding && Input.GetKeyDown(KeyCode.B))
        {
            DropNotebook();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            OpenCodeEditor();
        }
    }

    public void OpenCodeEditor()
    {
        // Переключаем активность Canvas
        msCanvas.SetActive(!msCanvas.activeSelf);
        msCamera.SetActive(!msCamera.activeSelf);
        //player.SetActive(!player.activeSelf);
        //playerCamera.gameObject.SetActive(!playerCamera.gameObject.activeSelf);

        // Если вы хотите также управлять временем игры (пауза), можете добавить это здесь
        if (msCanvas.activeSelf)
        {
            setCurrentFile("default.txt");
            if (_camera)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Ray ray = _camera.ScreenPointToRay(screenCenter);

                // Рисуем луч в редакторе (только для отладки)
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue, 56);

                // Если нужно выполнить проверку на столкновение
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject hitObject = hit.transform.gameObject;
                    MiniscriptCompiler target = hitObject.GetComponent<MiniscriptCompiler>();
                    if (target)
                    {
                        int id = hitObject.GetInstanceID();
                        string codeFile = id + "_code.ms";
                        setCurrentFile(codeFile);
                    }
                }
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0; // Останавливаем игру
            LoadText();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1; // Возвращаем игру к нормальному состоянию
        }
    }

    private void setCurrentFile(string filename)
    {
        _curentFile = filename;
    }

    void OpenNotebook()
    {
        _currentNotebook = Instantiate(notebookPrefab, transform, false);
        _currentNotebook.GetComponent<RectTransform>().localPosition = Vector3.zero; // Центрируем блокнот
        // Дополнительно можно добавить анимацию приближения
    }

    void CloseNotebook()
    {
        Destroy(_currentNotebook);
    }

    void PickUpNotebook(GameObject notebook)
    {
        _currentNotebook = notebook;
        _currentNotebook.transform.SetParent(player.transform); // Привязываем к персонажу
        _currentNotebook.transform.localPosition = new Vector3(0, 0, 1); // Настройте позицию по вашему усмотрению
        //_currentNotebook.transform.rotation = new Quaternion(0,180,0 ,1f); // Настройте позицию по вашему усмотрению
        _isHolding = true;

        // Здесь можно добавить анимацию приближения к экрану
        // Например, с помощью корутины или анимации
    }

    void DropNotebook()
    {
        _currentNotebook.transform.SetParent(null); // Отвязываем от персонажа
        _isHolding = false;
    }
}