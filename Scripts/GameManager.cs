using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Miniscript;

public class GameManager : MonoBehaviour
{
    class Agent
    {
        public string name;
        public string code;
        public GameObject agent;
        public bool IsRunning = false;
        public bool reload = false;
        public MiniscriptUnit unit = null;
        Interpreter interpreter;

        public Agent(string name, GameObject theAgent, string sourceCode)
        {
            agent = theAgent;
            code = sourceCode;
            interpreter = new Interpreter();
            interpreter.standardOutput = StandardOutput;
            interpreter.implicitOutput = ImplicitOutput;
            interpreter.errorOutput = ErrorOutput;
            interpreter.hostData = agent;
            unit = agent.GetComponent<MiniscriptUnit>();
        }

        public void Run()
        {
            //if (interpreter.Running())
            if (unit.reload)
            {
                interpreter.Reset(code);
                unit.reload = false;
            }
            interpreter.Compile();
            interpreter.RunUntilDone(0.01);
            if (interpreter.done)
            {
                unit.isRunning = false;
            }
        }        
        
        public void Stop()
        {
            interpreter.Stop();
        }

        private void StandardOutput(string s, bool set)
        {
            Debug.Log("MS:" + s);
        }
    
        private void ImplicitOutput(string s, bool set)
        {
            Debug.Log("MS:" +"<color=#66bb66>" + s + "</color>");
        } 
    
        private void ErrorOutput(string s, bool set)
        {
            Debug.Log("MS:" +"<color=#fcba03>" + s + "</color>");
        }
    }

    public GameObject testObject;

    public TextAsset testSource;
    public Interpreter Interpreter;
    private static bool _intrinsicsAdded;
    private bool _moveCoroutineRunning; // Это плохо и тупо не делай так

    private void Awake()
    {
        if (_intrinsicsAdded) return; // already done!
        _intrinsicsAdded = true;
        Intrinsic f;
        f = Intrinsic.Create("move");
        f.AddParam("len");
        f.code = (context, partialResult) =>
        {
            Value v;
            context.variables.TryGetValue("len", out v);
            GameObject agent = (GameObject)context.interpreter.hostData;
            MoveNew(agent,v.IntValue());
            if (_moveCoroutineRunning)
            {
                return Intrinsic.Result.Waiting;
            }
            return Intrinsic.Result.True;
        };       
        f = Intrinsic.Create("turn");
        f.AddParam("max");
        f.code = (context, partialResult) =>
        {
            Value v;
            context.variables.TryGetValue("max", out v);
            GameObject agent = (GameObject)context.interpreter.hostData;
            TurnDir(agent,v.IntValue());
            return Intrinsic.Result.True;
        };
    }

    private void MoveDir(GameObject agent, float len)
    {
        // Получаем направление вперед с учетом рельефа
        Vector3 direction = agent.transform.forward * len;
        // Определяем целевую позицию
        Vector3 targetPosition = agent.transform.position + direction;
        // Перемещаем объект к целевой позиции
        float dist = Vector3.Distance(agent.transform.position, targetPosition);
        agent.transform.position = targetPosition;
        Debug.Log("Move DONE!");
        // Устанавливаем объект точно на целевую позицию, чтобы избежать дребезга
    }

    private void TurnDir(GameObject agent, int ang)
    {
        agent.transform.Rotate(new Vector3(0, ang, 0), Space.World);
        Debug.Log("turn DONE!");
    }

    private void TurnNew(GameObject agent, int max)
    {
        StartCoroutine(TurnNewCoroutine(agent, max));
    }   
    
    private void MoveNew(GameObject agent, int len)
    {
        StartCoroutine(MoveCoroutine(agent, len));
    }
    
    private IEnumerator MoveCoroutine(GameObject agent, float len)
    {
        // Получаем направление вперед с учетом рельефа
        Vector3 direction = agent.transform.forward * len;
        Debug.Log("Set Dir");
        _moveCoroutineRunning = true;
        // Определяем целевую позицию
        Vector3 targetPosition = agent.transform.position + direction;
        // Перемещаем объект к целевой позиции
        float dist = Vector3.Distance(agent.transform.position, targetPosition);
        Debug.Log("dist:" + dist);
        while (dist >= 1f)
        {
            // Проверяем высоту рельефа с помощью Raycast
            RaycastHit hit;
            if (Physics.Raycast(agent.transform.position, Vector3.down, out hit))
            {
                targetPosition.y = hit.point.y+0.1f; // Устанавливаем Y-координату на уровне рельефа
            }
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, targetPosition, 1 * Time.deltaTime);
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Move DONE!");
        _moveCoroutineRunning = false;
        transform.position = targetPosition;
    }    
    
    private IEnumerator TurnNewCoroutine(GameObject agent, int max)
    {
        for (int i = 0; i < max; i++)
        {
            agent.transform.Rotate(new Vector3(0, 1, 0), Space.World);
            Debug.Log("i:"+i);
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("turn DONE!");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Agent _test;
    void Start()
    {
        try
        {
            string codeText = File.ReadAllText(Path.Combine(Application.persistentDataPath, "default.txt"));
            _test = new Agent("test", testObject, codeText);
        }
        catch (MiniscriptException err)
        {
            Console.WriteLine(err);
            throw;
        }
     
    }

    // Update is called once per frame
    void Update()
    {
        if (_test.unit.isRunning)
        {
            Debug.Log("isRunning!");
            _test.Run();
        }
        else
        {
            _test.Stop();
        }
    }
}
