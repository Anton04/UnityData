using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using TMPro;

public class DataText : DataNode {

	public TextMesh textMesh;
    public TextMeshPro textMeshPro; 
    public Text text;
	public string Unit = "";
	public int decimals = 0;
	public double scale = 1;
    bool updated = false;


	//public string Subproperty = null;
	public int SubpropertyId = 0;


	// Use this for initialization
	void Start () {
		GameObject parentObject;
		parentObject = this.transform.root.gameObject;
		if (textMesh == null && text == null && textMeshPro == null)
		    textMesh = parentObject.GetComponent<TextMesh>();

		if (textMesh == null && text == null && textMeshPro == null)
			text = gameObject.GetComponent(typeof(Text)) as Text;

        if (textMesh == null && text == null && textMeshPro == null)
            textMeshPro = parentObject.GetComponent<TextMeshPro>();



    }
	
	// Update is called once per frame
	void Update () {

        if (!updated)
            return;

        updated = false;

        string newtext = "";

        if (LastData == null)
            return;

        if (LastData.Values == null)
            return;

        if (SubpropertyId >= LastData.Values.Length)
            return;

        //Debug.Log (data.Values.Length);
        //Debug.Log (SubpropertyId);
        if (!double.IsNaN(LastData.Values[SubpropertyId]))
        {

            newtext = Math.Round(LastData.Values[SubpropertyId] / scale, decimals).ToString() + " " + Unit;


            if (textMesh != null)
                textMesh.text = newtext;
            if (text != null)
                text.text = newtext;
            if (textMeshPro != null)
                textMeshPro.text = newtext;
            return;
        }

        if (LastData.Texts[SubpropertyId] != null)
        {
            if (textMesh != null)
                textMesh.text = LastData.Texts[SubpropertyId];
            if (text != null)
                text.text = LastData.Texts[SubpropertyId];
            if (textMeshPro != null)
                textMeshPro.text = LastData.Texts[SubpropertyId]; 
        }
    }

	//override public void JsonUpdate(Subscription Sub, JSONObject json) {

	//	if (Subproperty == null)
	//		textMesh.text = json.str;
	//	else {
	//		textMesh.text = json.GetField (Subproperty).str;
	//	}
	//}

	override public void TimeDataUpdate(Subscription Sub, DataPoint data) {

        base.TimeDataUpdate(Sub, data);

        updated = true;

		//if (NodeName == "Heating Energy")
		//	print ("Heating Energy data recived");

		

    

	}





}
