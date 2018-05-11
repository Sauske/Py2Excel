using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFormScript : MonoBehaviour,IComparable
{
    private struct stInitWidgetPosition
    {
        public int renderFrameStamp;

        public GameObject widget;

        public Vector3 worldPosition;
    }

    private class AsyncLoadedImage
    {
        public Image image;

        public string prefabPath;

        public bool needCached;

        public bool unloadBelongedAssetBundleAfterLoaded;

        public bool isShowSpecMatrial;

        public AsyncLoadedImage(Image image, string prefabPath, bool needCached, bool unloadBelongedAssetBundleAfterLoaded, bool isShowSpecMatrial = false)
        {
            this.image = image;
            this.prefabPath = prefabPath;
            this.needCached = needCached;
            this.unloadBelongedAssetBundleAfterLoaded = unloadBelongedAssetBundleAfterLoaded;
            this.isShowSpecMatrial = isShowSpecMatrial;
        }
    }

    private Vector2 referenceResolution = new Vector2(1920, 1080f);

    public bool fullScreenBG;

    public bool isSingleton;

    public int group;

    public bool hideUnderForms;

    public bool disableInput;

    public bool isModal;

    public GameObject[] formWidgets = new GameObject[0];

    public enFormPriority priority;

    public bool alwaysKeepVisible;

    public bool enableMultiClickedEvent = true;

    public string formFadeInAnimationName = string.Empty;

    public enFormFadeAnimationType formFadeInAnimationType;

    public string formFadeOutAnimationName = string.Empty;

    public enFormFadeAnimationType formFadeOutAnimationType;
    
    [HideInInspector]
    public CanvasScaler canvasScaler;

    [HideInInspector]
    public string formPath;

    [HideInInspector]
    public bool useFormPool;

    [HideInInspector]
    public enUIEventID revertToHideEvent;

    [HideInInspector]
    public enUIEventID revertToVisibleEvent;

    [HideInInspector]
    public enUIEventID[] eventIDs = new enUIEventID[Enum.GetValues(typeof(enFormEventType)).Length];

    public stUIEventParams[] eventParams = new stUIEventParams[Enum.GetValues(typeof(enFormEventType)).Length];

    [HideInInspector]
    public int clickedEventDispatchedCounter;

    private enFormPriority m_defaultPriority;

    private bool m_isNeedClose;

    private bool m_isClosed;

    private bool m_isInFadeIn;

    private bool m_isInFadeOut;

    private int m_openOrder;

    private int m_sortingOrder;

    private int m_sequence;

    private int m_hideFlags;

    private bool m_isHided;

    private bool m_isInitialized;

    private int m_renderFrameStamp;

    private List<UIFormScript.stInitWidgetPosition> m_initWidgetPositions;

    private UIComponent m_formFadeInAnimationScript;

    private UIComponent m_formFadeOutAnimationScript;

    private GraphicRaycaster m_graphicRaycaster;

    private List<UIComponent> m_uiComponents;

    private Canvas m_canvas;

    [HideInInspector]
    private List<GameObject> m_relatedScenes;

    [HideInInspector]
    private List<List<Camera>> m_relatedSceneCamera;

    private List<UIFormScript.AsyncLoadedImage> m_asyncLoadedImages;


    private Dictionary<string, GameObject> m_loadedSpriteDictionary;

    public Canvas canvas
    {
            get
        {
            return m_canvas;
        }

    }

    private void Awake()
    {
        m_uiComponents = new List<UIComponent>();
        m_relatedScenes = new List<GameObject>();
        m_relatedSceneCamera = new List<List<Camera>>();

        InitializeCanvas();
    }

    public void InitializeCanvas()
    {
        m_canvas = gameObject.GetComponent<Canvas>();
        canvasScaler = gameObject.GetComponent<CanvasScaler>();
    }

    public void MatchScreen()
    {
        if (canvasScaler == null)
        {
            return;
        }
        canvasScaler.referenceResolution = referenceResolution;
        if ((float)Screen.width / canvasScaler.referenceResolution.x > (float)Screen.height / canvasScaler.referenceResolution.y)
        {
            if (fullScreenBG)
            {
                canvasScaler.matchWidthOrHeight = 0f;
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 1f;
            }
        }
        else if (fullScreenBG)
        {
            canvasScaler.matchWidthOrHeight = 1f;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0f;
        }
    }

    private void OnDestroy()
    {
        if (m_asyncLoadedImages != null)
        {
            m_asyncLoadedImages.Clear();
            m_asyncLoadedImages = null;
        }

        if (m_loadedSpriteDictionary != null)
        {
            m_loadedSpriteDictionary.Clear();
            m_loadedSpriteDictionary = null;
        }
    }


    public void CustomUpdate()
    {
        UpdateFadeIn();
        UpdateFadeOut();
    }

    public void CustomLateUpdate()
    {
        if (m_initWidgetPositions != null)
        {
            int i = 0;
            while (i < m_initWidgetPositions.Count)
            {
                UIFormScript.stInitWidgetPosition stInitWidgetPosition = m_initWidgetPositions[i];
                if (m_renderFrameStamp - stInitWidgetPosition.renderFrameStamp <= 1)
                {
                    if (stInitWidgetPosition.widget != null)
                    {
                        stInitWidgetPosition.widget.transform.position = stInitWidgetPosition.worldPosition;
                    }
                    i++;
                }
                else
                {
                    this.m_initWidgetPositions.RemoveAt(i);
                }
            }
        }
        UpdateAsyncLoadedImage();
        clickedEventDispatchedCounter = 0;
        m_renderFrameStamp++;
    }

    /// <summary>
    /// 开始播放窗体的淡入动画
    /// </summary>
    private void StartFadeIn()
    {
        if (formFadeInAnimationType == enFormFadeAnimationType.None || string.IsNullOrEmpty(formFadeInAnimationName))
        {
            return;
        }

        enFormFadeAnimationType eFormFadeInAnimationType = formFadeInAnimationType;
        if (eFormFadeInAnimationType != enFormFadeAnimationType.Animation)
        {
            if (eFormFadeInAnimationType == enFormFadeAnimationType.Animator)
            {
                m_formFadeInAnimationScript = gameObject.GetComponent<UIAnimatorScript>();
                if (m_formFadeInAnimationScript != null)
                {
                    ((UIAnimatorScript)m_formFadeInAnimationScript).PlayAnimator(formFadeInAnimationName);
                    m_isInFadeIn = true;
                }
            }
        }
        else
        {
            m_formFadeInAnimationScript = gameObject.GetComponent<UIAnimationScript>();
            if (m_formFadeInAnimationScript != null)
            {
                ((UIAnimationScript)m_formFadeInAnimationScript).PlayAnimation(formFadeInAnimationName, true);
                m_isInFadeIn = true;
            }
        }
    }

    /// <summary>
    /// 开始播放窗体的淡出动画
    /// </summary>
    private void StartFadeOut()
    {
        if (formFadeOutAnimationType == enFormFadeAnimationType.None || string.IsNullOrEmpty(formFadeOutAnimationName))
        {
            return;
        }
        enFormFadeAnimationType eFormFadeOutAnimationType = formFadeOutAnimationType;
        if (eFormFadeOutAnimationType != enFormFadeAnimationType.Animation)
        {
            if (eFormFadeOutAnimationType == enFormFadeAnimationType.Animator)
            {
                m_formFadeOutAnimationScript = gameObject.GetComponent<UIAnimatorScript>();
                if (m_formFadeOutAnimationScript != null)
                {
                    ((UIAnimatorScript)m_formFadeOutAnimationScript).PlayAnimator(formFadeOutAnimationName);
                    m_isInFadeOut = true;
                }
            }
        }
        else
        {
            m_formFadeOutAnimationScript = gameObject.GetComponent<UIAnimationScript>();
            if (m_formFadeOutAnimationScript != null)
            {
                ((UIAnimationScript)m_formFadeOutAnimationScript).PlayAnimation(formFadeOutAnimationName, true);
                m_isInFadeOut = true;
            }
        }
    }

    /// <summary>
    /// 检测窗体的淡入动画是否结束
    /// </summary>
    private void UpdateFadeIn()
    {
        if (m_isInFadeIn)
        {
            enFormFadeAnimationType eFormFadeInAnimationType = formFadeInAnimationType;
            if (eFormFadeInAnimationType != enFormFadeAnimationType.Animation)
            {
                if (eFormFadeInAnimationType == enFormFadeAnimationType.Animator)
                {
                    if (m_formFadeInAnimationScript == null || ((UIAnimatorScript)m_formFadeInAnimationScript).IsAnimationStopped(formFadeInAnimationName))
                    {
                        m_isInFadeIn = false;
                    }
                }
            }
            else if (m_formFadeInAnimationScript == null || ((UIAnimationScript)m_formFadeInAnimationScript).IsAnimationStopped(formFadeInAnimationName))
            {
                m_isInFadeIn = false;
            } 
        }
    }

    /// <summary>
    /// 检测窗体的淡出动画是否结束
    /// </summary>
    private void UpdateFadeOut()
    {
        if (m_isInFadeOut)
        {
            enFormFadeAnimationType eFormFadeOutAnimationType = formFadeOutAnimationType;
            if (eFormFadeOutAnimationType != enFormFadeAnimationType.Animation)
            {
                if (eFormFadeOutAnimationType == enFormFadeAnimationType.Animator)
                {
                    if (m_formFadeOutAnimationScript == null || ((UIAnimatorScript)m_formFadeOutAnimationScript).IsAnimationStopped(formFadeOutAnimationName))
                    {
                        m_isInFadeOut = false;
                    }
                }
            }
            else if (m_formFadeOutAnimationScript == null || ((UIAnimationScript)m_formFadeOutAnimationScript).IsAnimationStopped(formFadeOutAnimationName))
            {
                m_isInFadeOut = false;
            }
        }
    }

    public void Appear(enFormHideFlag hideFlag = enFormHideFlag.HideByCustom, bool dispatchVisibleChangedEvent = true)
    {
        if (alwaysKeepVisible)
        {
            return;
        }

        m_hideFlags &= (int)(~(int)hideFlag);
        if (m_hideFlags != 0 || !m_isHided)
        {
            return;
        }
        m_isHided = false;
        if (m_canvas != null)
        {
            m_canvas.enabled = true;
            m_canvas.sortingOrder = m_sortingOrder;
        }

        for (int i = 0; i < m_relatedScenes.Count; ++i)
        {
            UIUtility.SetGameObjectLayer(this.m_relatedScenes[i], 18);
            SetSceneCameraEnable(i, true);    
        }
        AppearComponent();
        DispatchRevertVisibleFormEvent();
        if (dispatchVisibleChangedEvent)
        {
            DispatchVisibleChangedEvent();
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        if (active)
        {
            Appear(enFormHideFlag.HideByCustom, true);
        }
        else
        {
            Hide(enFormHideFlag.HideByCustom, true);
        }
    }

    public void Hide(enFormHideFlag hideFlag = enFormHideFlag.HideByCustom, bool dispatchVisibleChangedEvent = true)
    {
        if (alwaysKeepVisible)
        {
            return;
        }
        m_hideFlags |= (int)hideFlag;
        if (m_hideFlags == 0 || m_isHided)
        {
            return;
        }
        m_isHided = true;
        if (m_canvas != null)
        {
            m_canvas.enabled = false;
        }
        
        for (int i = 0; i < m_relatedScenes.Count; i++)
        {
            UIUtility.SetGameObjectLayer(m_relatedScenes[i], 31);
            SetSceneCameraEnable(i, false);
        }
        HideComponent();
        if (revertToHideEvent != enUIEventID.None)
        {
            Singleton<UIEventManager>.GetInstance().DispatchUIEvent(revertToHideEvent);
        }
        if (dispatchVisibleChangedEvent)
        {
            DispatchVisibleChangedEvent();
        }
    }

    /// <summary>
    /// 该窗体为打开状态，并设置Canvas的渲染模式
    /// </summary>
    /// <param name="inFormPath">窗体路径</param>
    /// <param name="camera">摄像头</param>
    /// <param name="sequence">窗体序列</param>
    /// <param name="exist"></param>
    /// <param name="openOrder">窗体索引</param>
    public void Open(string inFormPath, Camera camera, int sequence, bool exist, int openOrder)
    {
        formPath = inFormPath;
        if (m_canvas != null)
        {
            m_canvas.worldCamera = camera;
            if (camera == null)
            {
                if (m_canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                }
            }
            else if (m_canvas.renderMode != RenderMode.ScreenSpaceCamera)
            {
                m_canvas.renderMode = RenderMode.ScreenSpaceCamera;
            }
            m_canvas.pixelPerfect = true;
        }
        RefreshCanvasScaler();
        Open(sequence, exist, openOrder);
    }

    /// <summary>
    /// 该窗体为打开状态，并初始化窗体信息,设置窗体的顺序（Order）
    /// </summary>
    /// <param name="sequence">窗体序列</param>
    /// <param name="exist"></param>
    /// <param name="openOrder">窗体索引</param>
    public void Open(int sequence, bool exist, int openOrder)
    {
        m_isNeedClose = false;
        m_isClosed = false;
        m_isInFadeIn = false;
        m_isInFadeOut = false;
        clickedEventDispatchedCounter = 0;
        m_sequence = sequence;
        SetDisplayOrder(openOrder);
        m_renderFrameStamp = 0;
        if (!exist)
        {
            Initialize();
            DispatchFormEvent(enFormEventType.Open);
        }
    }
    
    /// <summary>
    /// 将该窗体改为需要关闭状态
    /// </summary>
    public void Close()
    {
        if (m_isNeedClose)
        {
            return;
        }
        m_isNeedClose = true;
        DispatchFormEvent(enFormEventType.Close);
        CloseComponent();
    }

    public void SetSceneCameraEnable(int index, bool bEnable)
    {
        if (index < 0 || index >= m_relatedSceneCamera.Count || m_relatedSceneCamera[index] == null)
        {
            return;
        }
        for (int i = 0; i < m_relatedSceneCamera[index].Count; i++)
        {
            if (m_relatedSceneCamera[index][i] != null)
            {
                m_relatedSceneCamera[index][i].enabled = bEnable;
            }
        }
    }

    private void RefreshCanvasScaler()
    {
        try
        {
            if (canvasScaler != null)
            {
                canvasScaler.enabled = false;
                canvasScaler.enabled = true;
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.AssertFormat(false, "Error form {0}: message: {1}, callstack: {2}", new object[]
			{
				base.name,
				ex.Message,
				ex.StackTrace
			});
        }
    }

    /// <summary>
    /// 是否覆盖
    /// 在canvas不为空时，下面条件只要满足一个就返回true
    /// 1：Canvas.renderMode = RenderMode.ScreenSpaceOverlay
    /// 2：m_canvas.worldCamera = null
    /// </summary>
    /// <returns></returns>
    private bool IsOverlay()
    {
        return !(m_canvas == null) && (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || m_canvas.worldCamera == null);
    }

    /// <summary>
    /// 计算窗体的排序顺序（sorting order）
    /// 1：RenderMode.ScreenSpaceOverlay模式下,Order的在1000+ (int)priority * 1000 + openOrder * 10；
    /// 2：RenderMode.ScreenSpaceCamera模式下,Order的在0+ (int)priority * 1000 + openOrder * 10；
    /// </summary>
    /// <param name="priority">优先级</param>
    /// <param name="openOrder">窗体索引</param>
    /// <returns>顺序</returns>
    public int CalculateSortingOrder(enFormPriority priority, int openOrder)
    {
        
        if (openOrder * 10 >= 1000)
        {
            openOrder %= 100;
        }
        return (int)(((!IsOverlay()) ? 0 : 10000) + (int)priority * 1000 + openOrder * 10);
    }

    private void SetComponentSortingOrder(int sortingOrder)
    {
        for (int i = 0; i < m_uiComponents.Count; i++)
        {
            m_uiComponents[i].SetSortingOrder(sortingOrder);
        }
    }

    /// <summary>
    /// 设置窗体的排序顺序（Sorting Order）
    /// </summary>
    /// <param name="openOrder">窗体索引</param>
    public void SetDisplayOrder(int openOrder)
    {
        m_openOrder = openOrder;
        if (m_canvas != null)
        {
            m_sortingOrder = CalculateSortingOrder(priority, m_openOrder);
            m_canvas.sortingOrder = m_sortingOrder;
            try
            {
                if (m_canvas.enabled)
                {
                    m_canvas.enabled = false;
                    m_canvas.enabled = true;
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.AssertFormat(false, "Error form {0}: message: {1}, callstack: {2}", new object[]
                {
                    name,
                    ex.Message,
                    ex.StackTrace
                });
            }
        }
        SetComponentSortingOrder(m_sortingOrder);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialize()
    {
        if (m_isInitialized)
        {
            return;
        }
        m_defaultPriority = priority;
        InitializeComponent(gameObject);
        m_isInitialized = true;
    }

    /// <summary>
    /// 初始化所有的UIComponent（包含UIComponent子类）组件
    /// </summary>
    /// <param name="root"></param>
    public void InitializeComponent(GameObject root)
    {
        UIComponent[] components = root.GetComponents<UIComponent>();
        if (components != null && components.Length > 0)
        {
            for (int i = 0; i < components.Length; i++)
            {
                components[i].Initialize(this);
            }
        }
        for (int j = 0; j < root.transform.childCount; j++)
        {
            InitializeComponent(root.transform.GetChild(j).gameObject);
        }
    }

    public void InitializeWidgetPosition(int widgetIndex, Vector3 worldPosition)
    {
        InitializeWidgetPosition(GetWidget(widgetIndex), worldPosition);
    }

    public void InitializeWidgetPosition(GameObject widget, Vector3 worldPosition)
    {
        if (m_initWidgetPositions == null)
        {
            m_initWidgetPositions = new List<UIFormScript.stInitWidgetPosition>();
        }
        UIFormScript.stInitWidgetPosition item = default(UIFormScript.stInitWidgetPosition);
        item.renderFrameStamp = this.m_renderFrameStamp;
        item.widget = widget;
        item.worldPosition = worldPosition;
        m_initWidgetPositions.Add(item);
    }

    /// <summary>
    /// 设置窗体的优先级
    /// </summary>
    /// <param name="formPriority"></param>
    public void SetPriority(enFormPriority formPriority)
    {
        if (priority == formPriority)
        {
            return;
        }
        priority = formPriority;
        SetDisplayOrder(m_openOrder);
        DispatchChangeFormPriorityEvent();
    }

    /// <summary>
    /// 重置窗体的优先级
    /// </summary>
    public void RestorePriority()
    {
        SetPriority(m_defaultPriority);
    }

    /// <summary>
    /// 关闭所有的UIComponent（包含UIComponent子类）组件
    /// </summary>
    private void CloseComponent()
    {
        for (int i = 0; i < m_uiComponents.Count; i++)
        {
            m_uiComponents[i].Close();
        }
    }

    /// <summary>
    /// 隐藏所有的UIComponent（包含UIComponent子类）组件
    /// </summary>
    private void HideComponent()
    {
        for (int i = 0; i < m_uiComponents.Count; i++)
        {
            m_uiComponents[i].Hide();
        }
    }

    /// <summary>
    /// 显示所有的UIComponent（包含UIComponent子类）组件
    /// </summary>
    private void AppearComponent()
    {
        for (int i = 0; i < m_uiComponents.Count; i++)
        {
            m_uiComponents[i].Appear();
        }
    }


    private void DispatchFormEvent(enFormEventType formEventType)
    {
        if (eventIDs[(int)formEventType] == enUIEventID.None)
        {
            return;
        }
        UIEvent uiEvent = Singleton<UIEventManager>.GetInstance().GetUIEvent();
        uiEvent.eventID = eventIDs[(int)formEventType];
        uiEvent.eventParams = eventParams[(int)formEventType];
        uiEvent.srcFormScript = this;
        uiEvent.srcWidget = null;
        uiEvent.srcWidgetScript = null;
        uiEvent.srcWidgetBelongedListScript = null;
        uiEvent.SrcWidgetIndexInBelongedList = 0;
        uiEvent.pointerEventData = null;
        Singleton<UIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
    }

    private void DispatchChangeFormPriorityEvent()
    {
        UIEvent uiEvent = Singleton<UIEventManager>.GetInstance().GetUIEvent();
        uiEvent.eventID = enUIEventID.UI_OnFormPriorityChanged;
        uiEvent.srcFormScript = this;
        uiEvent.srcWidget = null;
        uiEvent.srcWidgetScript = null;
        uiEvent.srcWidgetBelongedListScript = null;
        uiEvent.SrcWidgetIndexInBelongedList = 0;
        uiEvent.pointerEventData = null;
        Singleton<UIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
    }

    private void DispatchVisibleChangedEvent()
    {
        UIEvent uiEvent = Singleton<UIEventManager>.GetInstance().GetUIEvent();
        uiEvent.eventID = enUIEventID.UI_OnFormVisibleChanged;
        uiEvent.srcFormScript = this;
        uiEvent.srcWidget = null;
        uiEvent.srcWidgetScript = null;
        uiEvent.srcWidgetBelongedListScript = null;
        uiEvent.SrcWidgetIndexInBelongedList = 0;
        uiEvent.pointerEventData = null;
        Singleton<UIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
    }

    private void DispatchRevertVisibleFormEvent()
    {
        if (revertToVisibleEvent == enUIEventID.None)
        {
            return;
        }
        UIEvent uiEvent = Singleton<UIEventManager>.GetInstance().GetUIEvent();
        uiEvent.eventID = revertToVisibleEvent;
        uiEvent.srcFormScript = this;
        uiEvent.srcWidget = null;
        uiEvent.srcWidgetScript = null;
        uiEvent.srcWidgetBelongedListScript = null;
        uiEvent.SrcWidgetIndexInBelongedList = 0;
        uiEvent.pointerEventData = null;
        Singleton<UIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
    }
     
    /// <summary>
    /// 获取窗体序列
    /// </summary>
    /// <returns>窗体序列</returns>
    public int GetSequence()
    {
        return m_sequence;     
    }

    /// <summary>
    /// 排序比较函数
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int CompareTo(object obj)
    {
        UIFormScript uiFormScript = obj as UIFormScript;
        if (m_sortingOrder > uiFormScript.m_sortingOrder)
        {
            return 1;
        }
        else if (m_sortingOrder == uiFormScript.m_sortingOrder)
        {
            return 0;
        }
        return -1;

    }

    public GameObject GetWidget(int index)
    {
        if (index < 0 || index >= formWidgets.Length)
        {
            return null;
        }
        return formWidgets[index];
    }

     /// <summary>
     /// 获取渲染UGUI的摄像机
     /// </summary>
     /// <returns></returns>
    public Camera GetCamera()
    {
        if (m_canvas == null || m_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }
        return m_canvas.worldCamera;
    }

    /// <summary>
    /// 窗体是否处于隐藏状态
    /// </summary>
    /// <returns></returns>
    public bool IsHided()
    {
        return m_isHided;
    }

     /// <summary>
    /// 窗体是否处于关闭状态
     /// </summary>
     /// <returns></returns>
    public bool IsClosed()
    {
        return m_isClosed;
    }

    /// <summary>
    /// 窗体是否处于需要关闭状态
    /// </summary>
    /// <returns></returns>
    public bool IsNeedClose()
    {
        return m_isNeedClose;
    }

    /// <summary>
    /// 窗体从需要关闭状态转变为关闭状态
    /// </summary>
    /// <param name="ignoreFadeOut"></param>
    /// <returns></returns>
    public bool TurnToClosed(bool ignoreFadeOut)
    {
        m_isNeedClose = false;
        m_isClosed = true;
        Singleton<EventRouter>.GetInstance().BroadCastEvent<string>(EventID.UI_FORM_CLOSED, formPath);
        if (ignoreFadeOut || !IsNeedFadeOut())
        {
            return true;
        }
        StartFadeOut();
        return false;
    }

    /// <summary>
    /// 窗体是否需要播放淡入动画
    /// </summary>
    /// <returns></returns>
    public bool IsNeedFadeIn()
    {
        return GameSettings.renderQuality != GameRenderQuality.Low && formFadeInAnimationType != enFormFadeAnimationType.None && !string.IsNullOrEmpty(formFadeInAnimationName);
    }

    /// <summary>
    /// 窗体是否需要播放淡出动画
    /// </summary>
    /// <returns></returns>
    public bool IsNeedFadeOut()
    {
        return false;
    }

    /// <summary>
    /// Canvas是否处于激活状态
    /// </summary>
    /// <returns></returns>
    public bool IsCanvasEnabled()
    {
        return !(m_canvas == null) && m_canvas.enabled;
    }

    /// <summary>
    /// 窗体是否正在播放淡入动画
    /// </summary>
    /// <returns></returns>
    public bool IsInFadeIn()
    {
        return m_isInFadeIn;
    }

    /// <summary>
    /// 窗体是否正在播放淡出动画
    /// </summary>
    /// <returns></returns>
    public bool IsInFadeOut()
    {
        return m_isInFadeOut;
    }

    public Vector2 GetReferenceResolution()
    {
        return (!(canvasScaler == null)) ? canvasScaler.referenceResolution : Vector2.zero;
    }

    /// <summary>
    /// 获取窗体的排序顺序值
    /// </summary>
    /// <returns></returns>
    public int GetSortingOrder()
    {
        return m_sortingOrder;
    }

    public GraphicRaycaster GetGraphicRaycaster()
    {
        return m_graphicRaycaster;
    }

    public float GetScreenScaleValue()
    {
        float result = 1f;
        RectTransform component = base.GetComponent<RectTransform>();
        if (component && canvasScaler.matchWidthOrHeight == 0f)
        {
            result = component.rect.width / component.rect.height / (canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y);
        }
        return result;
    }

    public void AddUIComponent(UIComponent uiComponent)
    {
        if (uiComponent != null && m_uiComponents.Contains(uiComponent))
        {
            m_uiComponents.Add(uiComponent);
        }
    }

    public void RemoveUIComponent(UIComponent uiComponent)
    {
        if (m_uiComponents.Contains(uiComponent))
        {
            m_uiComponents.Remove(uiComponent);
        }
    }

    public bool IsRelatedSceneExist(string sceneName)
    {
        for (int i = 0; i < m_relatedScenes.Count; ++i)
        {
            if (string.Equals(sceneName, m_relatedScenes[i].name))
            {
                return true;
            }
        }
        return false;
    }

    public void AddRelatedScene(GameObject scene, string sceneName)
    {
        scene.name = sceneName;
        scene.transform.SetParent(gameObject.transform);
        scene.transform.localPosition = Vector3.zero;
        scene.transform.localRotation = Quaternion.identity;
        m_relatedScenes.Add(scene);
        m_relatedSceneCamera.Add(new List<Camera>());
        AddRelatedSceneCamera(m_relatedSceneCamera.Count - 1, scene);
    }

    public void AddRelatedSceneCamera(int index, GameObject go)
    {
        if (index < 0 || index >= m_relatedSceneCamera.Count || go == null)
        {
            return;
        }
        Camera component = go.GetComponent<Camera>();
        if (component != null)
        {
            m_relatedSceneCamera[index].Add(component);
        }
        for (int i = 0; i < go.transform.childCount; i++)
        {
            AddRelatedSceneCamera(index, go.transform.GetChild(i).gameObject);
        }
    }

    public float ChangeScreenValueToForm(float value)
    {
        if (canvasScaler.matchWidthOrHeight == 0f)
        {
            return value * canvasScaler.referenceResolution.x / (float)Screen.width;
        }
        if (canvasScaler.matchWidthOrHeight == 1f)
        {
            return value * canvasScaler.referenceResolution.y / (float)Screen.height;
        }
        return value;
    }

    public float ChangeFormValueToScreen(float value)
    {
        if (canvasScaler.matchWidthOrHeight == 0f)
        {
            return value * (float)Screen.width /canvasScaler.referenceResolution.x;
        }
        if (canvasScaler.matchWidthOrHeight == 1f)
        {
            return value * (float)Screen.height / canvasScaler.referenceResolution.y;
        }
        return value;
    }

    public void AddAsyncLoadedImage(Image image, string prefabPath, bool needCached, bool unloadBelongedAssetBundleAfterLoaded, bool isShowSpecMatrial = false)
    {
        if (m_asyncLoadedImages == null)
        {
            m_asyncLoadedImages = new List<AsyncLoadedImage>();
        }
        if (m_loadedSpriteDictionary == null)
        {
            m_loadedSpriteDictionary = new Dictionary<string, GameObject>();
        }
        for (int i = 0; i < m_asyncLoadedImages.Count; i++)
        {
            if (m_asyncLoadedImages[i].image == image)
            {
                m_asyncLoadedImages[i].prefabPath = prefabPath;
                return;
            }
        }
        UIFormScript.AsyncLoadedImage item = new UIFormScript.AsyncLoadedImage(image, prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded, isShowSpecMatrial);
        m_asyncLoadedImages.Add(item);

    }

    private void UpdateAsyncLoadedImage()
    {
        if (m_asyncLoadedImages == null)
        {
            return;
        }
        bool flag = false;
        int i = 0;
        while (i < m_asyncLoadedImages.Count)
        {
            UIFormScript.AsyncLoadedImage asyncLoadedImage = m_asyncLoadedImages[i];
            Image image = asyncLoadedImage.image;
            if (image != null)
            {
                GameObject go = null;
                string prefabPath = asyncLoadedImage.prefabPath;
                if (!m_loadedSpriteDictionary.TryGetValue(prefabPath, out go) && !flag)
                {
                    go = UIUtility.GetSpritePrefeb(prefabPath, asyncLoadedImage.needCached, asyncLoadedImage.unloadBelongedAssetBundleAfterLoaded);
                    m_loadedSpriteDictionary.Add(prefabPath, gameObject);
                    flag = true;
                }
                if (go != null)
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
                    image.SetSprite(go, asyncLoadedImage.isShowSpecMatrial);
                    m_asyncLoadedImages.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            else
            {
                m_asyncLoadedImages.RemoveAt(i);
            }
        }
    }


}
