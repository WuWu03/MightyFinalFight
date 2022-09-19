using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Test2:MonoBehaviour,IPointerClickHandler
{
    public TextMeshProUGUI Text;

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(Text, pos, Camera.main); //--UI相机
        if (linkIndex > -1)
        {
            TMP_LinkInfo linkInfo = Text.textInfo.linkInfo[linkIndex];
            Debug.Log(linkInfo.GetLinkID());
            //Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}