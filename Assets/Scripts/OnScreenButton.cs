﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.EventSystems;

[AddComponentMenu("Input/OnScreenButton")]
public class OnScreenButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        SendValueToControl(0.0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SendValueToControl(1.0f);
    }

    [InputControl(layout = "Button")]
    [SerializeField]
    private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}