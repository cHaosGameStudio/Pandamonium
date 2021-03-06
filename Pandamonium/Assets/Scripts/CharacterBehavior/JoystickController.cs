﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{

    public Image bgImg, jsImg;
    public Vector2 InputDirection { set; get; }

    public Vector2 lastInputDirection { set; get; }

    private void Start()
    {
        bgImg = GetComponent<Image>();
        jsImg = GetComponentsInChildren<Image>()[1];
        InputDirection = Vector3.zero;
    }

    //EventSystems interfaces
    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 pos = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle
            (bgImg.rectTransform,
                ped.position,
                ped.pressEventCamera,
                out pos))
        {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            float x = (bgImg.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
            float y = (bgImg.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

            InputDirection = new Vector2(x, y);
            InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;
            jsImg.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (bgImg.rectTransform.sizeDelta.x / 3)
                , InputDirection.y * (bgImg.rectTransform.sizeDelta.y / 3));
        }

    }
    public virtual void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }
    public virtual void OnPointerUp(PointerEventData ped)
    {
        lastInputDirection = InputDirection;
        InputDirection = Vector3.zero;
        jsImg.rectTransform.anchoredPosition = Vector3.zero;
    }
}
