using System;
using UnityEngine;

public class ParticleScaler : MonoBehaviour, IPooledMonoBehaviour
{
    public float particleScale = 1f;

    public bool alsoScaleGameobject = true;
    
    [HideInInspector]
    public bool scriptGenerated;

    private bool m_gotten;

    private float m_prevScale = 1f;


    public void OnCreate()
    {
    }

    public void OnGet()
    {
        if (m_gotten)
        {
            return;
        }
        m_gotten = true;
        m_prevScale = particleScale;
        if (scriptGenerated && particleScale != 1f)
        {
            m_prevScale = 1f;
            CheckAndApplyScale();
        }
    }

    public void OnRecycle()
    {
        m_gotten = false;
    }

    private void Start()
    {
        OnGet();
    }

    public void CheckAndApplyScale()
    {
        if (m_prevScale != particleScale && particleScale > 0f)
        {
            if (alsoScaleGameobject)
            {
                transform.localScale = new Vector3(particleScale, particleScale, particleScale);
            }
            float scaleFactor = particleScale / m_prevScale;
            ScaleLegacySystems(scaleFactor);
            ScaleShurikenSystems(scaleFactor);
            ScaleTrailRenderers(scaleFactor);
            m_prevScale = particleScale;
        }
    }

    private void Update()
    {
    }

    private void ScaleShurikenSystems(float scaleFactor)
    {
        ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>(true);
        ParticleSystem[] array = componentsInChildren;
        for (int i = 0; i < array.Length; i++)
        {
            ParticleSystem particleSystem = array[i];
            particleSystem.startSpeed *= scaleFactor;
            particleSystem.startSize *= scaleFactor;
            particleSystem.gravityModifier *= scaleFactor;
        }
    }

    private void ScaleLegacySystems(float scaleFactor)
    {
        ParticleEmitter[] componentsInChildren = GetComponentsInChildren<ParticleEmitter>(true);
        ParticleAnimator[] componentsInChildren2 = GetComponentsInChildren<ParticleAnimator>(true);
        ParticleEmitter[] array = componentsInChildren;
        for (int i = 0; i < array.Length; i++)
        {
            ParticleEmitter particleEmitter = array[i];
            particleEmitter.minSize *= scaleFactor;
            particleEmitter.maxSize *= scaleFactor;
            particleEmitter.worldVelocity *= scaleFactor;
            particleEmitter.localVelocity *= scaleFactor;
            particleEmitter.rndVelocity *= scaleFactor;
        }
        ParticleAnimator[] array2 = componentsInChildren2;
        for (int j = 0; j < array2.Length; j++)
        {
            ParticleAnimator particleAnimator = array2[j];
            particleAnimator.force *= scaleFactor;
            particleAnimator.rndForce *= scaleFactor;
        }
    }

    private void ScaleTrailRenderers(float scaleFactor)
    {
        TrailRenderer[] componentsInChildren = GetComponentsInChildren<TrailRenderer>(true);
        TrailRenderer[] array = componentsInChildren;
        for (int i = 0; i < array.Length; i++)
        {
            TrailRenderer trailRenderer = array[i];
            trailRenderer.startWidth *= scaleFactor;
            trailRenderer.endWidth *= scaleFactor;
        }
    }
}
