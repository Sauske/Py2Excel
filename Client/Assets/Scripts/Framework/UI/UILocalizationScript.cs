using System;
using UnityEngine;
using UnityEngine.UI;

public class UILocalizationScript : UIComponent
{
    [HideInInspector]
    public string key;

    private Text m_text;

    public override void Initialize(UIFormScript formScript)
    {
        if (m_isInitialized)
        {
            return;
        }
        base.Initialize(formScript);
        m_text = gameObject.GetComponent<Text>();
        SetDisplay();
    }

    protected override void OnDestroy()
    {
        m_text = null;
        base.OnDestroy();
    }

    public void SetKey(string nkey)
    {
        key = nkey;
        SetDisplay();
    }

    public void SetDisplay()
    {
        if (m_text == null || string.IsNullOrEmpty(key) || !Singleton<TextManager>.GetInstance().IsTextLoaded())
        {
            return;
        }
        m_text.text = Singleton<TextManager>.GetInstance().GetText(key);
    }
}
