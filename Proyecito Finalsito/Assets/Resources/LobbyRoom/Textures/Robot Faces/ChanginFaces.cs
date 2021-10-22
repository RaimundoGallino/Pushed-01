using UnityEngine;

public class ChanginFaces : MonoBehaviour
{
    [SerializeField] public Texture happyTexture, susTexture, sadTexture, happyEmission, susEmission, sadEmission;
    Renderer m_Renderer;

    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
    }

    public void HoverPush()
    {
        m_Renderer.material.mainTexture = happyTexture;
        m_Renderer.material.SetTexture("_EmissionMap", happyEmission);
    }
    public void HoverSettings()
    {
        m_Renderer.material.mainTexture = susTexture;
        m_Renderer.material.SetTexture("_EmissionMap", susEmission);
    }
    public void HoverAbort()
    {
        m_Renderer.material.mainTexture = sadTexture;
        m_Renderer.material.SetTexture("_EmissionMap", sadEmission);
    }
}
