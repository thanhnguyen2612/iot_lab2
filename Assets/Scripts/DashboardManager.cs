using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Dashboard
{
    public class DashboardManager : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _loginLayer;
        [SerializeField]
        public InputField BrokerUrlInput;
        [SerializeField]
        public InputField UsernameInput;
        [SerializeField]
        public InputField PasswordInput;
        [SerializeField]
        public Text ConnectStatus;

        [SerializeField]
        private CanvasGroup _dataLayer;
        [SerializeField]
        public Text Temperature;
        [SerializeField]
        public Text Humidity;
        // [SerializeField]
        // public ToggleSwitch LedToggle;
        // [SerializeField]
        // public ToggleSwitch PumpToggle;

        private Tween twenFade;

        void Start() {
            BrokerUrlInput.text = "mqttserver.tk";
            UsernameInput.text = "bkiot";
            PasswordInput.text = "12345678";
        }

        public ControlData GetLedControlData()
        {
            ControlData data = new ControlData();
            data.device = "LED";
            // data.status = LedToggle.toggle.isOn ? "OFF" : "ON";
            // LedToggle.toggle.interactable = false;
            data.status = "ON";
            return data;
        }

        public ControlData GetPumpControlData()
        {
            return GetLedControlData();
            // ControlData data = new ControlData();
            // data.device = "PUMP";
            // data.status = PumpToggle.toggle.isOn ? "OFF" : "ON";
            // PumpToggle.toggle.interactable = false;
            // return data;
        }

        public void UpdateConnectStatus(string status) {
            StartCoroutine(_DisplayConnectionStatus(status));
        }

        public void UpdateStatusData(StatusData status_data)
        {
            Temperature.text = float.Parse(status_data.temperature) + "°C";
            Humidity.text = float.Parse(status_data.humidity) + "°%";
        }

        public void UpdateToggle(ControlData data)
        {
            Debug.Log("Update Toggle");
            // if (data.device == "LED")
            // {
            //     LedToggle.toggle.interactable = true;
            //     LedToggle.toggle.isOn = data.status == "ON" ? true : false;
            // }
            // if (data.device == "PUMP")
            // {
            //     PumpToggle.toggle.interactable = true;
            //     PumpToggle.toggle.isOn = data.status == "ON" ? true : false;
            // }
        }

        public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish)
        {
            if (twenFade != null)
            {
                twenFade.Kill(false);
            }

            twenFade = _canvas.DOFade(endValue, duration);
            twenFade.onComplete += onFinish;
        }

        public void FadeIn(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 1f, duration, () =>
            {
                _canvas.interactable = true;
                _canvas.blocksRaycasts = true;
            });
        }

        public void FadeOut(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 0f, duration, () =>
            {
                _canvas.interactable = false;
                _canvas.blocksRaycasts = false;
            });
        }

        IEnumerator _IESwitchLayer()
        {
            if (_loginLayer.interactable == true)
            {
                FadeOut(_loginLayer, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_dataLayer, 0.25f);
            }
            else
            {
                FadeOut(_dataLayer, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_loginLayer, 0.25f);
            }
        }

        IEnumerator _DisplayConnectionStatus(string status)
        {
            ConnectStatus.text = status;
            yield return new WaitForSeconds(2f);
            ConnectStatus.text = "";
        }

        public void SwitchLayer()
        {
            StartCoroutine(_IESwitchLayer());
        }
    }
}