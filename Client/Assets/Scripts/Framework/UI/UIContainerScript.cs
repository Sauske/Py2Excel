using System;
using System.Collections.Generic;
using UnityEngine;

public class UIContainerScript : UIComponent
{
    public int prepareElementAmount;

    private const int c_elementMaxAmount = 200;

    private GameObject m_elementTemplate;

    private string m_elementName;

    private int m_usedElementAmount;

    private GameObject[] m_usedElements = new GameObject[200];

    private List<GameObject> m_unusedElements = new List<GameObject>();

    public override void Initialize(UIFormScript formScript)
    {
        if (m_isInitialized)
        {
            return;
        }
        base.Initialize(formScript);
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject gameObject1 = gameObject.transform.GetChild(i).gameObject;
            if (m_elementTemplate == null)
            {
                m_elementTemplate = gameObject1;
                m_elementName = gameObject1.name;
                m_elementTemplate.name = m_elementName + "_Template";
                if (m_elementTemplate.activeSelf)
                {
                    m_elementTemplate.SetActive(false);
                }
            }
            gameObject1.SetActive(false);
        }
        if (prepareElementAmount > 0)
        {
            for (int j = 0; j < prepareElementAmount; j++)
            {
                GameObject gameObject2 = GameObject.Instantiate(m_elementTemplate);
                gameObject2.gameObject.name = m_elementName;
                base.InitializeComponent(gameObject2.gameObject);
                if (gameObject2.activeSelf)
                {
                    gameObject2.SetActive(false);
                }
                if (gameObject2.transform.parent != gameObject.transform)
                {
                    gameObject2.transform.SetParent(gameObject.transform, true);
                    gameObject2.transform.localScale = Vector3.one;
                }
                m_unusedElements.Add(gameObject2);
            }
        }
    }

    protected override void OnDestroy()
    {
        m_elementTemplate = null;
        m_usedElements = null;
        m_unusedElements.Clear();
        m_unusedElements = null;
        base.OnDestroy();
    }

    public int GetElement()
    {
        if (m_elementTemplate == null || m_usedElementAmount >= 200)
        {
            return -1;
        }
        GameObject gameObject;
        if (m_unusedElements.Count > 0)
        {
            gameObject = m_unusedElements[0];
            m_unusedElements.RemoveAt(0);
        }
        else
        {
            gameObject = GameObject.Instantiate(m_elementTemplate);
            gameObject.name = m_elementName;
            base.InitializeComponent(gameObject.gameObject);
        }
        gameObject.SetActive(true);
        for (int i = 0; i < 200; i++)
        {
            if (m_usedElements[i] == null)
            {
                m_usedElements[i] = gameObject;
                m_usedElementAmount++;
                return i;
            }
        }
        return -1;
    }

    public GameObject GetElement(int sequence)
    {
        if (sequence < 0 || sequence >= 200)
        {
            return null;
        }
        return (!(m_usedElements[sequence] == null)) ? m_usedElements[sequence].gameObject : null;
    }

    public void RecycleElement(int sequence)
    {
        if (m_elementTemplate == null || sequence < 0 || sequence >= 200)
        {
            return;
        }
        GameObject gameObject = m_usedElements[sequence];
        m_usedElements[sequence] = null;
        if (gameObject != null)
        {
            gameObject.SetActive(false);
            if (gameObject.transform.parent != gameObject.transform)
            {
                gameObject.transform.SetParent(gameObject.transform, true);
                gameObject.transform.localScale = Vector3.one;
            }
            m_unusedElements.Add(gameObject);
            m_usedElementAmount--;
        }
    }

    public void RecycleElement(GameObject elementObject)
    {
        if (m_elementTemplate == null || elementObject == null)
        {
            return;
        }
        for (int i = 0; i < 200; i++)
        {
            if (m_usedElements[i] == elementObject)
            {
                m_usedElements[i] = null;
                m_usedElementAmount--;
                break;
            }
        }
        elementObject.SetActive(false);
        if (elementObject.transform.parent != gameObject.transform)
        {
            elementObject.transform.SetParent(gameObject.transform, true);
            elementObject.transform.localScale = Vector3.one;
        }
        m_unusedElements.Add(elementObject);
    }

    public void RecycleAllElement()
    {
        if (m_elementTemplate == null || m_usedElementAmount <= 0)
        {
            return;
        }
        for (int i = 0; i < 200; i++)
        {
            if (m_usedElements[i] != null)
            {
                m_usedElements[i].SetActive(false);
                if (m_usedElements[i].transform.parent != gameObject.transform)
                {
                    m_usedElements[i].transform.SetParent(gameObject.transform, true);
                    m_usedElements[i].transform.localScale = Vector3.one;
                }
                m_unusedElements.Add(this.m_usedElements[i]);
                m_usedElements[i] = null;
                m_usedElementAmount--;
            }
        }
    }
}
