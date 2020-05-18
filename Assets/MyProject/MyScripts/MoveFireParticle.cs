using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFireParticle : MonoBehaviour
{
    private int speed = 10;
    private float zMin;// = 89.0f;
    private float zMax;// = 118.0f;
    private float xMin;// = 89.0f;
    private float xMax;// = 118.0f;

    // Start is called before the first frame update
    void Start()
    {
        zMin = GameObject.Find("Spawner").GetComponent<SpawnManager>().zMin;
        zMax = GameObject.Find("Spawner").GetComponent<SpawnManager>().zMax;
        xMin = GameObject.Find("Spawner").GetComponent<SpawnManager>().xMin;
        xMax = GameObject.Find("Spawner").GetComponent<SpawnManager>().xMax;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.up * speed * Time.deltaTime, Space.World);

        if(transform.position.x < xMin - 5 ||
           transform.position.x > xMax + 5 ||
           transform.position.z < zMin - 5 ||
           transform.position.z > zMax + 5)
        {
            Destroy(gameObject);
        }
    }
}
