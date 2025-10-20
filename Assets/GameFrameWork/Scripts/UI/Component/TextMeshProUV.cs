using TMPro;
using UnityEngine;

public class TextMeshProUV : MonoBehaviour
{
    public TextMeshProUGUI textUGUI;
    public TextMeshPro text2;

    [Range(0, 1)] public float faceDilate = 0f;
    [Range(0, 1)] public float outlineWidth = 0f;
    public Color32 outlineColor = Color.black;

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        if (textUGUI == null)
        {
            textUGUI = GetComponent<TextMeshProUGUI>();
        }

        if (textUGUI == null)
        {
            text2 = GetComponent<TextMeshPro>();
            Outline(text2);
            return;
        }
        
        Outline(textUGUI);
    }

    private void Outline(TMP_Text text)
    {
        text.fontSharedMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, faceDilate);
        text.outlineColor = outlineColor;
        text.outlineWidth = outlineWidth;
    }
    private void Start()
    {
        if (Application.isPlaying)
        {
            Refresh();
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Refresh();
    }
#endif
}
