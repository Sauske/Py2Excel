using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : Singleton<UIManager>
{
    public delegate void OnFormSorted(List<UIFormScript> inForms);

    public UIManager.OnFormSorted onFormSorted;

    public static int suiSystemRenderFrameCounter;

    private Camera m_formCamera;

    private bool m_needSortForms;

    private List<int> m_existFormSequences;

    private int m_formSequence;

    private GameObject m_uiRoot;

    private bool m_needUpdateRaycasterAndHide;

    private EventSystem m_uiInputEventSystem;

    private List<UIFormScript> m_forms;

    private List<UIFormScript> m_pooledForms;

    private static string s_formCameraName = "Camera_Form";
    private static string s_uiSceneName = "UI_Scene";

    public Camera formCamera
    {
        get
        {
            return m_formCamera;
        }
    }

    public override void Init()
    {
        m_forms = new List<UIFormScript>();
        m_pooledForms = new List<UIFormScript>();
        m_formSequence = 0;
        m_existFormSequences = new List<int>();
        UIManager.suiSystemRenderFrameCounter = 0;
        CreateUIRoot();
        CreateEventSystem();
        CreateCamera();
        CreateUIScene();
        base.Init();
    }

    
    /// <summary>
    /// 创建UI根节点
    /// </summary>
    private void CreateUIRoot()
    {
        m_uiRoot = new GameObject("UIManager");
        GameObject obj2 = GameObject.Find("BootObj");
        if(obj2 != null)
        {
            m_uiRoot.transform.parent = obj2.transform;
        }
    }

    /// <summary>
    /// 创建渲染UI所需的摄像机
    /// </summary>
    private void CreateCamera()
    {
        GameObject obj2 = new GameObject(s_formCameraName);
        obj2.transform.SetParent(m_uiRoot.transform,true);
        obj2.transform.localPosition = Vector3.zero;
        obj2.transform.localRotation = Quaternion.identity;
        obj2.transform.localScale = Vector3.one;
        Camera camera = obj2.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 50f;
        camera.clearFlags = CameraClearFlags.Depth;
        camera.cullingMask = 32;
        camera.depth = 10f;
        m_formCamera = camera;
    }

    /// <summary>
    /// 创建UI事件系统
    /// </summary>
    private void CreateEventSystem()
    {
        m_uiInputEventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
        if (m_uiInputEventSystem == null)
        {
            GameObject obj2 = new GameObject("EventSystem");
            m_uiInputEventSystem = obj2.AddComponent<EventSystem>();
            obj2.AddComponent<StandaloneInputModule>();
        }
        m_uiInputEventSystem.gameObject.transform.parent = m_uiRoot.transform;
    }

    private void CreateUIScene()
    {
        GameObject obj2 = new GameObject(s_uiSceneName)
        {
            transform = { parent = m_uiRoot.transform }
        };
    }

    public void Update()
    {
        for (int i = 0; i < m_forms.Count; i++)
        {
            m_forms[i].CustomUpdate();
            if (m_forms[i].IsNeedClose())
            {
                if (m_forms[i].TurnToClosed(false))
                {
                    RecycleForm(i);
                    m_needSortForms = true;
                    continue;
                }
            }
            else if (m_forms[i].IsClosed() && !m_forms[i].IsInFadeOut())
            {
                RecycleForm(i);
                m_needSortForms = true;
                continue;
            }
        }
        if (m_needSortForms)
        {
            ProcessFormList(true, true);
        }
        else if (m_needUpdateRaycasterAndHide)
        {
            ProcessFormList(false, true);
        }
        m_needSortForms = false;
        m_needUpdateRaycasterAndHide = false;
    }


    public void LateUpdate()
    {
        for (int i = 0; i < m_forms.Count; i++)
        {
            m_forms[i].CustomLateUpdate();
        }
        UIManager.suiSystemRenderFrameCounter++;
    }

    /// <summary>
    /// 打开窗体
    /// </summary>
    /// <param name="formPath">窗体路径</param>
    /// <param name="useFormPool">是否从窗体对象次取</param>
    /// <param name="useCameraRenderMode">是否使用摄像机渲染</param>
    /// <returns></returns>
    public UIFormScript OpenForm(string formPath, bool useFormPool, bool useCameraRenderMode = true)
    {
        UIFormScript uiFormScript = GetUnCloseForm(formPath);
        if (uiFormScript != null && uiFormScript.isSingleton)
        {
            RemoveFromExistFormSequenceList(uiFormScript.GetSequence());
            AddToExistFormSequenceList(m_formSequence);
            int formOpenOrder = GetFormOpenOrder(m_formSequence);
            uiFormScript.Open(m_formSequence, true, formOpenOrder);
            m_formSequence++;
            m_needSortForms = true;
            return uiFormScript;
        }
        GameObject gameObject = CreateForm(formPath, useFormPool);
        if (gameObject == null)
        {
            return null;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        string formName = GetFormName(formPath);
        gameObject.name = formName;
        if (gameObject.transform.parent != m_uiRoot.transform)
        {
            gameObject.transform.SetParent(m_uiRoot.transform);
        }
        uiFormScript = gameObject.GetComponent<UIFormScript>();
        if (uiFormScript != null)
        {
            AddToExistFormSequenceList(m_formSequence);
            int formOpenOrder2 = GetFormOpenOrder(m_formSequence);
            uiFormScript.Open(formPath,(!useCameraRenderMode) ? null : m_formCamera,m_formSequence,false,formOpenOrder2);
            if(uiFormScript.group > 0)
            {
                CloseGroupForm(uiFormScript.group);    
            }
            m_forms.Add(uiFormScript);
        }
        m_formSequence++;
        m_needSortForms = true;
        return uiFormScript;
    }

    /// <summary>
    ///根据窗体路径关闭窗体
    /// </summary>
    /// <param name="formPath">窗体路径</param>
    public void CloseForm(string formPath)
    {
        for (int i = 0; i < m_forms.Count; i++)
        {
            if (string.Equals(m_forms[i].formPath, formPath))
            {
                m_forms[i].Close();
            }
        }
    }

    /// <summary>
    ///根据窗体对象关闭窗体 
    /// </summary>
    /// <param name="formScript">窗体对象</param>
    public void CloseForm(UIFormScript formScript)
    {
        for (int i = 0; i < m_forms.Count; i++)
        {
            if (m_forms[i] == formScript)
            {
                m_forms[i].Close();
            }
        }
    }

    /// <summary>
    /// 根据窗体序列关闭窗体
    /// </summary>
    /// <param name="formSequence">窗体序列</param>
    public void CloseForm(int formSequence)
    {
        for (int i = 0; i < m_forms.Count; i++)
        {
            if (m_forms[i].GetSequence() == formSequence)
            {
                m_forms[i].Close();
            }
        }
    }

    public void CloseGroupForm(int group)
    {
        if (group == 0)
        {
            return;
        }
        for (int i = 0; i < m_forms.Count; ++i)
        {
            if (m_forms[i].group == group)
            {
                m_forms[i].Close();
            }
        }
    }

    //public void CloseAllFormExceptLobby(bool closeImmediately = true)
    //{
    //    string[] exceptFormNames = new string[]
    //        {
    //            CLobbySystem.LOBBY_FORM_PATH,
    //            CLobbySystem.SYSENTRY_FORM_PATH,
    //            CChatController.ChatFormPath,
    //            CLobbySystem.RANKING_BTN_FORM_PATH
    //        };
    //    Singleton<CUIManager>.GetInstance().CloseAllForm(exceptFormNames, closeImmediately, true);
    //}

    public void CloseAllForm(string[] exceptFormNames = null, bool closeImmediately = true, bool clearFormPool = true)
    {
        for (int i = 0; i < m_forms.Count; i++)
        {
            bool flag = true;
            if (exceptFormNames != null)
            {
                for (int j = 0; j < exceptFormNames.Length; j++)
                {
                    if (string.Equals(m_forms[i].formPath, exceptFormNames[j]))
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag)
            {
                m_forms[i].Close();
            }
        }
        if (closeImmediately)
        {
            int k = 0;
            while (k < m_forms.Count)
            {
                if (m_forms[k].IsNeedClose() || m_forms[k].IsClosed())
                {
                    if (m_forms[k].IsNeedClose())
                    {
                        m_forms[k].TurnToClosed(true);
                    }
                    RecycleForm(k);
                }
                else
                {
                    k++;
                }
            }
            if (exceptFormNames != null)
            {
                ProcessFormList(true, true);
            }
        }
        if (clearFormPool)
        {
            ClearFormPool();
        }
    }

    /// <summary>
    /// 回收窗体
    /// </summary>
    /// <param name="formIndex">窗体索引</param>
    private void RecycleForm(int formIndex)
    {
        RemoveFromExistFormSequenceList(m_forms[formIndex].GetSequence());
        RecycleForm(m_forms[formIndex]);
        m_forms.RemoveAt(formIndex);
    }

    /// <summary>
    /// 回收窗体
    /// </summary>
    /// <param name="formScript">窗体对象</param>
    private void RecycleForm(UIFormScript formScript)
    {
        if (formScript == null)
        {
            return;
        }
        if (formScript.useFormPool)
        {
            formScript.Hide(enFormHideFlag.HideByCustom, true);
            m_pooledForms.Add(formScript);
        }
        else
        {
            try
            {
                if (formScript.canvasScaler != null)
                {
                    formScript.canvasScaler.enabled = false;
                }
                UnityEngine.Object.Destroy(formScript.gameObject);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.AssertFormat(false, "Error destroy {0} formScript gameObject: message: {1}, callstack: {2}", new object[]
				{
					formScript.name,
					ex.Message,
					ex.StackTrace
				});
            }
        }
    }

    /// <summary>
    /// 创建窗体
    /// </summary>
    /// <param name="formPrefabPath">窗体路径</param>
    /// <param name="useFormPool">是否从窗体对象池去</param>
    /// <returns>对象</returns>
    private GameObject CreateForm(string formPrefabPath, bool useFormPool)
    {
        GameObject gameObject = null;
        if (useFormPool)
        {
            for (int i = 0; i < m_pooledForms.Count; ++i)
            {
                if (string.Equals(formPrefabPath, m_pooledForms[i].formPath, StringComparison.OrdinalIgnoreCase))
                {
                    m_pooledForms[i].Appear(enFormHideFlag.HideByCustom, true);
                    gameObject = m_pooledForms[i].gameObject;
                    m_pooledForms.RemoveAt(i);
                    break;
                }
            }
        }
        if (gameObject == null)
        {
            GameObject gameObject2 = (GameObject)Singleton<ResourceManager>.GetInstance().GetResource(formPrefabPath, typeof(GameObject), enResourceType.UIForm, false, false).content;
            if (gameObject2 == null)
            {
                return null;
            }
            gameObject = (GameObject)UnityEngine.Object.Instantiate(gameObject2) ;
        }
        if (gameObject != null)
        {
            UIFormScript component = gameObject.GetComponent<UIFormScript>();
            if (component != null)
            {
                component.useFormPool = useFormPool;
            }
        }
        return gameObject;
    }

    /// <summary>
    /// 根据窗体路径得到窗体名字
    /// </summary>
    /// <param name="formPath">路径</param>
    /// <returns>名字</returns>
    private string GetFormName(string formPath)
    {
        return FileManager.EraseExtension(FileManager.GetFullName(formPath));
    }

    /// <summary>
    //根据窗体路径从窗体表中获取该窗体（该窗体必须是没有关闭的窗体）
    /// </summary>
    /// <param name="inFormPath">窗体路径</param>
    /// <returns>窗体对象</returns>
    private UIFormScript GetUnCloseForm(string inFormPath)
    {
        for (int i = 0; i < m_forms.Count; ++i)
        {
            if (m_forms[i].formPath.Equals(inFormPath) && !m_forms[i].IsClosed())
            {
                return m_forms[i];
            }
        }
        return null;
    }


    private void ProcessFormList(bool sort, bool handleInputAndHide)
    {
        if (sort)
        {
            m_forms.Sort();
            for (int i = 0; i < m_forms.Count; ++i)
            {
                int formOpenOrder = GetFormOpenOrder(m_forms[i].GetSequence());
                m_forms[i].SetDisplayOrder(formOpenOrder);
            }
        }
        if (handleInputAndHide)
        {
            UpdateFormHided();
            UpdateFormRaycaster();     
        }
        if (onFormSorted != null)
        {
            onFormSorted(m_forms);
        }
    }

    private void UpdateFormHided()
    {
        bool flag = false;
        for (int i = m_forms.Count - 1; i >= 0; i--)
        {
            if (flag)
            {
                m_forms[i].Hide(enFormHideFlag.HideByOtherForm, false);
            }
            else
            {
                m_forms[i].Appear(enFormHideFlag.HideByOtherForm, false);
            }
            if (!flag && !m_forms[i].IsHided() && m_forms[i].hideUnderForms)
            {
                flag = true;
            }
        }
    }

    private void UpdateFormRaycaster()
    {
        bool flag = true;
        for (int i = m_forms.Count - 1; i >= 0; i--)
        {
            if (!m_forms[i].disableInput && !m_forms[i].IsHided())
            {
                GraphicRaycaster graphicRaycaster = m_forms[i].GetGraphicRaycaster();
                if (graphicRaycaster != null)
                {
                    graphicRaycaster.enabled = flag;
                }
                if (m_forms[i].isModal && flag)
                {
                    flag = false;
                }
            }
        }
    }

    /// <summary>
    /// 清空窗体池
    /// </summary>
    public void ClearFormPool()
    {
        for (int i = 0; i < m_pooledForms.Count; ++i)
        {
            UnityEngine.Object.Destroy(m_pooledForms[i].gameObject);
        }
        m_pooledForms.Clear();
    }

    /// <summary>
    ///从窗体序列表中获取窗体序列索引
    /// </summary>
    /// <param name="formSequence">窗体序列</param>
    /// <returns>窗体索引</returns>
    public int GetFormOpenOrder(int formSequence)
    {
        int num = m_existFormSequences.IndexOf(formSequence);
        return (num < 0) ? 0 : (num + 1);
    }

    /// <summary>
    /// 将窗体序列（类似窗体编号）添加至窗体序列表
    /// </summary>
    /// <param name="formSequence">窗体序列</param>
    public void AddToExistFormSequenceList(int formSequence)
    {
        if (m_existFormSequences != null)
        {
            m_existFormSequences.Add(formSequence);
        }
    }

    /// <summary>
    /// 将窗体序列（类似窗体编号）从窗体序列表中移除
    /// </summary>
    /// <param name="formSequence">窗体序列</param>
    public void RemoveFromExistFormSequenceList(int formSequence)
    {
        if (m_existFormSequences != null)
        {
            m_existFormSequences.Remove(formSequence);
        }
    }

    /// <summary>
    /// 是否有窗体打开
    /// </summary>
    /// <returns></returns>
    public bool HasForm()
    {
        return m_forms.Count > 0;
    }

    /// <summary>
    /// 依据窗体路径从窗体列表中获取窗体
    /// </summary>
    /// <param name="formPath">窗体路径</param>
    /// <returns></returns>
    public UIFormScript GetForm(string formPath)
    {
        for (int i = 0; i < m_forms.Count; i++)
        {
            if (m_forms[i].formPath.Equals(formPath) && !m_forms[i].IsNeedClose() && !m_forms[i].IsClosed())
            {
                return m_forms[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 依据窗体序列从窗体列表中获取窗体
    /// </summary>
    /// <param name="formSequence">窗体序列</param>
    /// <returns></returns>
    public UIFormScript GetForm(int formSequence)
    {
        for (int i = 0; i < m_forms.Count; i++)
        {
            if (m_forms[i].GetSequence() == formSequence && !m_forms[i].IsNeedClose() && !m_forms[i].IsClosed())
            {
                return m_forms[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 禁止输入
    /// </summary>
    public void DisableInput()
    {
        if (m_uiInputEventSystem != null)
        {
            m_uiInputEventSystem.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 激活输入
    /// </summary>
    public void EnableInput()
    {
        if (m_uiInputEventSystem != null)
        {
            m_uiInputEventSystem.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 获取顶部窗体
    /// </summary>
    /// <returns></returns>
    public UIFormScript GetTopForm()
    {
        UIFormScript uiFormScript = null;
        for (int i = 0; i < m_forms.Count; i++)
        {
            if (!(m_forms[i] == null))
            {
                if (uiFormScript == null)
                {
                    uiFormScript = m_forms[i];
                }
                else if (m_forms[i].GetSortingOrder() > uiFormScript.GetSortingOrder())
                {
                    uiFormScript = m_forms[i];
                }
            }
        }
        return uiFormScript;
    }

    /// <summary>
    /// 获取窗体列表
    /// </summary>
    /// <returns>窗体列表</returns>
    public List<UIFormScript> GetForms()
    {
        return m_forms;
    }

    /// <summary>
    /// 获取事件系统
    /// </summary>
    /// <returns></returns>
    public EventSystem GetEventSystem()
    {
        return m_uiInputEventSystem;
    }

    public void OpenSendMsgAlert(int autoCloseTime = 5, enUIEventID timeUpEventId = enUIEventID.None)
    {
        UIEvent uiEvent = new UIEvent();
        uiEvent.eventID = enUIEventID.Common_SendMsgAlertOpen;
        uiEvent.eventParams = new stUIEventParams
		{
			tag = autoCloseTime,
			tag2 = (int)timeUpEventId
		};
        Singleton<UIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
    }

    public void OpenSendMsgAlert(string txtContent, int autoCloseTime = 10, enUIEventID timeUpEventId = enUIEventID.None)
    {
        UIEvent uiEvent = new UIEvent();
        uiEvent.eventID = enUIEventID.Common_SendMsgAlertOpen;
        uiEvent.eventParams = new stUIEventParams
        {
            tagStr = txtContent,
            tag = autoCloseTime,
            tag2 = (int)timeUpEventId
        };
        Singleton<UIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
    }

    public void CloseSendMsgAlert()
    {
        UIEvent uiEvent = new UIEvent();
        uiEvent.eventID = enUIEventID.Common_SendMsgAlertClose;
        Singleton<UIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
    }

    public void OpenMessageBox(string strContent, bool isContentLeftAlign = false)
    {
        OpenMessageBoxBase(strContent, false, enUIEventID.None, enUIEventID.None, default(stUIEventParams), isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
    }

    public void OpenMessageBox(string strContent, enUIEventID confirmID, bool isContentLeftAlign = false)
    {
        OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, default(stUIEventParams), isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
    }

    public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, bool isContentLeftAlign = false)
    {
        OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
    }

    public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, bool isContentLeftAlign = false)
    {
        OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr, string.Empty, string.Empty, 0, enUIEventID.None);
    }

    public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, string titleStr, bool isContentLeftAlign = false)
    {
        OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr, string.Empty, titleStr, 0, enUIEventID.None);
    }

    public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, bool isContentLeftAlign = false)
    {
        OpenMessageBoxBase(strContent, true, confirmID, cancelID, default(stUIEventParams), isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
    }

    public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false)
    {
        OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, 0, enUIEventID.None);
    }

    public void OpenMessageBoxWithCancelAndAutoClose(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false, int autoCloseTime = 0, enUIEventID timeUpID = enUIEventID.None)
    {
        OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, string.Empty, string.Empty, string.Empty, autoCloseTime, timeUpID);
    }

    public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, string confirmStr, string cancelStr, bool isContentLeftAlign = false)
    {
        OpenMessageBoxBase(strContent, true, confirmID, cancelID, default(stUIEventParams), isContentLeftAlign, confirmStr, cancelStr, string.Empty, 0, enUIEventID.None);
    }

    public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams param, string confirmStr, string cancelStr, bool isContentLeftAlign = false)
    {
        OpenMessageBoxBase(strContent, true, confirmID, cancelID, param, isContentLeftAlign, confirmStr, cancelStr, string.Empty, 0, enUIEventID.None);
    }

    private void OpenMessageBoxBase(string strContent, bool isHaveCancelBtn, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false, string confirmStr = "", string cancelStr = "", string titleStr = "", int autoCloseTime = 0, enUIEventID timeUpID = enUIEventID.None)
    {
        UIFormScript uiFormScript = Singleton<UIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_MessageBox.prefab", false, false);
        if (uiFormScript == null)
        {
            return;
        }
        GameObject gameObject = uiFormScript.gameObject;
        if (gameObject == null)
        {
            return;
        }
        if (confirmStr == string.Empty)
        { 
            confirmStr = Singleton<TextManager>.GetInstance().GetText("Common_Confirm");  
        }
        if (cancelStr == string.Empty)
        { 
            cancelStr = Singleton<TextManager>.GetInstance().GetText("Common_Cancel");       
        }
        if (titleStr == string.Empty)
        { 
            titleStr = Singleton<TextManager>.GetInstance().GetText("Common_MsgBox_Title");         
        }
        GameObject obj3 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Confirm").gameObject;
        obj3.GetComponentInChildren<Text>().text = confirmStr;
        GameObject obj4 = gameObject.transform.Find("Panel/Panel/btnGroup/Button_Cancel").gameObject;
        obj4.GetComponentInChildren<Text>().text = cancelStr;
        gameObject.transform.Find("Panel/Panel/title/Text").gameObject.GetComponentInChildren<Text>().text = titleStr;
        Text component = gameObject.transform.Find("Panel/Panel/Text").GetComponent<Text>();
        component.text = strContent;
        if (!isHaveCancelBtn)
        {
            obj4.SetActive(false);
        }
        else
        {
            obj4.SetActive(true);
        }
        UIEventScript script2 = obj3.GetComponent<UIEventScript>();
        UIEventScript script3 = obj4.GetComponent<UIEventScript>();
        script2.SetUIEvent(enUIEventType.Click, confirmID, par);
        script3.SetUIEvent(enUIEventType.Click, cancelID, par);
        if (isContentLeftAlign)
        {
            component.alignment = TextAnchor.MiddleLeft;
        }
        if (autoCloseTime != 0)
        {
            Transform transform = uiFormScript.transform.Find("closeTimer");
            if (transform != null)
            {
                UITimerScript script4 = transform.GetComponent<UITimerScript>();
                if (script4 != null)
                {
                    script4.SetTotalTime((float)autoCloseTime);
                    script4.StartTimer();
                    script4.eventIDs[1] = timeUpID;
                    script4.eventParamsArry[1] = par;
                }
            }
        }
        CloseSendMsgAlert();
    }


    public void CloseMessageBox()
    {
        Singleton<UIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_MessageBox.prefab");
    }
}
