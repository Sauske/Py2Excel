using System;
using UnityEngine;

public class UIAnimationScript : UIComponent
{
    private Animation m_animation;

    private AnimationState m_currentAnimationState;

    private float m_currentAnimationTime;

    [HideInInspector]
    public enUIEventID[] eventIDs = new enUIEventID[Enum.GetValues(typeof(enAnimationEventType)).Length];

    public stUIEventParams[] eventParamsArray = new stUIEventParams[Enum.GetValues(typeof(enAnimationEventType)).Length];

    public override void Initialize(UIFormScript formScript)
    {
        if (m_isInitialized)
        {
            return;
        }
        base.Initialize(formScript);
        m_animation = gameObject.GetComponent<Animation>();
        if (m_animation != null && m_animation.playAutomatically && m_animation.clip != null)
        {
            m_currentAnimationState = m_animation[m_animation.clip.name];
            m_currentAnimationTime = 0f;
            DispatchAnimationEvent(enAnimationEventType.AnimationStart);
        }
    }

    protected override void OnDestroy()
    {
        m_animation = null;
        m_currentAnimationState = null;
        base.OnDestroy();
    }

    private void Update()
    {
        if (belongedFormScript != null && belongedFormScript.IsClosed())
        {
            return;
        }
        if (m_currentAnimationState == null)
        {
            return;
        }
        if (m_currentAnimationState.wrapMode != WrapMode.Loop && m_currentAnimationState.wrapMode != WrapMode.PingPong && m_currentAnimationState.wrapMode != WrapMode.ClampForever)
        {
            if (m_currentAnimationTime >= m_currentAnimationState.length)
            {
                DispatchAnimationEvent(enAnimationEventType.AnimationEnd);
                m_currentAnimationState = null;
                m_currentAnimationTime = 0f;
            }
            else
            {
                m_currentAnimationTime += Time.deltaTime;
            }
        }
    }

    public void PlayAnimation(string animName, bool forceRewind)
    {
        if (m_currentAnimationState != null && m_currentAnimationState.name.Equals(animName) && !forceRewind)
        {
            return;
        }
        if (m_currentAnimationState != null)
        {
            m_animation.Stop(m_currentAnimationState.name);
            m_currentAnimationState = null;
            m_currentAnimationTime = 0f;
        }
        m_currentAnimationState = m_animation[animName];
        m_currentAnimationTime = 0f;
        if (m_currentAnimationState != null)
        {
            m_animation.Play(animName);
            DispatchAnimationEvent(enAnimationEventType.AnimationStart);
        }
    }

    public void StopAnimation(string animName)
    {
        if (m_currentAnimationState == null || !m_currentAnimationState.name.Equals(animName))
        {
            return;
        }
        m_animation.Stop(animName);
        DispatchAnimationEvent(enAnimationEventType.AnimationEnd);
        m_currentAnimationState = null;
        m_currentAnimationTime = 0f;
    }

    public string GetCurrentAnimation()
    {
        return (!(m_currentAnimationState == null)) ? m_currentAnimationState.name : null;
    }

    public bool IsAnimationStopped(string animationName)
    {
        return string.IsNullOrEmpty(animationName) || m_currentAnimationState == null || m_currentAnimationTime == 0f || !string.Equals(m_currentAnimationState.name, animationName);
    }

    public void DispatchAnimationEvent(enAnimationEventType animationEventType)
    {
        if (eventIDs[(int)animationEventType] == enUIEventID.None)
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
        uiEvent.eventID = eventIDs[(int)animationEventType];
        uiEvent.eventParams = eventParamsArray[(int)animationEventType];
        base.DispatchUIEvent(uiEvent);
    }

    public void SetAnimationEvent(enAnimationEventType animationEventType, enUIEventID eventId, stUIEventParams eventParams = default(stUIEventParams))
    {
        eventIDs[(int)animationEventType] = eventId;
        eventParamsArray[(int)animationEventType] = eventParams;
    }

    public void SetAnimationSpeed(string animName, float speed)
    {
        if (m_animation != null && m_animation[animName] != null)
        {
            m_animation[animName].speed = speed;
        }
    }
}
