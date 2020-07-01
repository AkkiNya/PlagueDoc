﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private Rigidbody rb;

    //Manual Movement System
    [Range(-15f, 15f)]
    public float moveleftright;
    [Range(-15f, 15f)]
    public float movefowardback;
    [Range(0f, 1f)]
    public float tiltinterval;
    [Range(0f, 20f)]
    public float tiltlimit;
    private float timer;

    public State state = State.WALKINGPAST;
    private NPCManager NPCM;
    private Actions Player;
    public float wanttobuysomething;
    int placeOnList;

    public enum State
    {
        WALKINGPAST,
        QUEUING,
        FLEEING,
        PANICKING,
        BEINGSERVED,
        LEAVING
    }

    void OnEnable()
    {
        state = State.WALKINGPAST;
        NPCM = GameObject.FindObjectOfType<NPCManager>();
        Player = GameObject.FindObjectOfType<Actions>();
        rb = GetComponent<Rigidbody>();
        wanttobuysomething = Random.Range(0f,10f);
        haveiqueuedalready = true;
    }
    public bool queue;
    public bool haveiqueuedalready;
    void Update()
    {
        // Tilt Timer resets after two movements
        timer += Time.deltaTime;
        if (timer > (tiltinterval * 2))
        {
            timer = 0f;
        }

        if (transform.position.x < Random.Range(9f, -5f) && (wanttobuysomething > 2f) && (haveiqueuedalready == true))
        {
            queue = true;
            state = State.QUEUING;
            haveiqueuedalready = false;

            movefowardback = 0f;
            moveleftright = 0f;

            if (NPCsInRange.Count == 0)
            {
                int AmIOnList = NPCM.CheckPlace(gameObject);
                if (AmIOnList == 1)
                {
                    state = State.QUEUING;
                }

                if (AmIOnList == 0)
                {
                    state = State.WALKINGPAST;
                }
            }
        }
        
    }

    //List of other objects in trigger coliders radius
    public List<GameObject> NPCsInRange = new List<GameObject>();
    public Collider other;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DANGEROUSNPC"))
        {
            NPCsInRange.Add(other.attachedRigidbody.gameObject);
            state = State.FLEEING;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DANGEROUSNPC"))
        {
            NPCsInRange.Remove(other.attachedRigidbody.gameObject);
          
        }
    }

    void FixedUpdate()
    {
        //Manual Control
        rb.AddForce(transform.right * moveleftright);
        rb.AddForce(transform.forward * movefowardback);

        //Tilt Physics
        Quaternion rotation = (transform.rotation);



        if (timer < tiltinterval)
        {
            transform.rotation = Quaternion.Euler(rotation[0], rotation[1] + 0f, rotation[2] + tiltlimit);
        }

        if (timer > tiltinterval)
        {
            transform.rotation = Quaternion.Euler(rotation[0], rotation[1] + 0f, rotation[2] - tiltlimit);
        }

        //personal bubble
        foreach (GameObject AI in NPCsInRange)
        {
            if (AI != this)
            {
                rb.AddForce((AI.gameObject.transform.position - transform.position).normalized * -4f);
            }
        }

        //AI States
        switch (state)
        {
            case State.WALKINGPAST:
                Walkingpast();
                gameObject.tag = "NPC";
                break;
            case State.QUEUING:
                Queuing();
                gameObject.tag = "NPC";
                break;
            case State.FLEEING:
                Fleeing();
                gameObject.tag = "NPC";
                break;
            case State.PANICKING:
                Panicking();
                gameObject.tag = "DANGEROUSNPC";
                break;
            case State.BEINGSERVED:
                Beingserved();
                gameObject.tag = "NPC";
                break;
            case State.LEAVING:
                Leaving();
                gameObject.tag = "NPC";
                break;

        }
    }

    private float timer2;
    private bool flipflop;
    void Walkingpast()
    {
        timer2 += Time.deltaTime;

        if (timer2 > 4f)
        {
            timer2 = 0f;
        }
        moveleftright = -2.5f;
        tiltlimit = 6f;
        if (timer2 < 2f)
        {
            if (flipflop == true)
            {
                movefowardback = Random.Range(0f, -10f);
            }
            flipflop = false;
        }

        moveleftright = -2.5f;
        if (timer2 > 2f)
        {
            if (flipflop == false)
            {
                movefowardback = Random.Range(0f, 10f);
                flipflop = true;
            }
        }
    }

    void Leaving()
    {
        tiltlimit = 6f;
        moveleftright = -4f;
        if (transform.position.z > -6)
        {
            movefowardback = -12f;
        }
        if (transform.position.z < -6)
        {
            movefowardback = 0f;
            state = State.WALKINGPAST;
        }
    }

    private GameObject queueposition;
    void Queuing()
    {
        tiltinterval = 0.52f;
        tiltlimit = 1.8f;

        if (queue == true)
        {
            NPCM.AddNPC(gameObject);
            queue = false;
        }
        placeOnList = NPCM.CheckPlace(gameObject);

        if (placeOnList == 0)
        {
            state = State.BEINGSERVED;
        }

        if (placeOnList == 1)
        {
            queueposition = GameObject.Find("Q1");
        }

        if (placeOnList == 2)
        {
            queueposition = GameObject.Find("Q2");
        }

        if (placeOnList == 3)
        {
            queueposition = GameObject.Find("Q3");
        }

        if (placeOnList == 4)
        {
            queueposition = GameObject.Find("Q4");
        }

        if (placeOnList == 5)
        {
            queueposition = GameObject.Find("Q5");
        }

        if (transform.position.x > queueposition.transform.position.x)
        {
            moveleftright = -queueSpeed;
        }

        if (transform.position.x < queueposition.transform.position.x)
        {
            moveleftright = queueSpeed;
        }

        if (transform.position.z < queueposition.transform.position.z)
        {
            movefowardback = queueSpeed;
        }

        if (transform.position.z > queueposition.transform.position.z)
        {
            movefowardback = -queueSpeed;
        }
    }

    void Fleeing()
    {

    }

    private float panicktimer;
    private bool panickbool;
    private bool startpanick;
    public float panickedrunspeed;
    void Panicking()
    {

        if (startpanick == false)
        {
            panicktimer = 6f;
            startpanick = true;
        }

        panicktimer += Time.deltaTime;
        if (panicktimer > 2f)
        {
            panicktimer = 0f;
            panickbool = true;
        }
        
       

        if  (panickbool == true)
        {
            float direction = Random.Range(0f, 10f);
            float direction2 = Random.Range(0f, 10f);
            
            if(direction < 5f)
            {
                movefowardback = panickedrunspeed;
            }

            if (direction > 5f)
            {
                movefowardback = -panickedrunspeed;
            }

            if (direction2 < 5f)
            {
                moveleftright = panickedrunspeed;
            }

            if (direction2 > 5f)
            {
                moveleftright = -panickedrunspeed;
            }
            panickbool = false;
        }
    }

    private GameObject serveposition;
    public float queueSpeed;
    public bool finishedserving;
    public int[] order;
    public int ordersize;
    public bool ordermade;
    public float dist;
    public float fails;
    public Renderer colourchange;
    void Beingserved()
    {
        tiltlimit = 3f;
        tiltinterval = 0.4f;
        serveposition = GameObject.Find("ServePos");
        if (transform.position.x > serveposition.transform.position.x)
        {
            moveleftright = -queueSpeed;
        }

        if (transform.position.x < serveposition.transform.position.x)
        {
            moveleftright = queueSpeed;
        }

        if (transform.position.z < serveposition.transform.position.z)
        {
            movefowardback = queueSpeed;
        }

        if (transform.position.z > serveposition.transform.position.z)
        {
            movefowardback = -queueSpeed;
        }

        if (finishedserving == true)
        {
            NPCM.RemoveNPC(gameObject);
            state = State.LEAVING;
        }
        dist = Vector3.Distance(serveposition.transform.position, transform.position);

        if ((dist < 1.5f) && (ordermade == false))
        {
            order = new int[ordersize];

            float randomizer = Random.Range(0f, 3f);
            if ((randomizer > 0f) && (randomizer <= 1f))
            {
                order[0] = 1;
            }
            if ((randomizer > 1f) && (randomizer <= 2f))
            {
                order[0] = 2;
            }
            if ((randomizer > 2f) && (randomizer <= 3f))
            {
                order[0] = 3;
            }

            randomizer = Random.Range(0f, 3f);
            if ((randomizer > 0f) && (randomizer <= 1f))
            {
                order[1] = 1;
            }
            if ((randomizer > 1f) && (randomizer <= 2f))
            {
                order[1] = 2;
            }
            if ((randomizer > 2f) && (randomizer <= 3f))
            {
                order[1] = 3;
            }
            randomizer = Random.Range(0f, 3f);
            if ((randomizer > 0f) && (randomizer <= 1f))
            {
                order[2] = 1;
            }
            if ((randomizer > 1f) && (randomizer <= 2f))
            {
                order[2] = 2;
            }
            if ((randomizer > 2f) && (randomizer <= 3f))
            {
                order[2] = 3;
            }
            ordermade = true;
            Player.RecieveOrder(order[0],order[1],order[2]);
        }

        fails = Player.drugfails;
        if(fails == 1)
        {
            colourchange = GetComponent<Renderer>();
            colourchange.material.SetColor("_Color",Color.green);
            finishedserving = true;
            Player.drugfails = 0;
        }
        if (fails > 1)
        {
            colourchange = GetComponent<Renderer>();
            colourchange.material.SetColor("_Color", Color.red);
            finishedserving = true;
            Player.drugfails = 0;
        }
        if (fails == 0f)
        {
            finishedserving = false;
            Player.drugfails = 0;
        }
    }

    public void RecieveDrug(int drugfails)
    {
        if (drugfails > 0)
        {
            finishedserving = true;
        }

        if (drugfails == 0)
        {
            finishedserving = true;
        }
    }
}