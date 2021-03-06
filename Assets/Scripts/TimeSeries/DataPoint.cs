using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[Serializable]
public class DataEvent  {
	public double Timestamp;
}

public class CompareEvent : IComparer<DataEvent>
{
	static IComparer<DataEvent> comparer = new CompareEvent();

	public int Compare(DataEvent x, DataEvent y)
	{
		if (x == y)    return 0;
		if (x == null) return -1;
		if (y == null) return 1;
		if (x.Timestamp > y.Timestamp)
			return 1;
		if (x.Timestamp < y.Timestamp)
			return -1;

		return 0;
	}
}


[Serializable] 
public class DataPoint:DataEvent  {
	
	public double[] Values;
	public string[] Texts;
	public BasicDataSeries origin = null;

	public DataPoint Clone() {
		DataPoint clone = new DataPoint();

		if (Values != null)
			clone.Values = (double[])Values.Clone ();

		if (Texts != null)
			clone.Texts = (string[])Texts.Clone ();
		
		clone.Timestamp = Timestamp;
		return clone;
	}



	public DataPoint Add(DataPoint augend) {
		DataPoint clone;
		int first,second,largest;
		double firstval,secondval;

		if (augend == null)
			return this;

		if (augend.Values == null)
			return this;

		if (Values == null)
			return augend;
		
		first = Values.Length;
		second = augend.Values.Length;

		largest = first > second ? first : second;

		clone = new DataPoint();
		clone.Values = new double[largest];

		for (int i = 0; i < largest; i++) {

			if (first <= i)
				firstval = 0.0;
			else
				firstval = Values [i];

			if (firstval == null)
				firstval = 0.0;
			
			if  (second <= i)
				secondval = 0.0;
			else
				secondval = augend.Values [i];

			if (secondval == null)
				secondval = 0.0;

			clone.Values [i] = firstval + secondval;
		}

		return clone;

	}

	public DataPoint Div(DataPoint augend) {
		DataPoint clone;
		int first,second,largest;
		double firstval,secondval;

		if (augend == null)
			return null;

		if (augend.Values == null)
			return null;

		if (Values == null)
			return null;

		first = Values.Length;
		second = augend.Values.Length;

		largest = first > second ? first : second;

		clone = new DataPoint();
		clone.Values = new double[largest];

		for (int i = 0; i < largest; i++) {

			if (first <= i)
				firstval = 0.0;
			else
				firstval = Values [i];

			if (firstval == null)
				firstval = 0.0;

			if  (second <= i)
				secondval = 0.0;
			else
				secondval = augend.Values [i];

			if (secondval == null)
				secondval = 0.0;

			clone.Values [i] = firstval / secondval;
		}

		return clone;
	}

}

[Serializable]
public class BasicDataSeries
{
	public List<DataPoint> Data = new List<DataPoint>();
	public int index = 0;


	public DataPoint GetCurrent() {
		return Data [index];
	}

	public DataPoint GetPrevious() {
		if (index < 1)
			return null;

		return Data [index-1];
	}


	public bool AtEnd() {
		return !(index < Data.Count);
	}

	public bool Next() {
		index++;
		return AtEnd ();
	}

	public DataPoint GetDataPointAt(double TimeStamp) {
		for (int i = 1; i < Data.Count; i++) {
			if (Data [i].Timestamp > TimeStamp && Data [i-1].Timestamp <= TimeStamp)
				return Data [i - 1];
		}

		return null;
	}

	public double CurrentTime() {

		if (!(index < Data.Count))
			return Double.NaN;

		return Data[index].Timestamp;
	}

	public double NextTime() {

		if (!(index < Data.Count - 1))
			return Double.NaN;

		return Data[index+1].Timestamp;
	}

    public DataPoint Operation(int type)
    {
        DataPoint Res = null,FirstValid = null;
        int nValues = 0; 

        //Get the first valid point as well as the width
        foreach (DataPoint data in Data) {
            if (data == null)
                continue;

            //if (FirstValid == null)
            //    FirstValid = data;

            if (data.Values.Length > nValues)
                nValues = data.Values.Length;

        }

        if (nValues == 0)
            return null;

        //Sum = FirstValid.Clone();
        Res = new DataPoint();
        Res.Values = new double[nValues];

        
        foreach (DataPoint data in Data)
        {
            if (data == null)
                continue;
            if (type == 0)
                Res = Res.Add(data);
            else if (type == 1)
                Res = Res.Div(data);
        }

        return Res;

    }

