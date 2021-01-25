using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageScale : MonoBehaviour
{
    public bool A = false;
    public bool B = false;
    public bool C = false;
    public bool D = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnable()
    {
        BoundriesCheck.OnSurfaceDetected += ReSizeCube;
        BoundriesCheck.OnSurfaceNotDetected += ReSizeCubeSmall;
    }


    void OnDisable()
    {
        BoundriesCheck.OnSurfaceDetected -= ReSizeCube;
        BoundriesCheck.OnSurfaceNotDetected -= ReSizeCubeSmall;
    }

    private void Update()
    {
        if (transform.localScale.x > 0)
        {

            //ScaleMode x
            if (A.Equals(false) && B.Equals(false))
            {
                transform.localScale = transform.localScale + new Vector3(-0.01f, 0, 0);
            }
            if (C.Equals(false) && D.Equals(false))
            {
                transform.localScale = transform.localScale + new Vector3(-0.01f, 0, 0);
            }
        }

        if (transform.localScale.z > 0)
        {
            //scale z
            if (A.Equals(false) && D.Equals(false))
            {
                transform.localScale = transform.localScale + new Vector3(0, 0, -0.01f);
            }
            if (B.Equals(false) && C.Equals(false))
            {
                transform.localScale = transform.localScale + new Vector3(0, 0, -0.01f);
            }
        }
     
    }

    void ReSizeCube(string name)
    {
        Debug.Log("Did Hit with "+ name);
        switch (name)
        {
            case "A":
                A = true;
                break;
            case "B":
                B = true;
                break;
            case "C":
                C = true;
                break;
            case "D":
                D = true;
                break;

            default:
                break;
        }
    }

    void ReSizeCubeSmall(string name)
    {
        Debug.Log("Did Hit with " + name);
        switch (name)
        {
            case "A":
                A = false;
                break;
            case "B":
                B = false;
                break;
            case "C":
                C = false;
                break;
            case "D":
                D = false;
                break;

            default:
                break;
        }
    }
}
