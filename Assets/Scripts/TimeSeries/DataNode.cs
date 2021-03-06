using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//
public class DataNode : SimulationObject {
	[System.Serializable]
	public class Period {
		public bool Enabled = false;
		public bool Relative = false;
		public double FromTime = double.NaN, ToTime = double.NaN;
		public bool Contains(double ts) {
			double now, delta;

			if(!Enabled)
				return true;

			if(!Relative) {
				if(FromTime > ts)
					return false;
				if(ToTime < ts)
					return false;

				return true;
			}

			now = GameTime.GetInstance().time;
			delta = now - ts;

			if(FromTime > delta)
				return false;
			if(ToTime < delta)
				return false;

			return true;

		}


	}

	[System.Serializable]
	public class Subscription {
		public string Topic = null;
		public Period DataPeriod = new Period();

		//[HideInInspector]
		public DataNode Source;
		public DataNode Target;

		public DataPoint LastTransmission = null;
		//		public bool UseTopicFilter = false;

		//		public bool UseTimeFilter = false;
		//		public double FromTime = double.NaN,ToTime=double.NaN;
		//		public bool RelativeTime=false;

		public bool Equals(Subscription other) {
			if(other == null) return false;
			return ((this.Target == other.Target) && (this.Source == other.Source) && (this.Topic == other.Topic));
		}

		public bool TimeDataUpdate(DataPoint Data) {

			if(!DataPeriod.Contains(Data.Timestamp))
				return false;

			Target.TimeDataUpdate(this, Data);

			return true;
		}

		public bool MatchesTopic(string topic2) {
			if(Topic == null && Topic == "")
				return true;

			if(Topic == topic2)
				return true;

			//TODO

			return false;
		}

	}

	[Header("Data node properties")]
	public string NodeName;
	[Space(10)]
	public List<string> Units = new List<string>();
	public List<string> Columns = new List<string>();
	[Space(10)]
	public List<Subscription> Sources = new List<Subscription>();
	public List<Subscription> Targets = new List<Subscription>();
	[Space(10)]

	public DataPoint LastData = null;

	//
	virtual public void Subscribe(Subscription Sub) {

		if(Sub.Target == null && Sub.Source == null)
			return;

		if(Sub.Target == this) {
			if(!Sources.Contains(Sub))
				Sources.Add(Sub);
		} else if(Sub.Source == this) {
			//Sub.Target.Register (Sub);
			if(!Targets.Contains(Sub))
				Targets.Add(Sub);

		}


	}

    public int GetColumnID(string name)
    {
        for (int i=0; i < Columns.Count; i++)
        {
            if (name == Columns[i])
                return i;
        }

        return -1;
    }

    public int AddColumnID(string name)
    {
        

        for (int i=0; i < Columns.Count; i++)
        {
            if (name == Columns[i])
                return i;
        }

        if (name == "time" || name == "Time")
        {
            return -1;
        }

        Columns.Add(name);

        return Columns.Count - 1;
    }


    //
    virtual public void Unsubscribe(Subscription Sub) {
		if (Sub.Target == this) {
			Sources.Remove (Sub);
			Sub.Source.Targets.Remove (Sub);
		} else {
			Targets.Remove (Sub);
			Sub.Target.Sources.Remove (Sub);
		}
	}

	//
	public new void Awake() {
		base.Awake ();
		if(NodeName == "")
			NodeName = this.gameObject.name;

		SyncSubscriptions();
	}

	public new void Start() {
		base.Start ();
	}

	//
	public void SyncSubscriptions() {
		foreach(Subscription Sub in Sources) {
			Sub.Target = this;
			if(Sub.Source != null)
				Sub.Source.Subscribe(Sub);
		}

		foreach(Subscription Sub in Targets) {
			Sub.Source = this;
			if(Sub.Target != null)
				Sub.Target.Subscribe(Sub);
		}



	}

	//
	virtual public void TimeDataUpdate(Subscription Sub, DataPoint data) {

		Subscribe(Sub);

        //if (NodeName == "AC&Ventilation") {
        //	print ("UPDATE: " + (data.Timestamp - Sub.LastTransmission.Timestamp) );
        //}

		Sub.LastTransmission = data;

		DataPoint point;

		point = SumSources(data.Timestamp);

		UpdateAllTargets(point);
	}

	virtual public void CreateCounterRateInterpolation(double ts, double timefactor) {
		DataPoint dp = LastData.Clone ();

		double deltaT = ts - dp.Timestamp;
		dp.Values [1] = dp.Values [1] + dp.Values [0] * deltaT / timefactor;
		dp.Timestamp = ts;

		UpdateAllTargets(dp);
	}

	//
	public DataPoint SumSources(double At) {

		DataPoint point = null;
		double LastTimeStamp;
		double Values;

		foreach(Subscription conn in Sources) {



			if(conn.Source == null)
				continue;
			if(conn.LastTransmission == null)
				continue;
			if(conn.LastTransmission.Timestamp > At)
				continue;

			if(point == null) {
				point = conn.LastTransmission.Clone();
				continue;
			}

			point = point.Add(conn.LastTransmission);
		}

		point.Timestamp = At;

		return point;
	}

	//
	virtual public void TimeDataUpdate(Subscription Con, DataPoint[] data) {
		print("Unhandled data!");
	}

	//
	virtual public void JsonUpdate(Subscription Con, JSONObject json) {
		//Convert to DataPoint or DataPoints and do TimeDataUpdate
		print("Unhandled data!");
	}

	//
	virtual public void UpdateAllTargets(DataPoint Data) {

		LastData = Data;

		//Debug.Log ("Update old");
		//Debug.Log (this.name);

		foreach(Subscription conn in Targets) {
			if(conn != null) {
				if(conn.Source == null)
					conn.Source = this;
				if (conn.Target == null) {
					print ("Node " + NodeName + " is missing a target pointer");
					continue;
				}

				conn.Target.TimeDataUpdate(conn, Data);
			}
		}

	}
}
