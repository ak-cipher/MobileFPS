using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MobileFPSGameManager : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;

    void Start()
    {
        if(playerPrefab!=null)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                float randomPoint = Random.Range(-10, 10);
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, 0f, randomPoint), Quaternion.identity);
            }
        }
        
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
