
using System;
using UnityEngine;

/**
 * Скрипт Вешается на обьект которым можно управлять через программирование В варианте с gameManager
 */
public class MiniscriptUnit : MonoBehaviour
{
    public string codeText; public bool isRunning;
    public bool reload;

    public void Start()
    {
        isRunning = false;
    }

    [ContextMenu("run")]
    public void Run()
    {
        reload = true;
        isRunning = true;
    }
}