using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bBoxDrawer : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //plane.transform.position = new Vector3(0, 0.5f, 0);

        //Draw2d();
        //ConvertImageCoordinates();

        DrawBox(0.1f, 0.4f, 0.2f, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void Draw2d()
    {
        var line = gameObject.AddComponent<LineRenderer>();

        line.sortingLayerName = "OnTop";
        line.sortingOrder = 5;
        line.SetVertexCount(3);
        line.SetPosition(0, new Vector3(5f,0,5f));
        line.SetPosition(1, new Vector3(0, 0, 0));
        line.SetPosition(2, new Vector3(0, 30, 0));
        line.SetWidth(0.5f, 0.5f);
        line.useWorldSpace = false;
        //line.material = LineMaterial;

    }

    void DrawBox(float x,float y, float w, float h)
    {
        var line = gameObject.AddComponent<LineRenderer>();


        Vector2 pos = new Vector2();
        pos.x = x;
        pos.y = y;



        line.sortingLayerName = "OnTop";
        line.sortingOrder = 5;
        line.SetVertexCount(5);
        line.SetPosition(0, ConvertImageCoordinates(pos));
        line.SetPosition(4, ConvertImageCoordinates(pos));

        pos.x = x + w;
        line.SetPosition(1, ConvertImageCoordinates(pos));

        pos.y = y + h;
        line.SetPosition(2, ConvertImageCoordinates(pos));

        pos.x = x;
        line.SetPosition(3, ConvertImageCoordinates(pos));
        line.SetWidth(0.5f, 0.5f);
        line.useWorldSpace = false;


    }


    Vector3 ConvertImageCoordinates(Vector2 pos)
    {
        Vector3 new_pos = new Vector3();
        new_pos.x = pos.x * -10 + 5;
        new_pos.y = 0.1f;
        new_pos.z = pos.y * -10 + 5;

        return new_pos;
    }
}
