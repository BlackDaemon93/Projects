using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Adjustments/Brightness")]
public class Brightness : MonoBehaviour {

    /// Oferă o proprietate a shader-ului care este stabilită în inspector
    /// și un material instanțiat de shader
    public Shader shaderDerp;
    Material m_Material;

    [Range(0f, 2f)]
    public float brightness = 1f;

    void Start() {
        // Dezactivează dacă nu suportă efectele grafice
        if (!SystemInfo.supportsImageEffects) {
            enabled = false;
            return;
        }

        // Dezactivează efectele grafice
        // daca placa grafică a utilizatorului nu le suportă
        if (!shaderDerp || !shaderDerp.isSupported)
            enabled = false;
    }


    Material material {
        get {
            if (m_Material == null) {
                m_Material = new Material(shaderDerp);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
        }
    }


    void OnDisable() {
        if (m_Material) {
            DestroyImmediate(m_Material);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        material.SetFloat("_Brightness", brightness);
        Graphics.Blit(source, destination, material);
    }
}
