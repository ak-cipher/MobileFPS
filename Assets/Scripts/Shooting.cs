using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;



public class Shooting : MonoBehaviourPunCallbacks
{
    //public delegate void Firing();

    public Camera fpsCamera;

    public GameObject hitEffectPrefab;

    public Animator animator;

    public FireButton fireButton;



    [Header("Health Stuff")]
    public float startHealth = 100f;
    public float health;
    public Image healthBar;

    public int killed;

    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;


        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            if (fireButton.isFiring)
            {

                Fire();
            }
        }
       
    }



    

    public void Fire()
    {

        RaycastHit hit;
        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if(Physics.Raycast(ray,out hit,100))
        {

            //Debug.Log(hit.collider.gameObject.name);

            photonView.RPC("CreateHitEffect", RpcTarget.All, hit.point);

            if( hit.collider.gameObject.CompareTag("Player")  && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                Debug.Log(hit.collider.gameObject.name);
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered,5f);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float damage,PhotonMessageInfo info)
    {
        health -= damage;
        Debug.Log(health);

        healthBar.fillAmount = health / startHealth;

        if(health <= 0f && health >-4)
        {
            Die();
            Debug.Log(info.Sender.NickName + " Killed " + info.photonView.Owner.NickName);
        }
    }


    [PunRPC]
    public void CreateHitEffect(Vector3 position)
    {
        GameObject hitEffect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffect, 0.5f);
    }


    [PunRPC]
    public void Die()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Here Now first time");
            killed += 1;
            animator.SetBool("IsDead", true);
            StartCoroutine(Respawn());
        }
    }


    IEnumerator Respawn()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        GameObject killCount = GameObject.Find("KillCount");

        killCount.GetComponent<Text>().text = "You have been killed " + killed + " times ";

        float respawnTime = 8.0f;

        while(respawnTime > 0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;

            transform.GetComponent<PlayerMovementController>().enabled = false;

            transform.GetComponent<Shooting>().enabled = false;
            respawnText.GetComponent<Text>().text = "You have been killed. respawning in " + respawnTime.ToString(".00");
        }

        animator.SetBool("IsDead", false);

        respawnText.GetComponent<Text>().text = "";
        killCount.GetComponent<Text>().text = "";

        int randomPosition = Random.Range(-20, 20);
        transform.position = new Vector3(randomPosition, 0, randomPosition);

        transform.GetComponent<PlayerMovementController>().enabled = true;
        transform.GetComponent<Shooting>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);

    }


    [PunRPC]
    public void RegainHealth()
    {

        health = startHealth;
        healthBar.fillAmount = health / startHealth;
    }
}
