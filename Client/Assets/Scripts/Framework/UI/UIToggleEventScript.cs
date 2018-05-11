using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class CUIToggleEventScript : UIComponent
{
    [HideInInspector]
    public enUIEventID m_onValueChangedEventID;

    private Toggle m_toggle;

    public override void Initialize(UIFormScript formScript)
    {
        if (m_isInitialized)
        {
            return;
        }
        m_toggle = gameObject.GetComponent<Toggle>();
        m_toggle.onValueChanged.RemoveAllListeners();
        m_toggle.onValueChanged.AddListener(new UnityAction<bool>(OnToggleValueChanged));
        Transform transform = gameObject.transform.Find("Label");
        if (transform != null)
        {
            if (m_toggle.isOn)
            {
                transform.GetComponent<Text>().color = UIUtility.s_Color_White;
            }
            else
            {
                transform.GetComponent<Text>().color = UIUtility.s_Text_Color_ListElement_Normal;
            }
        }
        base.Initialize(formScript);
    }

    protected override void OnDestroy()
    {
        m_toggle = null;
        base.OnDestroy();
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (m_onValueChangedEventID == enUIEventID.None)
        {
            return;
        }
        UIEvent uIEvent = Singleton<UIEventManager>.GetInstance().GetUIEvent();
        uIEvent.srcFormScript = belongedFormScript;
        uIEvent.srcWidget = base.gameObject;
        uIEvent.srcWidgetScript = this;
        uIEvent.srcWidgetBelongedListScript = belongedListScript;
        uIEvent.SrcWidgetIndexInBelongedList = indexInList;
        uIEvent.pointerEventData = null;
        uIEvent.eventID = m_onValueChangedEventID;
        uIEvent.eventParams.togleIsOn = isOn;
        Transform transform = gameObject.transform.Find("Label");
        if (transform != null)
        {
            if (isOn)
            {
                transform.GetComponent<Text>().color = UIUtility.s_Color_White;
            }
            else
            {
                transform.GetComponent<Text>().color = UIUtility.s_Text_Color_ListElement_Normal;
            }
        }
        DispatchUIEvent(uIEvent);
    }
}
