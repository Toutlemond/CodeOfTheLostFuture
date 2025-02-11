using UnityEngine;

public class CodebleObjectsController : MonoBehaviour
{
    private Camera _camera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
           // Debug.Log("click wheel:");
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            // Создаем луч из камеры в центр экрана
            if (_camera)
            {
                Ray ray = _camera.ScreenPointToRay(screenCenter);
                // Рисуем луч в редакторе (только для отладки)
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 56);
                // Если нужно выполнить проверку на столкновение
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    GameObject hitObject = hit.transform.gameObject;
                    MiniscriptCompiler target = hitObject.GetComponent<MiniscriptCompiler>();
                    if (target)
                    {
                        target.Run();
                    }
                    else
                    {
                        Debug.Log("Target Have no MiniScript");
                    }
                }
            }
        }
    }
}