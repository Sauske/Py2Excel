using System;
using UnityEngine;

public class UIParticleScript : UIComponent
{
    public string resPath = string.Empty;

    public bool isFixScaleToForm;

    public bool isFixScaleToParticleSystem;

    private Renderer[] m_renderers;

    private int m_rendererCount;

    private void LoadRes()
    {
        string text = this.resPath;
        if (!string.IsNullOrEmpty(text))
        {
            if (GameSettings.particleQuality == GameRenderQuality.Low)
            {
                text = string.Concat(new string[]
                {
                    UIUtility.s_Particle_Dir,
                    resPath,
                    "/",
                    resPath,
                    "_low.prefeb"
                });
            }
            else if (GameSettings.particleQuality == GameRenderQuality.Medium)
            {
                text = string.Concat(new string[]
                {
                    UIUtility.s_Particle_Dir,
                    resPath,
                    "/",
                    resPath,
                    "_mid.prefeb"
                });
            }
            else
            {
                text = string.Concat(new string[]
                {
                    UIUtility.s_Particle_Dir,
                    resPath,
                    "/",
                    resPath,
                    ".prefeb"
                });
            }
            GameObject asset = Singleton<ResourceManager>.GetInstance().GetResource(text, typeof(GameObject), enResourceType.UIPrefab, false, false).content as GameObject;
            if (asset != null && gameObject.transform.childCount == 0)
            {
                GameObject gameObject2 = UnityEngine.Object.Instantiate(asset) as GameObject;
                gameObject2.transform.SetParent(gameObject.transform);
                gameObject2.transform.localPosition = Vector3.zero;
                gameObject2.transform.localRotation = Quaternion.identity;
                gameObject2.transform.localScale = Vector3.one;
            }
        }
    }

    public void LoadRes(string resName)
    {
        if (!m_isInitialized)
        {
            return;
        }
        resPath = resName;
        LoadRes();
        InitializeRenderers();
        SetSortingOrder(belongedFormScript.GetSortingOrder());
        if (isFixScaleToForm)
        {
            ResetScale();
        }
        if (isFixScaleToParticleSystem)
        {
            ResetParticleScale();
        }
        if (belongedFormScript.IsHided())
        {
            Hide();
        }
    }

    public override void Initialize(UIFormScript formScript)
    {
        if (m_isInitialized)
        {
            return;
        }
        LoadRes();
        InitializeRenderers();
        base.Initialize(formScript);
        if (isFixScaleToForm)
        {
            ResetScale();
        }
        if (isFixScaleToParticleSystem)
        {
            ResetParticleScale();
        }
        if (belongedFormScript.IsHided())
        {
            Hide();
        }
    }

    protected override void OnDestroy()
    {
        m_renderers = null;
        base.OnDestroy();
    }

    public override void Hide()
    {
        base.Hide();
        UIUtility.SetGameObjectLayer(gameObject, 31);
    }

    public override void Appear()
    {
        base.Appear();
        UIUtility.SetGameObjectLayer(gameObject, 5);
    }

    public override void SetSortingOrder(int sortingOrder)
    {
        base.SetSortingOrder(sortingOrder);
        for (int i = 0; i < m_rendererCount; i++)
        {
            m_renderers[i].sortingOrder = sortingOrder;
        }
    }

    private void InitializeRenderers()
    {
        m_renderers = new Renderer[100];
        m_rendererCount = 0;
        UIUtility.GetComponentsInChildren<Renderer>(gameObject, m_renderers, ref m_rendererCount);
    }

    private void ResetScale()
    {
        float num = 1f / belongedFormScript.gameObject.transform.localScale.x;
        gameObject.transform.localScale = new Vector3(num, num, 0f);
    }

    private void ResetParticleScale()
    {
        if (belongedFormScript == null)
        {
            return;
        }
        float scale = 1f;
        RectTransform component = belongedFormScript.GetComponent<RectTransform>();
        if (belongedFormScript.canvasScaler.matchWidthOrHeight == 0f)
        {
            scale = component.rect.width / component.rect.height / (belongedFormScript.canvasScaler.referenceResolution.x / belongedFormScript.canvasScaler.referenceResolution.y);
        }
        else if (belongedFormScript.canvasScaler.matchWidthOrHeight == 1f)
        {
        }
        this.InitializeParticleScaler(base.gameObject, scale);
    }

    private void InitializeParticleScaler(GameObject gameObject, float scale)
    {
        ParticleScaler particleScaler = gameObject.GetComponent<ParticleScaler>();
        if (particleScaler == null)
        {
            particleScaler = gameObject.AddComponent<ParticleScaler>();
        }
        if (particleScaler.particleScale != scale)
        {
            particleScaler.particleScale = scale;
            particleScaler.alsoScaleGameobject = false;
            particleScaler.CheckAndApplyScale();
        }
    }
}
