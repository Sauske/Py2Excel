using System;
using UnityEngine;
using UnityEngine.UI;

public class UITimerScript : UIComponent
{
    public enTimerType timerType;
    public enTimerDisplayType timerDisplayType;
    public bool pausedWhenAppPaused = true;
    public bool closeBelongedFormWhenTimeup;
    [HideInInspector]
    public enUIEventID[] eventIDs = new enUIEventID[Enum.GetValues(typeof(enTimerEventType)).Length];
    public stUIEventParams[] eventParamsArry = new stUIEventParams[Enum.GetValues(typeof(enTimerEventType)).Length];

    private double m_currentTime;
    private double m_totalTime;
    private double m_lastOnChangedTime;
    private double m_onChangedIntervalTime = 1.0;
    private double m_pauseTime;
    private double m_pauseElastTime;
    private double m_startTime;
    private bool m_isRunning;
    private bool m_isPaused;
    private bool m_runImmediately;

    private Text m_timerText;
    
    public override void  Initialize(UIFormScript formScript)
    {
        if (m_isInitialized)
        {
            return;
        }
        base.Initialize(formScript);
        if (m_runImmediately)
        {
            StartTimer();
        }
    
        m_timerText = base.GetComponentInChildren<Text>(gameObject);
        if (timerDisplayType == enTimerDisplayType.None && m_timerText != null)
        {
            m_timerText.gameObject.SetActive(false);
        }
        RefreshTimeDisplay();
    }

    protected override void OnDestroy()
    {
        m_timerText = null;
        base.OnDestroy();
    }

    public override void Close()
    {
        base.Close();
        ResetTime();
    }

    private void Update()
    {
        if (belongedFormScript != null && belongedFormScript.IsClosed())
        {
            return;
        }
        UpdateTimer();
    }

    public void SetTotalTime(float time)
    {
        m_totalTime = (double)time;
        RefreshTimeDisplay();
    }

    public void SetTimerEventId(enTimerEventType eventType, enUIEventID eventId)
    {
        if (eventType >= enTimerEventType.TimeStart && eventType < (enTimerEventType)eventIDs.Length)
        {
            eventIDs[(int)eventType] = eventId;
        }
    }

    public void SetCurrentTime(float time)
    {
        m_currentTime = (double)time;
    }

    public float GetCurrentTime()
    {
        return (float)this.m_currentTime;
    }

    public void SetOnChangedIntervalTime(float intervalTime)
    {
        this.m_onChangedIntervalTime = (double)intervalTime;
    }

    public void StartTimer()
    {
        if (m_isRunning)
        {
            return;
        }
        ResetTime();
        m_isRunning = true;
        DispatchTimerEvent(enTimerEventType.TimeStart);
    }

    public void ReStartTimer()
    {
        EndTimer();
        StartTimer();
    }

    public void OnApplicationPause(bool pause)
    {
        if (!pausedWhenAppPaused)
        {
            return;
        }

        if (pause)
        {
            PauseTimer();
        }
        else
        {
            ResumeTimer();    
        }
    }

    public void PauseTimer()
    {
        if (m_isPaused)
        {
            return;
        }
        m_pauseTime = (double)Time.realtimeSinceStartup;
        m_isPaused = true;
    }

    public void ResumeTimer()
    {
        if (!m_isPaused)
        {
            return;
        }
        m_pauseElastTime += (double)Time.realtimeSinceStartup - this.m_pauseTime;
        m_isPaused = false;
    }

    public void EndTimer()
    {
        ResetTime();
        m_isRunning = false;
    }

    public void ResetTime()
    {
        m_startTime = Time.realtimeSinceStartup;
        m_pauseTime = 0.0;
        m_pauseElastTime = 0.0;
        if (timerType == enTimerType.CountUp)
        {
            m_currentTime = 0.0;
        }
        else if (timerType == enTimerType.CountDown)
        {
            m_currentTime = m_totalTime;
        }
        m_lastOnChangedTime = m_currentTime;
    }

