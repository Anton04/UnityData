using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using UnityEngine.UI;
using System;


public class mqttTest : MonoBehaviour {

	private MqttClient client;

    //t/AI/StockholmStudio/Camera1

    public Text mspPowerText;
	public Text mspPercentText;
	public String Topic = "test/AI/StockholmStudio/Camera1";
    public ImageViewer Target=null;

	private String mqttMsg;
	private String mqttTopic;
	private String mspPower;
	private String mspPercent;

    // public String mqttServerAdress = "213.168.249.129";

    // Use this for initialization
    void Start () {
		// create client instance 
		client = new MqttClient("op-en.se",1883 , false , null ); 
		
		// register to message received 
		client.MqttMsgPublishReceived += client_MqttMsgPublishReceived; 
		
		string clientId = Guid.NewGuid().ToString(); 
		client.Connect(clientId);

        // subscribe to the topic mspPowerTopic with QoS 2 
		client.Subscribe(new string[] { Topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }); 

	}


	void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e) { 

		//Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message)  );
		mqttMsg = System.Text.Encoding.UTF8.GetString (e.Message);
		//mqttTopic = System.Text.Encoding.UTF8.GetString (e.Topic);
//		Debug.Log(mqttMsg);


        if (Target != null)
            Target.Show(e.Message);




    } 


	// Update is called once per frame
	void Update () {
// mspPowerText.text = mspPower + " W";
// mspPercentText.text = mspPercent + " %";
    }

    private void OnDestroy()
    {

        if (client != null)
            client.Disconnect();
    }
}