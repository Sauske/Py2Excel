using System;
using UnityEngine.UI;

public class CUIToggleListElementScript : UIListElementScript
{
    private Toggle m_toggle;

    public override void Initialize(UIFormScript formScript)
    {
        if (m_isInitialized)
        {
            return;
        }
        base.Initialize(formScript);
        m_toggle = GetComponentInChildren<Toggle>(base.gameObject);
        if (m_toggle != null)
        {
            m_toggle.interactable = false;
        }
    }

    protected override void OnDestroy()
    {
        m_toggle = null;
        base.OnDestroy();
    }

    public override void ChangeDisplay(bool selected)
    {
        base.ChangeDisplay(selected);
        if (m_toggle != null)
        {
            m_toggle.isOn = selected;
        }
    }
}
