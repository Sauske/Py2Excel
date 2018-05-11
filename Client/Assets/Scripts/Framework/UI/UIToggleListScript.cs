using System;
using System.Collections.Generic;
using UnityEngine;


public class CUIToggleListScript : UIListScript
{
    public bool isMultiSelected;

    private int m_selected;

    private bool[] m_multiSelected;

    public override void Initialize(UIFormScript formScript)
    {
        if (m_isInitialized)
        {
            return;
        }
        if (isMultiSelected)
        {
            m_multiSelected = new bool[elementAmount];
            for (int i = 0; i < elementAmount; i++)
            {
                m_multiSelected[i] = false;
            }
        }
        else
        {
            m_selected = -1;
        }
        base.Initialize(formScript);
    }

    public override void SetElementAmount(int amount, List<Vector2> elementsSize)
    {
        if (isMultiSelected && (m_multiSelected == null || m_multiSelected.Length < amount))
        {
            bool[] array = new bool[amount];
            for (int i = 0; i < amount; i++)
            {
                if (m_multiSelected != null && i < m_multiSelected.Length)
                {
                    array[i] = m_multiSelected[i];
                }
                else
                {
                    array[i] = false;
                }
            }
            m_multiSelected = array;
        }
        base.SetElementAmount(amount, elementsSize);
    }

    public override void SelectElement(int index, bool isDispatchSelectedChangeEvent = true)
    {
        if (isMultiSelected)
        {
            bool flag = m_multiSelected[index];
            flag = !flag;
            m_multiSelected[index] = flag;
            UIListElementScript elemenet = base.GetElemenet(index);
            if (elemenet != null)
            {
                elemenet.ChangeDisplay(flag);
            }
            DispatchElementSelectChangedEvent();
        }
        else
        {
            if (index == m_selected)
            {
                if (alwaysDispatchSelectedChangeEvent)
                {
                    DispatchElementSelectChangedEvent();
                }
                return;
            }
            if (m_selected >= 0)
            {
                UIListElementScript elemenet2 = base.GetElemenet(m_selected);
                if (elemenet2 != null)
                {
                    elemenet2.ChangeDisplay(false);
                }
            }
            m_selected = index;
            if (m_selected >= 0)
            {
                UIListElementScript elemenet3 = base.GetElemenet(m_selected);
                if (elemenet3 != null)
                {
                    elemenet3.ChangeDisplay(true);
                }
            }
            DispatchElementSelectChangedEvent();
        }
    }

    public int GetSelected()
    {
        return m_selected;
    }

    public bool[] GetMultiSelected()
    {
        return m_multiSelected;
    }

    public void SetSelected(int selected)
    {
        m_selected = selected;
        for (int i = 0; i < m_elementScripts.Count; i++)
        {
            m_elementScripts[i].ChangeDisplay(IsSelectedIndex(m_elementScripts[i].m_index));
        }
    }

    public void SetMultiSelected(int index, bool selected)
    {
        if (index < 0 || index >= elementAmount)
        {
            return;
        }
        m_multiSelected[index] = selected;
        for (int i = 0; i < m_elementScripts.Count; i++)
        {
            m_elementScripts[i].ChangeDisplay(IsSelectedIndex(m_elementScripts[i].m_index));
        }
    }

    public override bool IsSelectedIndex(int index)
    {
        return (!isMultiSelected) ? (index == m_selected) : m_multiSelected[index];
    }
}
