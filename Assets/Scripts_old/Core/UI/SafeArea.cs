using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SafeArea : WagBehaviour
{
    RectTransform rectTransform => transform as RectTransform;
    [SerializeField] Canvas _uiCanvas;

    IEnumerator Start()
    {
        yield return null;
        yield return null; //Some devices might have bugs on area in start
        FitSafeArea();
    }

    void FitSafeArea()
    {
        var safeArea = Screen.safeArea;

        var heightRatio = safeArea.height / Screen.height;
        var widthRatio = safeArea.width / Screen.width;

        var newFullHeight = 1080f / heightRatio;
        var newFullWidth = newFullHeight * Screen.width / (float)Screen.height;
        var scaler = _uiCanvas.GetComponent<CanvasScaler>();


        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1080f);
        var newWidth = safeArea.width * 1080f / safeArea.height;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

        var myRect = rectTransform.rect;
        var heightOffsetRatio = safeArea.y / Screen.height;
        var heightOffset = heightOffsetRatio * newFullHeight;
        var widthOffsetRatio = safeArea.x / Screen.width;
        var widthOffset = widthOffsetRatio * newFullWidth;

        var xOffset = widthOffset + newWidth / 2 - newFullWidth / 2;
        var yOffset = heightOffset + 1080 / 2f - newFullHeight / 2;

        rectTransform.anchoredPosition = new Vector2(xOffset, yOffset);

        scaler.referenceResolution = new Vector2(newFullWidth, newFullHeight);
    }
}