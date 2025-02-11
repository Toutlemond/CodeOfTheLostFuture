using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MSEditorWindow : MonoBehaviour
{
    public TMP_InputField inputField; // Ссылка на InputField
    public Button saveButton; // Ссылка на кнопку сохранения
    public Button openButton; 
    public Button newButton; 

    void Start()
    {
        saveButton.onClick.AddListener(SaveText);
        LoadText();
    }

    void SaveText()
    {
        string textToSave = inputField.text;
        string path = Path.Combine(Application.persistentDataPath, "default.txt");
        File.WriteAllText(path, textToSave);
        Debug.Log("Text saved to: " + path);
    }

    void LoadText()
    {
        string textToLoad = File.ReadAllText(Path.Combine(Application.persistentDataPath, "default.txt"));
        inputField.text = textToLoad;
    }
}