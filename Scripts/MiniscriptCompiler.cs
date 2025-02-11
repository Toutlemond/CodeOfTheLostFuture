using System;
using System.Collections.Generic;
using System.IO;
using Al;
using HeneGames.DialogueSystem;
using UnityEngine;
using Miniscript;
using UI;
using UI.BotMenuItems;

/**
 * Script for GameObject
 */
public class MiniscriptCompiler : MonoBehaviour, IInteractable
{
    private Interpreter _interpreter;
    private static bool _intrinsicsAdded;
    private bool _someCoroutineIsRuning = false;
    public string codeText;
    public bool codeIsRunning = false;
    public Animator _animator;
    public GameObject _thisGameObject;
    private BotEnergy _botEnergy;
    private DialogueCharacter _dialogueCharacter;


    void Start()
    {
        SetInterpreter();
        _animator = GetComponent<Animator>();
        _botEnergy = GetComponent<BotEnergy>();
        _thisGameObject = gameObject;
        _dialogueCharacter = Resources.Load<DialogueCharacter>("CodeBot");
    }


    public float GetEnergy()
    {
        return _botEnergy.GetEnergyLevel();
    }

    public void UseEnergy(float energy)
    {
        _botEnergy.UseEnergy(energy);
    }

    void SetInterpreter()
    {
        _interpreter = new Interpreter();
        _interpreter.standardOutput = StandardOutput;
        _interpreter.implicitOutput = ImplicitOutput;
        _interpreter.errorOutput = ErrorOutput;
        _interpreter.hostData = this;
        if (!_intrinsicsAdded)
        {
            StandartIntrinsics.AddStandartIntrinsics(_interpreter);
            ResetAnimator();
            _intrinsicsAdded = true;
        }
    }

    [ContextMenu("Run Miniscript")]
    public void Run()
    {
        if (!_botEnergy || _botEnergy.GetEnergyLevel() == 0)
        {
            DialogueUI.instance.ShowSentence(_dialogueCharacter, "No Energy");
            return;
        }

        if (codeIsRunning)
        {
            Debug.LogWarning("Miniscript is already running.");
            _interpreter.Stop();
            codeIsRunning = false;
            return;
        }

        int id = _thisGameObject.GetInstanceID();
        string codeFile = id + "_code.ms";
        Debug.Log("codeFile:" + codeFile + '\n');
        try
        {
            codeText = File.ReadAllText(Path.Combine(Application.persistentDataPath, codeFile));
        }
        catch (Exception e)
        {
            codeText = File.ReadAllText(Path.Combine(Application.persistentDataPath, "default.txt"));
        }

        string constants = "_events=[];";
        _interpreter.Reset(constants + codeText);
        _interpreter.Compile();
        codeIsRunning = !codeIsRunning;
    }

    public void StopAllMiniscript(string message = "No Energy! Stopping...")
    {
        DialogueUI.instance.ShowSentence(_dialogueCharacter, message);
        _interpreter.Stop();
        codeIsRunning = false;
        return;
    }

    private void ResetAnimator()
    {
        if (_animator)
        {
            _animator.SetBool("walk", false);
            _animator.SetBool("run", false);
        }
    }

    private void StandardOutput(string s, bool set)
    {
        Debug.Log("MS:" + s);
    }

    private void ImplicitOutput(string s, bool set)
    {
        Debug.Log("MS:" + "<color=#66bb66>" + s + "</color>");
    }

    private void ErrorOutput(string s, bool set)
    {
        Debug.Log("MS:" + "<color=#fcba03>" + s + "</color>");
    }

    void Update()
    {
        if (codeIsRunning)
        {
            try
            {
                _interpreter.RunUntilDone(0.05f);
                if (!_interpreter.Running())
                {
                    codeIsRunning = false;
                }
            }
            catch (MiniscriptException e)
            {
                Debug.Log(e);
            }
        }
    }

    public bool ExecuteCommand(string command)
    {
        switch (command)
        {
            case "RunScript":
                Run();
                return true;
                break;
            case "StopScript":
                StopAllMiniscript("Stop Code From Menu");
                break;
        }
        return false;
    }
}