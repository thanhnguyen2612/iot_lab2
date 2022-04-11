using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ToggleSwitch : MonoBehaviour
{
    public Toggle toggle;
    public RectTransform toggleIndicator;
    public Image backgroundImage;
    public float tweenTime = 0.25f;

    private Color onColor;
    private Color offColor;
    private float onX;
    private float offX;

    void Awake()
    {
        offX = toggleIndicator.anchoredPosition.x;
        onX = -offX;
        onColor = new Color(0.12f, 1f, 0f, 1f);
        offColor = backgroundImage.color;
        toggle.onValueChanged.AddListener(OnSwitch);

        OnSwitch(toggle.isOn);
    }

    void OnSwitch(bool on) {
        ToggleColor(on);
        MoveIndicator(on);
    }

    private void ToggleColor(bool value) {
        backgroundImage.DOColor(value ? onColor : offColor, tweenTime);
    }

    private void MoveIndicator(bool value) {
        toggleIndicator.DOAnchorPosX(value ? onX : offX, tweenTime);
    }

    void OnDestroy() {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
