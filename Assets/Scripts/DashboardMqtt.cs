using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dashboard
{
    public enum Device { LED, PUMP }

    public class StatusData
    {
        public string temperature { get; set; }
        public string humidity { get; set; }
    }

    public class ControlData
    {
        public string device { get; set; }
        public string status { get; set; }

    }

    public class DashboardMqtt : M2MqttUnityClient
    {
        public DashboardManager dashboard_manager;
        private string _studentId = "1852740";
        private List<string> _topics = new List<string>();

        [SerializeField]
        private StatusData _statusData;
        [SerializeField]
        private ControlData _controlData;

        protected override void Start()
        {
            _topics.Add(string.Format("/bkiot/{0}/status", _studentId));
            _topics.Add(string.Format("/bkiot/{0}/led", _studentId));
            _topics.Add(string.Format("/bkiot/{0}/pump", _studentId));

            base.Start();
        }

	    public void ConnectServer()
        {
            this.brokerAddress = dashboard_manager.BrokerUrlInput.text;
            this.brokerPort = 1883;
            this.mqttUserName = dashboard_manager.UsernameInput.text;
            this.mqttPassword = dashboard_manager.PasswordInput.text;

            this.Connect();
        }

        public void CloseConnection() {
            Disconnect();
            dashboard_manager.UpdateConnectStatus("");
            dashboard_manager.SwitchLayer();
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            dashboard_manager.SwitchLayer();
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            dashboard_manager.UpdateConnectStatus("Connection Failed");
        }

        protected override void OnDisconnected()
        {
            dashboard_manager.UpdateConnectStatus("Disconnected!");
        }

        protected override void OnConnectionLost()
        {
            dashboard_manager.UpdateConnectStatus("Connection Lost");
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        protected override void SubscribeTopics()
        {
            foreach (string topic in _topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                    Debug.Log(string.Format("Subscribed topic: {0}", topic));
                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            foreach (string topic in _topics)
            {
                if (topic != "")
                {
                    client.Unsubscribe(new string[] { topic });
                }
            }

        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);

            Debug.Log("Received: " + msg);
            if (topic == _topics[0])
                ProcessMessageStatus(msg);
        }

        private void ProcessMessageStatus(string msg)
        {
            _statusData = JsonConvert.DeserializeObject<StatusData>(msg);
            _statusData.temperature = Math.Round(float.Parse(_statusData.temperature), 2).ToString();
            _statusData.humidity = Math.Round(float.Parse(_statusData.humidity), 2).ToString();
            dashboard_manager.UpdateStatusData(_statusData);
        }

        public void PublishLed()
        {
            ControlData data = dashboard_manager.GetLedControlData();
            string msg_control = JsonConvert.SerializeObject(data);
            client.Publish(_topics[1], System.Text.Encoding.UTF8.GetBytes(msg_control), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        public void PublishPump()
        {
            ControlData data = dashboard_manager.GetPumpControlData();
            string msg_control = JsonConvert.SerializeObject(data);
            client.Publish(_topics[2], System.Text.Encoding.UTF8.GetBytes(msg_control), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }
    }
}
