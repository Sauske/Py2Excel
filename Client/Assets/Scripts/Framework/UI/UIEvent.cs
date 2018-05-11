using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEvent 
{
    public UIFormScript srcFormScript;

    public GameObject srcWidget;

    public UIComponent srcWidgetScript;

    public UIListScript srcWidgetBelongedListScript;

    public PointerEventData pointerEventData;

    public stUIEventParams eventParams;

    public enUIEventID eventID;

    public bool inUse;

    public int SrcWidgetIndexInBelongedList
	{
		get;
		set;
	}

	public void Clear()
	{
		srcFormScript = null;
		srcWidget = null;
		srcWidgetScript = null;
		srcWidgetBelongedListScript = null;
		SrcWidgetIndexInBelongedList = -1;
		pointerEventData = null;
		eventID = enUIEventID.None;
		inUse = false;
	}
}
