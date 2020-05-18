using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleCharacterControl : MonoBehaviour {

    private enum ControlMode
    {
        Tank,
        Direct
    }

    public enum PlayMode
    {
        Singleplayer,
        MultiKey,
        Multiplayer
    }

    [SerializeField] private float m_moveSpeed = 7;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;
    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;
    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

    /* my */
    public GameObject[] trophies;
    public GameObject objToChange;
    private float shiftMove = 7;
    public int playerNum;
    private int mana = 30;
    private int hitPoints = 30;
    private bool isDisabled = false;
    private bool changeFlag = false;
    public PlayMode playMode = PlayMode.MultiKey;

    
    // CONSTS
    public GameObject tree;
    public GameObject bush;
    public GameObject fireParticle;
    private float upKeepResources = 2.0f;
    private float upKeepResourcesInterval = 5.0f;
    private int FIRE_COST = 5;
    private int SHIFT_COST = 3;
    private int CHANGE_COST = 12;
    private int MANA_CAP = 30;
    private int HIT_POINTS_CAP = 30;
    private int DISABLE_TIME = 5;
    private int RAISE_MANA = 10;
    private int RAISE_HIT_POINTS = 2;
    string playerNumber = "PlayerNumber";


    // key schemes

    // player 1
    private KeyCode firePlayer1 = KeyCode.F;
    private KeyCode changePlayer1 = KeyCode.C;
    private KeyCode shiftPlayer1 = KeyCode.Alpha1;
    private KeyCode forwardPlayer1 = KeyCode.W;
    private KeyCode backwardPlayer1 = KeyCode.S;
    private KeyCode leftPlayer1 = KeyCode.A;
    private KeyCode rightPlayer1 = KeyCode.D;
    private KeyCode walkPlayer1 = KeyCode.LeftShift;

    // player 2
    private KeyCode firePlayer2 = KeyCode.RightShift;
    private KeyCode changePlayer2 = KeyCode.Keypad3;
    private KeyCode shiftPlayer2 = KeyCode.Keypad1;
    private KeyCode forwardPlayer2 = KeyCode.UpArrow;
    private KeyCode backwardPlayer2 = KeyCode.DownArrow;
    private KeyCode leftPlayer2 = KeyCode.LeftArrow;
    private KeyCode rightPlayer2 = KeyCode.RightArrow;
    private KeyCode walkPlayer2 = KeyCode.Keypad0;



    /* end my */

    public void SetPlayerNumber(int number) { playerNum = number; }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("entered trigger - tag: " + other.gameObject.tag);

        //FireParticle
        if (other.gameObject.CompareTag("FireParticle"))
        {
            hitPoints = hitPoints - 10;
            if(hitPoints <= 0)
            {
                isDisabled = true;
                StartCoroutine("Enable");
            }
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Powerup"))
        {
            hitPoints += 10;
            mana += 10;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Tree") ||
            other.gameObject.CompareTag("Bush"))
        {
            changeFlag = true;
            objToChange = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tree") ||
            other.gameObject.CompareTag("Bush"))
        {
            changeFlag = false;
            objToChange = null;
        }
    }

    IEnumerator Enable()
    {
        m_animator.enabled = false;
        yield return new WaitForSeconds(DISABLE_TIME);
        isDisabled = false;
        hitPoints = HIT_POINTS_CAP / 2;
        m_animator.enabled = true;

    }

    private void OnCollisionEnter(Collision collision)
    {

        



        /* original code /
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider)) {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
        /* end original code */
    }
    /* 
    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        } else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }
    /**/
    private void Start()
    {
        InvokeRepeating("ResourcesUp", upKeepResources, upKeepResourcesInterval);
        trophies = new GameObject[3];

        var rotationVector = transform.rotation * Quaternion.Euler(0, 90, 0);
        //Debug.Log(rotationVector.eulerAngles.x + " " + rotationVector.eulerAngles.y + " " + rotationVector.eulerAngles.z);
        if(playerNum == 1)
        {
            var vec = new Vector3(9f, 0, -4);
            trophies[0] = Instantiate(tree, transform.position + vec, rotationVector);
            trophies[0].GetComponent<TagsScript>().Add(playerNumber + playerNum);

            vec = new Vector3(0f, 0, -6f);
            trophies[1] = Instantiate(tree, transform.position + vec, rotationVector);
            trophies[1].GetComponent<TagsScript>().Add(playerNumber + playerNum);

            vec = new Vector3(-9, 0, -4);
            trophies[2] = Instantiate(tree, transform.position + vec, rotationVector);
            trophies[2].GetComponent<TagsScript>().Add(playerNumber + playerNum);
        }
        else
        {
            var vec = new Vector3(9f, 0, 1);
            trophies[0] = Instantiate(tree, transform.position + vec, rotationVector);
            trophies[0].GetComponent<TagsScript>().Add(playerNumber + playerNum);

            vec = new Vector3(0f, 0, 3f);
            trophies[1] = Instantiate(tree, transform.position + vec, rotationVector);
            trophies[1].GetComponent<TagsScript>().Add(playerNumber + playerNum);

            vec = new Vector3(-9, 0, 1);
            trophies[2] = Instantiate(tree, transform.position + vec, rotationVector);
            trophies[2].GetComponent<TagsScript>().Add(playerNumber + playerNum);
        }
        
    }

    void Update () {
        
        if(isDisabled) { return; }

        if(mana >= FIRE_COST)
        {
            if (playerNum == 1 && Input.GetKeyDown(firePlayer1))
            {
                ConjureFire();
            }
            else if (playerNum == 2 && playMode.Equals(PlayMode.MultiKey) && Input.GetKeyDown(firePlayer2))
            {
                ConjureFire();
            }
        }

        if (mana >= CHANGE_COST && changeFlag && objToChange != null)
        {
            if(playerNum == 1 && Input.GetKeyDown(changePlayer1))
            {
                Change();
            }
            else if(playerNum == 2 && playMode.Equals(PlayMode.MultiKey) && Input.GetKeyDown(changePlayer2))
            {
                Change();
            }
        }

        // do not touch, working methods
        m_animator.SetBool("Grounded", m_isGrounded);

        switch(m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            case ControlMode.Tank:
                TankUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }

        m_wasGrounded = m_isGrounded;
    }

    void Change()
    {
        SpawnManager spawner = GameObject.Find("Spawner").GetComponent<SpawnManager>();

        if (objToChange.CompareTag("Tree"))
        {
            // check its not of the player
            for(int i = 0; i < spawner.players.Length; i++)
            {
                SimpleCharacterControl trophiesOfPlayerToFind = spawner.players[i].GetComponent<SimpleCharacterControl>();
                if (trophiesOfPlayerToFind.playerNum == playerNum)
                {
                    Debug.Log("it is of the same player ");
                    continue;
                }
                for (int j = 0; j < trophiesOfPlayerToFind.trophies.Length; j++)
                {
                    Debug.Log("onj:" + objToChange.GetInstanceID() + " other:" + trophiesOfPlayerToFind.trophies[j].GetInstanceID());
                    if (ReferenceEquals(objToChange, trophiesOfPlayerToFind.trophies[j]))
                    {
                        // change
                        Debug.Log("changed tree");
                        mana = mana - CHANGE_COST;
                        trophiesOfPlayerToFind.trophies[j] = Instantiate(bush, objToChange.transform.position, bush.transform.rotation);
                        trophiesOfPlayerToFind.trophies[j].GetComponent<TagsScript>().Add(trophiesOfPlayerToFind.playerNumber + trophiesOfPlayerToFind.playerNum);
                        Destroy(objToChange);
                        objToChange = null;
                        changeFlag = false;
                        

                        int numOfBushes = 0;
                        foreach(GameObject bush in trophiesOfPlayerToFind.trophies) { if(bush.tag.Equals("Bush")) { numOfBushes++; } }

                        //TODO: push for game over
                        if (numOfBushes == trophiesOfPlayerToFind.trophies.Length) { Debug.Log("Game Over"); }
                        return;
                    }
                }
            }
        }
        else if (objToChange.CompareTag("Bush"))
        {
            // find the bush of the player

            for(int i = 0; i < trophies.Length; i++)
            {
                if (ReferenceEquals(objToChange, trophies[i]))
                {
                    // change
                    //Debug.Log("changed bush");
                    mana = mana - CHANGE_COST;
                    trophies[i] = Instantiate(tree, objToChange.transform.position, tree.transform.rotation);
                    trophies[i].GetComponent<TagsScript>().Add(playerNumber + playerNum);
                    Destroy(objToChange);
                    objToChange = null;
                    changeFlag = false;
                    return;
                }
            }
        }
        //Debug.Log("didnt return");
    }

    void ConjureFire()
    {
        mana = mana - FIRE_COST;
        var rotationVector = transform.rotation * Quaternion.Euler(0, 90, 90);
        var transformVector = new Vector3(0, 1, 0) + transform.forward * 2;
        Instantiate(fireParticle, transform.position + transformVector, rotationVector);
    }

    void ResourcesUp()
    {
        mana = Mathf.Min(mana + RAISE_MANA, MANA_CAP);
        hitPoints = Mathf.Min(hitPoints + RAISE_HIT_POINTS, HIT_POINTS_CAP);
    }

    // movement methods, do not touch
    private void TankUpdate()
    {
        float v = 0;// = Input.GetAxis("Vertical");
        float h = 0;// = Input.GetAxis("Horizontal");

        if(playerNum == 1)
        {
            if(Input.GetKey(forwardPlayer1))
                v = 1;
            if (Input.GetKey(backwardPlayer1))
                v = -1;
            //v = Input.GetAxis("Vertical");
            //Debug.Log(v);
            if (Input.GetKey(rightPlayer1))
                h = 1;
            if (Input.GetKey(leftPlayer1))
                h = -1;
        }
        else if (playMode.Equals(PlayMode.MultiKey) && playerNum == 2)
        {
            if (Input.GetKey(forwardPlayer2))
                v = 1;
            if (Input.GetKey(backwardPlayer2))
                v = -1;
            //v = Input.GetAxis("Vertical");
            //Debug.Log(v);
            if (Input.GetKey(rightPlayer2))
                h = 1;
            if (Input.GetKey(leftPlayer2))
                h = -1;
        }

        bool walk = false;
        if (playerNum == 1)
        {
            walk = Input.GetKey(walkPlayer1);
        }
        // should be deleted for multiplayer
        else if (playMode.Equals(PlayMode.MultiKey) && playerNum == 2)
        {
            walk = Input.GetKey(walkPlayer2);
        }
        //bool walk = Input.GetKey(KeyCode.LeftShift);

        if (v < 0) {
            if (walk) { v *= m_backwardsWalkScale; }
            else { v *= m_backwardRunScale; }
        } else if(walk)
        {
            v *= m_walkScale;
        }

        bool shift = false;
        if (playerNum == 1)
        {
            shift = Input.GetKey(shiftPlayer1);
        }
        // should be deleted for multiplayer
        else if (playMode.Equals(PlayMode.MultiKey) && playerNum == 2)
        {
            shift = Input.GetKey(shiftPlayer2);
        }

        if (shift && mana >= SHIFT_COST)
        {
            m_currentV = shiftMove;
            transform.position += transform.forward * shiftMove * m_moveSpeed * Time.deltaTime;
            mana = mana - SHIFT_COST;
        }
        else
        {
            m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
            m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

            transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
            transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);
        }

        m_animator.SetFloat("MoveSpeed", m_currentV);
        JumpingAndLanding();
    }

    private void DirectUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Transform camera = Camera.main.transform;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if(direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }

        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && Input.GetKey(KeyCode.Space))
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }
}
