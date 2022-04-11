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
    public Color onColor;
    public Color offColor;
    public float tweenTime = 0.25f;

    private float onX;
    private float offX;

    void Awake()
    {
        offX = toggleIndicator.anchoredPosition.x;
        onX = -offX;
        toggle.onValueChanged.AddListener(OnSwitch);

        if (toggle.isOn) OnSwitch(true);
    }

    void OnSwitch(bool on) {
        ToggleColor(on);
        MoveIndicator(on);
    }

    private void ToggleColor(bool value) {
        backgroundImage.color = value ? onColor : offColor;
    }

    private void MoveIndicator(bool value) {
        toggleIndicator.DOAnchorPosX(value ? onX : offX, tweenTime);
    }

    void OnDestroy() {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
