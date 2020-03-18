using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounds : MonoBehaviour
{

    public bool _bIsSelected = true;

    void OnDrawGizmos()
    {
        if (_bIsSelected)
            OnDrawGizmosSelected();
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1f);  //center sphere
        if (transform.GetComponent<Renderer>() != null)
            Gizmos.DrawWireCube(transform.position, transform.GetComponent<Renderer>().bounds.size);
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
