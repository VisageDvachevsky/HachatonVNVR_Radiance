using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  // Подключаем новую систему ввода

public class RobotFreeAnim : MonoBehaviour
{

    Vector3 rot = Vector3.zero;
    float rotSpeed = 40f;
    Animator anim;

    // Use this for initialization
    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        gameObject.transform.eulerAngles = rot;
    }

    // Update is called once per frame
    void Update()
    {
        CheckKey();
        gameObject.transform.eulerAngles = rot;
    }

    void CheckKey()
    {
        // Получаем входные данные от клавиш с помощью новой системы ввода
        var keyboard = Keyboard.current;

        // Walk
        if (keyboard != null && keyboard.wKey.isPressed)
        {
            anim.SetBool("Walk_Anim", true);
        }
        else if (keyboard != null && keyboard.wKey.wasReleasedThisFrame)
        {
            anim.SetBool("Walk_Anim", false);
        }

        // Rotate Left
        if (keyboard != null && keyboard.aKey.isPressed)
        {
            rot[1] -= rotSpeed * Time.fixedDeltaTime;
        }

        // Rotate Right
        if (keyboard != null && keyboard.dKey.isPressed)
        {
            rot[1] += rotSpeed * Time.fixedDeltaTime;
        }

        // Roll
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            if (anim.GetBool("Roll_Anim"))
            {
                anim.SetBool("Roll_Anim", false);
            }
            else
            {
                anim.SetBool("Roll_Anim", true);
            }
        }

        // Close
        if (keyboard != null && keyboard.leftCtrlKey.wasPressedThisFrame)
        {
            if (!anim.GetBool("Open_Anim"))
            {
                anim.SetBool("Open_Anim", true);
            }
            else
            {
                anim.SetBool("Open_Anim", false);
            }
        }
    }
}
