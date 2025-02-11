using System.Collections;
using System.Collections.Generic;
using HeneGames.DialogueSystem;
using Miniscript;
using UnityEngine;

public class StandartIntrinsics

{
    
    /**
     * Тут должны быть собраны команды общие для всех типов роботов. Такие как Move и Turn
     */
    private static Interpreter _interpreter;
    public static void AddStandartIntrinsics(Interpreter interpreter)
    {
        _interpreter = interpreter;
        AddMoveCommand();
        AddTurnCommand();
        AddSayCommand();

    }

    private static void AddMoveCommand()
    {
        float energyPerStep = 0.001f; // Это очень много Не забудь сделать баланс
        Debug.Log("Add AddMoveCommand");
        Intrinsic f;
        f = Intrinsic.Create("move");
        f.AddParam("len");
        f.code = (context, partialResult) =>
        {
            MiniscriptCompiler mc = context.interpreter.hostData as MiniscriptCompiler;
            if (partialResult == null)
            {
                float len = context.GetLocalFloat("len");
                Vector3 direction = mc._thisGameObject.transform.forward * len;
                Vector3 targetPosition = mc._thisGameObject.transform.position + direction;
                List<Value> l = new List<Value>();
                Value valuex = new ValNumber(targetPosition.x);
                l.Add(valuex);
                Value valuey = new ValNumber(targetPosition.y);
                l.Add(valuey);
                Value valuez = new ValNumber(targetPosition.z);
                l.Add(valuez);
                return new Intrinsic.Result(new ValList(l), false);
            }
            else
            {
                //Its Repeat many times
                Debug.Log("EL: " +mc.GetEnergy());
                if (mc.GetEnergy() <= 0)
                {
                    mc.StopAllMiniscript();
                    return Intrinsic.Result.Null;
                }
                mc.UseEnergy(energyPerStep);

                ValList list = (ValList)partialResult.result;
                Value x = list.GetElem(new ValNumber(0));
                Value y = list.GetElem(new ValNumber(1));
                Value z = list.GetElem(new ValNumber(2));
                Vector3 targetPosition = new Vector3(x.FloatValue(), y.FloatValue() + 0.1f, z.FloatValue());
                RaycastHit hit;
                if (Physics.Raycast(mc._thisGameObject.transform.position, Vector3.down, out hit))
                {
                    targetPosition.y = hit.point.y + 0.1f;
                }

                if (mc._animator)
                {
                    mc._animator.SetBool("Walk", true);
                }

                float distance = Vector3.Distance(mc._thisGameObject.transform.position, targetPosition);
                mc._thisGameObject.transform.position = Vector3.MoveTowards(mc._thisGameObject.transform.position,
                    targetPosition, 1.1f * Time.deltaTime);
                if (distance < 0.5f)
                {
                    if (mc._animator)
                    {
                        mc._animator.SetBool("Walk", false);
                    }
                    return Intrinsic.Result.Null;
                }
                return partialResult;
            }
        };
    }

    private static void AddTurnCommand()
    {
        Intrinsic f;
        Debug.Log("Add AddTurnCommand");
        f = Intrinsic.Create("turn");
        f.AddParam("angle");
        f.code = (context, partialResult) =>
        {
            MiniscriptCompiler mc = context.interpreter.hostData as MiniscriptCompiler;
            if (partialResult == null)
            {
                int angle = context.GetLocalInt("angle");
                float targetAngle = Mathf.Repeat(mc._thisGameObject.transform.eulerAngles.y + angle, 360f);
                Debug.Log("targetAngle:" + targetAngle);
                return new Intrinsic.Result(new ValNumber(targetAngle), false);
            }
            float currentAngle = mc._thisGameObject.transform.eulerAngles.y; 
            float targetAngleSave = partialResult.result.FloatValue();
            float direction = Mathf.Sign(targetAngleSave - currentAngle);
            if (direction == 0f)
            {
                direction = 1f; 
            }
            
            currentAngle += direction * 1f;
            
            if (currentAngle < 0f)
            {
                currentAngle += 360f;
            }
            else if (currentAngle >= 360f)
            {
                currentAngle -= 360f;
            }
            
            if (Mathf.Abs(targetAngleSave - currentAngle) < 1f)
            {
                return Intrinsic.Result.Null;
            }

            // Применяем поворот к объекту
            mc._thisGameObject.transform.eulerAngles = new Vector3(
                mc._thisGameObject.transform.eulerAngles.x,
                currentAngle,
                mc._thisGameObject.transform.eulerAngles.z
            );
            return partialResult;
        };
    }

    private static void AddSayCommand()
    {
        Intrinsic f;
        f = Intrinsic.Create("say");
        f.AddParam("s", ValString.empty);
        f.AddParam("delimiter");
        f.code = (context, partialResult) => {
            Value sVal = context.GetLocal("s");
            string s = (sVal == null ? "null" : sVal.ToString());
            Value delimiter = context.GetLocal("delimiter");
            if (delimiter == null) context.vm.standardOutput(s, true);
            else context.vm.standardOutput(s + delimiter.ToString(), false);
            
            DialogueCharacter dialogueCharacter = Resources.Load<DialogueCharacter>("CodeBot");
            
            if (partialResult == null)
            {
                DialogueUI.instance.ShowSentence(dialogueCharacter, s);
                // Запоминаем время начала
                float startTime = Time.time;
                // Устанавливаем время окончания через 20 секунд
                float endTime = startTime + 5f;
                return new Intrinsic.Result(new ValNumber(endTime), false);
            }
            if (Input.GetKeyDown(DialogueUI.instance.actionInput))
            {
                DialogueUI.instance.ShowInteractionUI(false);
                DialogueUI.instance.ClearText();
                return Intrinsic.Result.Null;
            }
            
            float waitSec = partialResult.result.FloatValue();
            // Получаем текущее время
            float currentTime = Time.time;
            // Проверяем, не прошло ли 20 секунд
            if (currentTime < waitSec)
            {
                return partialResult;
            }
            
            DialogueUI.instance.ShowInteractionUI(false);
            DialogueUI.instance.ClearText();
            return Intrinsic.Result.Null;
        };
    }
}