    private void UpdateTimer()
    {
        if (!m_isRunning || m_isPaused)
        {
            return;
        }

        bool flag = false;
        double currentTime = m_currentTime;
        enTimerType etimerType = timerType;
        if (etimerType != enTimerType.CountUp)
        {
            if (etimerType == enTimerType.CountDown)
            {
                m_currentTime = m_totalTime - ((double)Time.realtimeSinceStartup - m_startTime - m_pauseElastTime);
                flag = (this.m_currentTime <= 0.0);
            }
        }
        else
        {
            m_currentTime = (double)Time.realtimeSinceStartup - m_startTime - m_pauseElastTime;
            flag = (m_currentTime >= m_totalTime);
        }

        if ((int)currentTime != (int)m_currentTime)
        {
            RefreshTimeDisplay();
        }
        if ((double)Mathf.Abs((float)(m_currentTime - m_lastOnChangedTime)) >= m_onChangedIntervalTime)
        {
            m_lastOnChangedTime = m_currentTime;
            DispatchTimerEvent(enTimerEventType.TimeChanged);
        }
        if (flag)
        {
            EndTimer();
            DispatchTimerEvent(enTimerEventType.TimeUp);
            if (closeBelongedFormWhenTimeup)
            {
                belongedFormScript.Close();
            }
        }
    }

    private void RefreshTimeDisplay()
    {
        if (m_timerText == null)
        {
            return;
        }
        if (timerDisplayType != enTimerDisplayType.None)
        {
            int num = (int)this.m_currentTime;
            switch (timerDisplayType)
            {
                case enTimerDisplayType.H_M_S:
                {
                    int num2 = num / 3600;
                    num -= num2 * 3600;
                    int num3 = num / 60;
                    int num4 = num - num3 * 60;
                    m_timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", num2, num3, num4);
                    break;
                }
                case enTimerDisplayType.M_S:
                {
                    int num5 = num / 60;
                    int num6 = num - num5 * 60;
                    m_timerText.text = string.Format("{0:D2}:{1:D2}", num5, num6);
                    break;
                }
                case enTimerDisplayType.S:
                {
                    m_timerText.text = string.Format("{0:D}", num);
                    break;
                }
                case enTimerDisplayType.H_M:
                {
                    int num7 = num / 3600;
                    num -= num7 * 3600;
                    int num8 = num / 60;
                    m_timerText.text = string.Format("{0:D2}:{1:D2}", num7, num8);
                    break;
                }
                case enTimerDisplayType.D_H_M_S:
                {
                    int num9 = num / 86400;
                    num -= num9 * 86400;
                    int num10 = num / 3600;
                    num -= num10 * 3600;
                    int num11 = num / 60;
                    int num12 = num - num11 * 60;
                    m_timerText.text = string.Format("{0}天{1:D2}:{2:D2}:{3:D2}", new object[]
					{
						num9,
						num10,
						num11,
						num12
					});
                    break;
                }
                case enTimerDisplayType.D_H_M:
                {
                    int num13 = num / 86400;
                    num -= num13 * 86400;
                    int num14 = num / 3600;
                    num -= num14 * 3600;
                    int num15 = num / 60;
                    m_timerText.text = string.Format("{0}天{1:D2}:{2:D2}", num13, num14, num15);
                    break;
                }
                case enTimerDisplayType.D:
                {
                    int num16 = num / 86400;
                    m_timerText.text = string.Format("{0}天", num16);
                    break;
                }
            }
        }
    }

    private void DispatchTimerEvent(enTimerEventType eventType)
    {
        if (eventIDs[(int)eventType] != enUIEventID.None)
        {
            UIEvent uiEvent = Singleton<UIEventManager>.GetInstance().GetUIEvent();
            uiEvent.srcFormScript = belongedFormScript;
            uiEvent.srcWidget = gameObject;
            uiEvent.srcWidgetScript = this;
            uiEvent.srcWidgetBelongedListScript = belongedListScript;
            uiEvent.SrcWidgetIndexInBelongedList = indexInList;
            uiEvent.pointerEventData = null;
            uiEvent.eventID = eventIDs[(int)eventType];
            uiEvent.eventParams = eventParamsArry[(int)eventType];
            base.DispatchUIEvent(uiEvent);
        }    
    }
}
