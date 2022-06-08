
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;


public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] soldierModel_GameChildren;
    public GameObject[] FPS_Hands_GameChildren;

    public GameObject playerUI_Prefab;
    public PlayerMovementController playerMovementController;
    public GameObject FPScamera;

    public Animator animator;

    private Shooting shooting;


    private void Start()
    {
        playerMovementController = this.gameObject.GetComponent<PlayerMovementController>();
        animator = GetComponent<Animator>();
        shooting = GetComponent<Shooting>();

        //Activate FPS hands , Deactivate Soldier Model
        if (photonView.IsMine)
        {
            foreach(GameObject gameObject in FPS_Hands_GameChildren)
            {
                gameObject.SetActive(true);
            }

            foreach(GameObject gameObject in soldierModel_GameChildren)
            {
                gameObject.SetActive(false);
            }
            playerMovementController = this.gameObject.GetComponent<PlayerMovementController>();
            

            //Instantiate PlayerUI
            GameObject PlayerUIGameObject = Instantiate(playerUI_Prefab);

            
            playerMovementController.joystick = PlayerUIGameObject.transform.Find("FixedJoystick").GetComponent<FixedJoystick>();
            
            playerMovementController.fixedTouch = PlayerUIGameObject.transform.Find("TouchArea").GetComponent<FixedTouchField>();

            shooting.fireButton = PlayerUIGameObject.transform.Find("FireButton").GetComponent<FireButton>();

            //Camera
            FPScamera.SetActive(true);

            //Animator
            animator.SetBool("IsSoldier", false);

            shooting.fireButton.enabled = true;
        }

        
        //Activate Soldier Model ,Deactivate FPS Hands
        else
        {

            foreach(GameObject gameObject in FPS_Hands_GameChildren)
            {
                gameObject.SetActive(false);
            }

            foreach(GameObject gameObject in soldierModel_GameChildren)
            {
                gameObject.SetActive(true);
            }


            //disbaling player movement if the player is not local
            playerMovementController.enabled = false;

            
            FPScamera.SetActive(false);
            

            animator.SetBool("IsSoldier", true);

            shooting.fireButton.enabled = false;
        }
    }
}
