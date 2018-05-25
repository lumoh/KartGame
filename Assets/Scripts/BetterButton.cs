using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BetterButton : Button 
{
    public UnityEvent onButtonUp = new UnityEvent();
    public UnityEvent onButtonDown = new UnityEvent();

    protected override void Start()
    {
        base.Start();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        onButtonDown.Invoke();
        Debug.Log(this.gameObject.name + " Was Clicked.");
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        onButtonUp.Invoke();
        Debug.Log(this.gameObject.name + " Was Released.");
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        onButtonUp.Invoke();
        Debug.Log(this.gameObject.name + " Was Released.");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        onButtonDown.RemoveAllListeners();
        onButtonUp.RemoveAllListeners();
    }
}