    public DataPoint Sum()
    {
        return Operation(0);
    }

    public DataPoint Div()
    {
        return Operation(1);
    }
    




    //	public void MergeStarcase(DataPoints Other) {
    //		DataPoints leading = null, nonleading = null, tmp;
    //		DataPoint point;
    //
    //		// Do we need to align the two dataseries?
    //		if ( Other.CurrentTime() == this.CurrentTime() ) {
    //			//No
    //			leading = null;
    //			nonleading = null;
    //		} else {
    //			//Set current to the one with the first point
    //			if (Other.CurrentTime() < this.CurrentTime() ) {
    //				leading = Other;
    //				nonleading = this;
    //			} else {
    //				leading = this;
    //				nonleading = Other;
    //			}
    //
    //
    //		}
    //
    //		//Continue until we run out of data. 
    //		while ( !this.AtEnd() || !Other.AtEnd() ) {
    //			//If missalinged find next point
    //			if (leading != null) {
    //				if (leading.NextTime () > nonleading.CurrentTime ()) {
    //					tmp = leading;
    //					leading = nonleading;
    //					nonleading = tmp;	
    //				} else {
    //					leading.Next ();
    //
    //				}
    //			} else {
    //
    //			}
    //
    //		}
    //
    //	}

}

[Serializable]
public class BasicDataSeriesCollection {
	public List<BasicDataSeries> Collection = new List<BasicDataSeries>();

	int index = -1;
	double At = Double.NegativeInfinity;


	//Returns the dataseries which with the pointer to the earlies timestamp
	public BasicDataSeries GetNextPoint() {

		double earliest = Double.PositiveInfinity;
		BasicDataSeries result = null;

		foreach (BasicDataSeries serie in Collection) {
			if (serie.CurrentTime() < earliest && serie.CurrentTime() > At) {
				earliest = serie.CurrentTime();
				result = serie;
			}
				
		}

		At = earliest;

		return result;
	}

	public BasicDataSeries GetNextPoints() {
		
		BasicDataSeries HasNextPoint;
		double TimeStamp;

		//Get the next point.
		HasNextPoint = GetNextPoint ();

		if (HasNextPoint == null)
			return null;

		//Save the timestamp.
		TimeStamp = HasNextPoint.CurrentTime ();
		//Move that series forward.
		HasNextPoint.Next ();

		DataPoint dp;

		BasicDataSeries result = new BasicDataSeries ();

		foreach (BasicDataSeries serie in Collection) {
			dp = serie.GetDataPointAt (TimeStamp);
			result.Data.Add (dp);
		}

		return result;
	}


	public bool AllAtEnd() {
		foreach (BasicDataSeries serie in Collection) {
			if (!serie.AtEnd ())
				return false;
		}

		return true;
	}

	//Staircase assumes that a value is valid until we get a new value. 
	//In this case missalighed data not inside the range of the other series will be discared. 
	public BasicDataSeries GetStaircaseSumOfSeries() {
		BasicDataSeries points;
		BasicDataSeries result = new BasicDataSeries ();

		points = GetNextPoints ();

		while (points != null) {


			DataPoint Sum = new DataPoint ();

			foreach (DataPoint point in points.Data) {

				//if (point == null) {
				//	Sum = null;
				//	break;
				//}

				Sum = Sum.Add (point);

			}

			if (Sum != null && Sum.Values != null) {

				Sum.Timestamp = At;
				result.Data.Add (Sum);
			}

			points = GetNextPoints ();
		}

		return result;
	}



	public BasicDataSeries GetStaircaseDivOfSeries() {
		BasicDataSeries points;
		BasicDataSeries result = new BasicDataSeries ();
        int i;

        points = GetNextPoints();

        while (points != null) {

            if (points.Data[0] == null)
            {
                points = GetNextPoints();
                continue;
            }

            DataPoint Sum = points.Data[0].Clone();
            

			for (i=1;i<points.Data.Count;i++) {


				Sum = Sum.Div (points.Data[i]);

			}

			if (Sum != null && Sum.Values != null) {

				Sum.Timestamp = At;
				result.Data.Add (Sum);
			}

			points = GetNextPoints ();
		}

		return result;
	}


}	

