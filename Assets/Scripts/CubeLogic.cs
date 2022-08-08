using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeLogic : MonoBehaviour
{

    public Gameplay CAM;

    public int X;
    public int Z;
    public bool showing;

    private int x_dist;
    private int z_dist;

    private Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer> ();
        showing = true;
        Vector3 VEC = transform.position;
        X = (int)VEC.x;
        Z = (int)VEC.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if should be rendered?
        x_dist = (int)Mathf.Abs((float)CAM.X - X);
        z_dist = (int)Mathf.Abs((float)CAM.Z - Z);
        if (x_dist+z_dist > 8 && showing) {
            rend.enabled = false;
            showing = false;
        } else if (x_dist+z_dist <= 8 && !showing) {
            rend.enabled = true;
            showing = true;
        }
    }
}
