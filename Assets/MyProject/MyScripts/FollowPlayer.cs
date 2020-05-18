using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    private float yHeight = 10;
    private Vector3 offset;
    private Vector3 myrotation = new Vector3(90, 0, 0);


    // Start is called before the first frame update
    void Start()
    {
        transform.rotation.SetLookRotation(myrotation);
        yHeight = this.GetComponent<Camera>().orthographicSize;
        offset = new Vector3(0, yHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // this controls the zoom size.
        // GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + 1;
        
        transform.position = player.transform.position + offset;

    }
}
