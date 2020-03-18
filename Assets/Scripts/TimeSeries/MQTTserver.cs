using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using UnityEngine.UI;
using System;
using System.Text;

public class MQTTserver : ServerObject
{
    public string Url="op-en.se";
    public int port=1883;
    public string clientId = Guid.NewGuid().ToString();

    private MqttClient client;
    bool initiated = false;

    public bool DebugPrint = false;



    // Start is called before the first frame update
    void Start()
    {

        if (!initiated)
            Connect();

        base.Start();
    }

    void Connect()
    {
        if (DebugPrint)
            print("Starting MQTT client");

        // create client instance 
        client = new MqttClient(Url, port, false, null);

        // register to message received 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        //string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);

        initiated = true;

        // subscribe to the topic mspPowerTopic with QoS 2 
        //client.Subscribe(new string[] { "test/signalA" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        //MQTTsubscribe("test/signalA");
    }


    // Update is called once per frame
    void Update()
    {
       
    }

    override public bool SubscribeTopic(string Topic)
    {

        if (!initiated)
            Connect();

        return MQTTsubscribe(Topic);
    }

    override public bool Publish(string topic, string payload)
    {
        //JSONObject json = new JSONObject("\"topic\":\"" + topic + "\"");



        //Dictionary<string, string> data = new Dictionary<string, string>();
        //data["topic"] = topic;
        //data["payload"] = payload;

        //Emit("publish", new JSONObject(data));

        return true;
    }

    public bool MQTTsubscribe(string topic)
    {
        // subscribe to the topic mspPowerTopic with QoS 2 
        client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        if (DebugPrint)
            print("Subscribing to topic: " + topic);


        return true;
    }



    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string mqttMsg,mqttTopic;

        

        mqttMsg = System.Text.Encoding.UTF8.GetString(e.Message);
        JSONObject json_payload = new JSONObject(mqttMsg);
        //mqttTopic = e.Topic;

        JSONObject json = new JSONObject(mqttMsg);

        json.AddField("topic", e.Topic);
        json.AddField("payload", mqttMsg);

        //print(mqttTopic);
        if (DebugPrint)
            print(json);

        UpdateAllTargets( "mqtt", json);

    }

    void OnApplicationQuit()
    {
        print("Stopping MQTT client");
        client.Disconnect();
    }
}


 