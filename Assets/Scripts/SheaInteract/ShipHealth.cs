﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipHealth : MonoBehaviour {

    public static ShipHealth instance;

    public delegate void DamageAction();
    public static event DamageAction onDamagedAction;

    [Header("Event System")]
    [SerializeField] float timeBetweenNEvents;

    // TODO: Have pipes in scene add to a ship integrity value 
    [Header("Ship Statistics")]
    public int shipCurrenHealth;
    public int shipMaxHealth;

    [Header("Ship Blast Attributes")]
    [SerializeField] GameObject blastEffectPrefab;
    [SerializeField] float explosionRadius;
    [SerializeField] int explosionDamage;
    [SerializeField] LayerMask interactableLayerMask;
    [Space]
    [SerializeField] Vector3[] possibleAttackPositions;

    Vector3 attackLocation;
    Vector3 lastHitLocaton;

    [Header("UI Elements")]
    public Image healthBar;
    public TextMeshProUGUI healthText;
    public GameObject loseGameScreen;




    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        StartCoroutine("eventSystem");
        AdjustUI();
    }

    IEnumerator eventSystem()
    {
        while(true)
        {
            yield return new WaitForSeconds(timeBetweenNEvents);
            StartCoroutine("shipBlast");
        }
    }

    IEnumerator shipBlast()
    {
        if(attackLocation != null)
        {
            while (attackLocation == lastHitLocaton)
            {
                attackLocation = possibleAttackPositions[Random.Range(0, possibleAttackPositions.Length)];
            }
        }
        else
        {
            attackLocation = possibleAttackPositions[Random.Range(0, possibleAttackPositions.Length)];
        }

        lastHitLocaton = attackLocation;

        GameObject newBlast = Instantiate(blastEffectPrefab, attackLocation, Quaternion.identity);

        Collider[] damagedObjects = Physics.OverlapSphere(attackLocation, explosionRadius, interactableLayerMask);

        foreach(Collider damagedObject in damagedObjects)
        {
            IDamageable<int> caughtObject = damagedObject.GetComponent<IDamageable<int>>();

            if(caughtObject != null) caughtObject.TakeDamage(explosionDamage);
        }

        shipCurrenHealth -= explosionDamage;
        AdjustUI();

        if (shipCurrenHealth <= 0)
        {
            LoseGame();
        }

        yield return new WaitForSeconds(0.5f);

        Destroy(newBlast);

        yield return null;
    }

    void AdjustUI()
    {
        //Debug.Log(shipCurrenHealth / shipMaxHealth);
        healthBar.fillAmount =(float)shipCurrenHealth / shipMaxHealth;
        healthText.text = shipCurrenHealth.ToString();
    }

    void LoseGame()
    {
        // TODO: Make UI prettier and animate
        loseGameScreen.SetActive(true);
        //Time.timeScale = Mathf.Lerp(1f, 0.2f, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        foreach (Vector3 attackPosition in possibleAttackPositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPosition, explosionRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPosition, 0.5f);

            Collider[] damagedObjects = Physics.OverlapSphere(attackPosition, explosionRadius, interactableLayerMask);

            foreach (Collider damagedObject in damagedObjects)
            {

                //if (Gizmos.color == Color.red) { Gizmos.color = Color.red; } else { Gizmos.color = Color.blue; }
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(damagedObject.transform.position, new Vector3(0.8f, 0.8f, 0.8f));
               // MeshRenderer caughtObject = damagedObject.GetComponent<MeshRenderer>();
               // caughtObject.material.color = Color.red;
            }

        }
    }
}
