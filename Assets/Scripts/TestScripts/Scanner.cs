using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using UnityEngine.UI;
using System;
using System.Text;

public class Scanner : MonoBehaviour
{

    private MqttClient client;

    //t/AI/StockholmStudio/Camera1


    public String Url;
    public String SensorTopic = "studdetector";
    public String PositionTopic = "position";
    public float MovementThreshold = 0.01f;
    //public ImageViewer Target = null;
    public Vector3 position,normal;

    public class xyz
    {
        public float x, y, z;
        public float nx, ny, nz;
    };


    public string mqttMsg;

    public String SensorValues;

    // public String mqttServerAdress = "213.168.249.129";

    // Use this for initialization
    void Start()
    {
        // create client instance 
        client = new MqttClient(Url, 1883, false, null);

        // register to message received 
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);

        // subscribe to the topic mspPowerTopic with QoS 2 
        client.Subscribe(new string[] { SensorTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        position = transform.position;

    }


    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {

        //Debug.Log("Received: " + System.Text.Encoding.UTF8.GetString(e.Message)  );
        mqttMsg = System.Text.Encoding.UTF8.GetString(e.Message);
        //mqttTopic = System.Text.Encoding.UTF8.GetString (e.Topic);
        //		Debug.Log(mqttMsg);


        //  if (Target != null)
        //    Target.Show(e.Message);

        SensorValues = mqttMsg;



    }


    // Update is called once per frame
    void Update()
    {
       

        Vector3 newpos;
        Scanner.xyz xyzpos;

        xyzpos = new Scanner.xyz();

        newpos = transform.position;

        if (Vector3.Distance(newpos,position)<MovementThreshold)
            return;


        position = transform.position;
        normal = transform.forward;

        xyzpos.x = position.x;
        xyzpos.y = position.y;
        xyzpos.z = position.z;

        xyzpos.nx = normal.x;
        xyzpos.ny = normal.y;
        xyzpos.nz = normal.z;





        string json = JsonUtility.ToJson(xyzpos);

        client.Publish(PositionTopic, Encoding.UTF8.GetBytes(json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE  , false);

    }

    private void OnDestroy()
    {

        if (client != null)
            client.Disconnect();
    }
}