using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingStaticFunctions : MonoBehaviour
{
    private Vector3 M = new Vector3(0, 0, 0);
    
    private void Start()
    {
        Debug.Log(M);
        // ClassName.FunctionName
        Debug.Log(MyHelperScripts.MyStaticFunction("Lasse"));

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.name = "Cube for testing static classes";
        MyMonoBehaviourHelperScripts.MoveTransformUp(go.transform);
    }
}
