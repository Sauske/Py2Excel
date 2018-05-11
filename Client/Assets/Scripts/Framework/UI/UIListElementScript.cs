using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIListElementScript : UIComponent
{
    public delegate void OnSelectedDelegate();

    public GameObject m_selectFrontObj;

    public Sprite m_selectedSprite;

    [HideInInspector]
    public Sprite m_defaultSprite;

    [HideInInspector]
    public Color m_defaultColor;

    [HideInInspector]
    public Color m_defaultTextColor;

    public Text m_textObj;

    public Color m_selectTextColor = new Color(1f, 1f, 1f, 1f);

    [HideInInspector]
    public ImageAlphaTexLayout m_defaultLayout;

    public ImageAlphaTexLayout m_selectedLayout;

    [HideInInspector]
    public Vector2 m_defaultSize;

    [HideInInspector]
    public int m_index;

    [HideInInspector]
    public enPivotType m_pivotType = enPivotType.LeftTop;

    private Image m_image;

    public stRect m_rect;

    public bool m_useSetActiveForDisplay = true;

    public bool m_autoAddUIEventScript = true;

    [HideInInspector]
    public enUIEventID m_onEnableEventID;

    public stUIEventParams m_onEnableEventParams;

    public UIListElementScript.OnSelectedDelegate onSelected;

    private CanvasGroup m_canvasGroup;

    private string m_dataTag;

    public override void Initialize(UIFormScript formScript)
    {
        if (m_isInitialized)
        {
            return;
        }
        base.Initialize(formScript);
        m_image = gameObject.GetComponent<Image>();
        if (m_image != null)
        {
            m_defaultSprite = m_image.sprite;
            m_defaultColor = m_image.color;
            //if (m_image is Image2)
            //{
            //    Image2 image = m_image as Image2;
            //    m_defaultLayout = image.alphaTexLayout;
            //}
        }
        if (m_autoAddUIEventScript)
        {
            UIEventScript uiEventScript = base.gameObject.GetComponent<UIEventScript>();
            if (uiEventScript == null)
            {
                uiEventScript = base.gameObject.AddComponent<UIEventScript>();
                uiEventScript.Initialize(formScript);
            }
        }
        if (!m_useSetActiveForDisplay)
        {
            m_canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
            if (m_canvasGroup == null)
            {
                m_canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
            }
        }
        m_defaultSize = GetDefaultSize();
        InitRectTransform();
        if (m_textObj != null)
        {
            m_defaultTextColor = m_textObj.color;
        }
    }

    protected override void OnDestroy()
    {
        m_selectFrontObj = null;
        m_selectedSprite = null;
        m_defaultSprite = null;
        m_textObj = null;
        m_image = null;
        onSelected = null;
        m_canvasGroup = null;
        base.OnDestroy();
    }

    protected virtual Vector2 GetDefaultSize()
    {
        return new Vector2(((RectTransform)gameObject.transform).rect.width, ((RectTransform)gameObject.transform).rect.height);
    }

    public void SetDataTag(string dataTag)
    {
        m_dataTag = dataTag;
    }

    public string GetDataTag()
    {
        return m_dataTag;
    }

    public void Enable(UIListScript belongedList, int index, string name, ref stRect rect, bool selected)
    {
        belongedListScript = belongedList;
        m_index = index;
        gameObject.name = name + "_" + index.ToString();
        if (m_useSetActiveForDisplay)
        {
            base.gameObject.SetActive(true);
        }
        else
        {
            m_canvasGroup.alpha = 1f;
            m_canvasGroup.blocksRaycasts = true;
        }
        SetComponentBelongedList(gameObject);
        SetRect(ref rect);
        ChangeDisplay(selected);
        DispatchOnEnableEvent();
    }

    public void Disable()
    {
        if (m_useSetActiveForDisplay)
        {
            gameObject.SetActive(false);
        }
        else
        {
            m_canvasGroup.alpha = 0f;
            m_canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnSelected(BaseEventData baseEventData)
    {
        belongedListScript.SelectElement(m_index, true);
    }

    public virtual void ChangeDisplay(bool selected)
    {
        if (m_image != null && m_selectedSprite != null)
        {
            if (selected)
            {
                m_image.sprite = m_selectedSprite;
                m_image.color = new Color(m_defaultColor.r, m_defaultColor.g, m_defaultColor.b, 255f);
            }
            else
            {
                m_image.sprite = m_defaultSprite;
                m_image.color = m_defaultColor;
            }
            //if (m_image is Image2)
            //{
            //    Image2 image = m_image as Image2;
            //    image.alphaTexLayout = ((!selected) ? m_defaultLayout : m_selectedLayout);
            //}
        }
        if (m_selectFrontObj != null)
        {
            m_selectFrontObj.SetActive(selected);
        }
        if (m_textObj != null)
        {
            m_textObj.color = ((!selected) ? m_defaultTextColor : m_selectTextColor);
        }
    }

    public void SetComponentBelongedList(GameObject gameObject)
    {
        UIComponent[] components = gameObject.GetComponents<UIComponent>();
        if (components != null && components.Length > 0)
        {
            for (int i = 0; i < components.Length; i++)
            {
                components[i].SetBelongedList(belongedListScript, m_index);
            }
        }
        for (int j = 0; j < gameObject.transform.childCount; j++)
        {
            SetComponentBelongedList(gameObject.transform.GetChild(j).gameObject);
        }
    }

    public void SetRect(ref stRect rect)
    {
        m_rect = rect;
        RectTransform rectTransform = gameObject.transform as RectTransform;
        rectTransform.sizeDelta = new Vector2((float)m_rect.m_width, (float)m_rect.m_height);
        if (m_pivotType == enPivotType.Centre)
        {
            rectTransform.anchoredPosition = rect.m_center;
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2((float)rect.m_left, (float)rect.m_top);
        }
    }

    public void SetOnEnableEvent(enUIEventID eventID)
    {
        m_onEnableEventID = eventID;
    }

    public void SetOnEnableEvent(enUIEventID eventID, stUIEventParams eventParams)
    {
        m_onEnableEventID = eventID;
        m_onEnableEventParams = eventParams;
    }

    private void InitRectTransform()
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = ((m_pivotType != enPivotType.Centre) ? new Vector2(0f, 1f) : new Vector2(0.5f, 0.5f));
        rectTransform.sizeDelta = m_defaultSize;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    protected void DispatchOnEnableEvent()
    {
        if (m_onEnableEventID != enUIEventID.None)
        {
            UIEvent uIEvent = Singleton<UIEventManager>.GetInstance().GetUIEvent();
            uIEvent.eventID = m_onEnableEventID;
            uIEvent.eventParams = m_onEnableEventParams;
            uIEvent.srcFormScript = belongedFormScript;
            uIEvent.srcWidgetBelongedListScript = belongedListScript;
            uIEvent.SrcWidgetIndexInBelongedList = indexInList;
            uIEvent.srcWidget = gameObject;
            uIEvent.srcWidgetScript = this;
            uIEvent.pointerEventData = null;
            base.DispatchUIEvent(uIEvent);
        }
    }
}
