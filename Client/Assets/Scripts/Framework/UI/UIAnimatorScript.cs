﻿using System;
using UnityEngine;

public class UIAnimatorScript : UIComponent
{
    private Animator m_animator;

    private int m_currentAnimatorStateCounter;

    private bool m_isEnableBeHide = true;

    private bool m_isEnableAnimatorBeHide = true;

    [HideInInspector]
    public enUIEventID[] eventIDs = new enUIEventID[Enum.GetValues(typeof(enAnimatorEventType)).Length];

    public stUIEventParams[] eventParamsArray = new stUIEventParams[Enum.GetValues(typeof(enAnimatorEventType)).Length];

    public string m_currentAnimatorStateName
    {
        get;
        private set;
    }

    public void SetUIEvent(enAnimatorEventType eventType, enUIEventID eventID, stUIEventParams eventParams)
    {
        this.eventIDs[(int)eventType] = eventID;
        this.eventParamsArray[(int)eventType] = eventParams;
    }

    public override void Initialize(UIFormScript formScript)
    {
        if (m_isInitialized)
        {
            return;
        }
        base.Initialize(formScript);
        m_animator =gameObject.GetComponent<Animator>();
        m_isEnableBeHide = enabled;
        m_isEnableAnimatorBeHide = m_animator.enabled;
    }

    protected override void OnDestroy()
    {
        m_animator = null;
        base.OnDestroy();
    }

    private void Update()
    {
        if (belongedFormScript != null && belongedFormScript.IsClosed())
        {
            return;
        }
        if (m_currentAnimatorStateName == null)
        {
            return;
        }
        if ((int)m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > m_currentAnimatorStateCounter)
        {
            m_animator.StopPlayback();
            string currentAnimatorStateName = this.m_currentAnimatorStateName;
            m_currentAnimatorStateName = null;
            m_currentAnimatorStateCounter = 0;
            DispatchAnimatorEvent(enAnimatorEventType.AnimatorEnd, currentAnimatorStateName);
        }
    }

    public void PlayAnimator(string stateName)
    {
        if (m_animator == null)
        {
            m_animator = base.gameObject.GetComponent<Animator>();
        }
        if (!m_animator.enabled)
        {
            m_animator.enabled = true;
        }
        m_animator.Play(stateName, 0, 0f);
        m_currentAnimatorStateName = stateName;
        m_animator.Update(0f);
        m_animator.Update(0f);
        m_currentAnimatorStateCounter = (int)this.m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        DispatchAnimatorEvent(enAnimatorEventType.AnimatorStart, this.m_currentAnimatorStateName);
    }

    public void SetBool(string name, bool value)
    {
        m_animator.SetBool(name, value);
    }

    public void SetAnimatorEnable(bool isEnable)
    {
        if (m_animator)
        {
            m_animator.enabled = isEnable;
            enabled = isEnable;
        }
    }

    public void SetInteger(string name, int value)
    {
        this.m_animator.SetInteger(name, value);
    }

    public void StopAnimator()
    {
    }

    public bool IsAnimationStopped(string animationName)
    {
        return string.IsNullOrEmpty(animationName) || !string.Equals(this.m_currentAnimatorStateName, animationName);
    }

    private void DispatchAnimatorEvent(enAnimatorEventType animatorEventType, string stateName)
    {
        if (eventIDs[(int)animatorEventType] == enUIEventID.None)
        {
            return;
        }
        UIEvent uiEvent = Singleton<UIEventManager>.GetInstance().GetUIEvent();
        uiEvent.srcFormScript = belongedFormScript;
        uiEvent.srcWidget = gameObject;
        uiEvent.srcWidgetScript = this;
        uiEvent.srcWidgetBelongedListScript = belongedListScript;
        uiEvent.SrcWidgetIndexInBelongedList = indexInList;
        uiEvent.pointerEventData = null;
        uiEvent.eventID = eventIDs[(int)animatorEventType];
        uiEvent.eventParams = eventParamsArray[(int)animatorEventType];
        uiEvent.eventParams.tagStr = stateName;
        base.DispatchUIEvent(uiEvent);
    }
}
