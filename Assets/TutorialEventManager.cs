﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventManager : MonoBehaviour{

    public delegate void currentTutorial();
    currentTutorial myTutorial;

    int Stages;
    [SerializeField] RepairableObject[] pipes;
    [SerializeField] Grid grid;
    

    [SerializeField] public float explosionRadius;
    [SerializeField] public LayerMask interactableLayerMask;
    [SerializeField] public int explosionDamage;

    [SerializeField] GameObject[] Doors;
    [SerializeField] List<Collider> doorCollider;
    [SerializeField] List<Animator> doorAnimator;


    [SerializeField] FlorpReceptorTutorial florpReceptor;

    [SerializeField] Collider[] damagedObjects;

    [Space]
    [Header("Dialogue settings")]
    public DialogueManager dialogueManager;
    public DialogueTrigger[] dialogueTriggers;

    void Start () {

        damagedObjects = new Collider[12];

        dialogueManager.trigger = dialogueTriggers[0];
        dialogueManager.StartDialogue(dialogueManager.trigger.dialogue);
        pipes = FindObjectsOfType<RepairableObject>();

        grid = Grid.instance;
        myTutorial = checkPipes;

        Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, damagedObjects, interactableLayerMask);
    
        for(int i = 0; i < damagedObjects.Length; i++)// (Collider damagedObject in damagedObjects)
        {
            damagedObjects[i].GetComponent<IDamageable<int>>().TakeDamage(explosionDamage);
            damagedObjects[i].GetComponent<IDamageable<int>>().TakeDamage(explosionDamage);
            //IDamageable<int> caughtObject = damagedObject[i].GetComponent<IDamageable<int>>();
            ////shipCurrenHealth -= explosionDamage;
            //if (caughtObject != null) caughtObject.TakeDamage(explosionDamage);
            //if (caughtObject != null) caughtObject.TakeDamage(explosionDamage);
        }

        //for(int i = 0; i < Doors.Length; i++)
        //{
        //    doorCollider[i] = Doors[i].GetComponent<Collider>();
        //    doorAnimator[i] = Doors[i].GetComponent<Animator>();
        //}
    }

    private void Update()
    {
        myTutorial();

    }

    void checkPipes()
    {
        int pipeMax = 0;
        int pipeCur = 0;
        foreach (RepairableObject pipe in pipes)
        {
            pipeMax += pipe.healthMax;
            pipeCur += pipe.health;
        }

        if(pipeMax == pipeCur)
        {
            dialogueManager.trigger = dialogueTriggers[1];
            dialogueManager.StartDialogue(dialogueManager.trigger.dialogue);
            doorAnimator[0].SetBool("IsOpen", true);
            doorCollider[0].enabled = false;
            myTutorial = checkFire;
            //Debug.Log("WORKS");
        }
    }

    void checkFire()
    {
        bool isAllChecked = true;
        for (int x = 0; x < grid.gridSizeX; x++)
        {
            for (int y = 0; y < grid.gridSizeX; y++)
            {
                if (!grid.grid[x, y].isFlamable)
                {
                    isAllChecked = grid.grid[x,y].isFlamable;
                }
            }
        }
        if(isAllChecked)
        {
            dialogueManager.trigger = dialogueTriggers[2];
            dialogueManager.StartDialogue(dialogueManager.trigger.dialogue);
            doorAnimator[1].SetBool("IsOpen", true);
            doorCollider[1].enabled = false;
            myTutorial = checkEngine;
        }
    }

    void checkEngine()
    {
        if(florpReceptor.isFilled > 3)
        {
            SceneFader.instance.FadeTo("WinScene");
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        Collider[] damagedObjects = Physics.OverlapSphere(transform.position, explosionRadius, interactableLayerMask);

        foreach (Collider damagedObject in damagedObjects)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(damagedObject.transform.position, new Vector3(0.8f, 0.8f, 0.8f));
        }

    }
}
