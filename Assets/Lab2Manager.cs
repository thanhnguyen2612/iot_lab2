using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;

public class LoginScreen : M2MqttUnityClient
{
    public Canvas InputScreen;
    public Canvas DataScreen;

    public InputField BrokerUrl;
    public InputField Username;
    public InputField Password;
    public Button ConnectButton;

    private string topic_to_subscribe = "/bkiot/1852740/status";
    private string topic_to_publish_led = "/bkiot/1852740/led";
    private string topic_to_publish_pump = "/bkiot/1852740/pump";
    private List<string> eventMessages = new List<string>();
    public string msg_received_from_topic = "";


    protected override void Start()
    {
        this.ConnectionSucceeded += delegate { SceneManager.LoadScene("MainScreen"); };
        BrokerUrl.text = "mqttserver.tk";
        Username.text = "bkiot";
        Password.text = "12345678";
        ConnectButton.onClick.AddListener(ConnectServer);
    }

    protected override void Update()
    {
        OnTab();
        if (eventMessages.Count > 0)
        {
            foreach (string msg in eventMessages)
            {
                ProcessMessage(msg);
            }
            eventMessages.Clear();
        }
        base.Update();
    }

    protected override void SubscribeTopics()
    {
        if (topic_to_subscribe != "")
        {
            client.Subscribe(new string[] { topic_to_subscribe }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }
    }

    protected override void UnsubscribeTopics()
    {
        client.Unsubscribe(new string[] { topic_to_subscribe });
    }

    protected override void OnConnectionFailed(string errorMessage)
    {
        AddUiMessage("CONNECTION FAILED! " + errorMessage);
    }

    protected override void OnDisconnected()
    {
        AddUiMessage("Disconnected.");
    }

    protected override void OnConnectionLost()
    {
        AddUiMessage("CONNECTION LOST!");
    }

    protected override void DecodeMessage(string topic, byte[] message)
    {
        string msg = System.Text.Encoding.UTF8.GetString(message);
        msg_received_from_topic = msg;
        ProcessMessage(msg);
        StoreMessage(msg);
    }

    private void UpdateBeforeConnect()
    {
        this.brokerAddress = BrokerUrl.text;
        this.brokerPort = 1883;
        this.mqttUserName = Username.text;
        this.mqttPassword = Password.text;
    }

    private void ConnectServer()
    {
        UpdateBeforeConnect();

        if (this.brokerAddress == "")
        {
            print("Please enter something");
            return;
        }
        this.Connect();
    }

    private void AddUiMessage(string msg)
    {
        Debug.Log(msg);
    }

    private void StoreMessage(string eventMsg)
    {
        eventMessages.Add(eventMsg);
    }

    private void ProcessMessage(string msg)
    {
        Debug.Log("Received: " + msg);
    }
    
    private void OnTab()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (BrokerUrl.isFocused) Username.Select();
            if (Username.isFocused) Password.Select();
            if (Password.isFocused) BrokerUrl.Select();
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}
