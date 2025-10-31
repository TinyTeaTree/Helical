using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CaptureLinkClicks : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TMP_Text _text;
    [SerializeField] Camera _cam;

    public event System.Action<string> OnLinkIdClicked;

    public void SetCam(Camera cam)
    {
        _cam = cam;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, Input.mousePosition, _cam);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = _text.textInfo.linkInfo[linkIndex];
            string linkId = linkInfo.GetLinkID();
            OnLinkIdClicked.Invoke(linkId);
        }
    }
}
