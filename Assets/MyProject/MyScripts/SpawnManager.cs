using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject border;
    public GameObject player;
    public GameObject powerup;
    public GameObject[] players;
    private float spawnDelay = 2;
    private float spawnInterval = 15.0f;

    public float zMin = 89.0f;
    public float zMax = 118.0f;
    public float xMin = 89.0f;
    public float xMax = 118.0f;

    private float FENCE_HEIGHT = 0.2f;
    private float PLAYER_HEIGHT = 0.2f;

    // Start is called before the first frame update
    void Start()
    {

        // spawn borders
        //east
        var vec = new Vector3(30f, FENCE_HEIGHT, 2f);
        // 30.07, 0.4248145, 2.070007
        var rotationVector = transform.rotation * Quaternion.Euler(0, 0, 0);
        Instantiate(border, transform.position + vec, rotationVector);

        //west
        vec = new Vector3(0f, FENCE_HEIGHT, 29.5f);
        rotationVector = transform.rotation * Quaternion.Euler(0, 180, 0);
        Instantiate(border, transform.position + vec, rotationVector);

        //north
        vec = new Vector3(29f, FENCE_HEIGHT, 30.5f);
        rotationVector = transform.rotation * Quaternion.Euler(0, -90, 0);
        Instantiate(border, transform.position + vec, rotationVector);

        // south
        vec = new Vector3(1.1f, FENCE_HEIGHT, 1f);
        rotationVector = transform.rotation * Quaternion.Euler(0, 90, 0);
        Instantiate(border, transform.position + vec, rotationVector);
       
        //spawn players
        players = new GameObject[2];
        rotationVector = transform.rotation * Quaternion.Euler(0, 0, 0);
        vec = new Vector3(15.27f, PLAYER_HEIGHT, 8.62f);
        players[0] = Instantiate(player, transform.position + vec, rotationVector);
        players[0].GetComponent<SimpleCharacterControl>().SetPlayerNumber(1);
        players[0].name = "PlayerNumber" + players[0].GetComponent<SimpleCharacterControl>().playerNum;
        var camera = Camera.main;
        camera.GetComponent<FollowPlayer>().player = players[0];

        rotationVector = transform.rotation * Quaternion.Euler(0, 180, 0);
        vec = new Vector3(15.27f, PLAYER_HEIGHT, 26f);
        players[1] = Instantiate(player, transform.position + vec, rotationVector);
        players[1].GetComponent<SimpleCharacterControl>().SetPlayerNumber(2);
        players[1].name = "PlayerNumber" + players[1].GetComponent<SimpleCharacterControl>().playerNum;
//        players[1] = null;

        InvokeRepeating("SpawnObjects", spawnDelay, spawnInterval);
    }

    void SpawnObjects()
    {
        // Set random spawn location and random object index
        
        float zLoc = Random.Range(zMin, zMax);
        float xLoc= Random.Range(xMin, xMax);
        Vector3 spawnLocation = new Vector3(xLoc, 0, zLoc);

        Instantiate(powerup, spawnLocation, powerup.transform.rotation);

        // If game is still active, spawn new object
        //if (!playerControllerScript.gameOver)
        //{
         //   Instantiate(objectPrefabs[index], spawnLocation, objectPrefabs[index].transform.rotation);
        //}

    }
}
