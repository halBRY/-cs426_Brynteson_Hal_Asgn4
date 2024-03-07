using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Cinemachine;
using TMPro;

public class PlayerController : NetworkBehaviour
{   
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private AudioListener audioListener;

    [SerializeField]
    private Camera OverheadCamera;
    [SerializeField]
    private AudioListener OverheadAudioListener;

    [SerializeField]
    private GameObject cat;
    [SerializeField]
    private GameObject mouse;

    public int myId;

    public GameObject playerManager;
    public PlayerManager playerManagerScript;

    public GameObject myNetUI;
    public NetworkManagerUI networkUI;

    public bool isCat;

    // create a list of colors
    public List<Color> colors = new List<Color>();

    // getting the reference to the prefab
    [SerializeField]
    private GameObject spawnedPrefab;
    // save the instantiated prefab
    private GameObject instantiatedPrefab;

    private CharacterController controller;
    private PlayerInput playerInput;
    
    private Vector3 playerVelocity;
    
    private bool groundedPlayer;

    private float playerSpeed = 11.0f;
    private float jumpHeight = 3.0f;
    private float gravityValue = -9.81f;

    public bool canMove = true;

    private InputAction moveAction;
    private InputAction jumpAction;

    private Transform cameraTransform;

    [SerializeField]
    private NetworkObject networkObject;

    public CinemachineVirtualCamera cineCamera;

    public TMP_Text debugID;

    void Start()
    {
        playerManagerScript.AddPlayerServerRpc();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        //debugID.text = myId + " " + playerManagerScript.newPlayerId.Value;

        if(myId == 0 && isCat)
        {
            cameraTransform = OverheadCamera.transform;
            gameObject.tag = "Cat"; 
            CatModelSpawningServerRpc();
        }
        else
        {
            cameraTransform = camera.transform;
            gameObject.tag = "Mouse"; 
            MouseModelSpawningServerRpc();

        }

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        Cursor.lockState = CursorLockMode.Locked;
    }

    [ServerRpc]
    private void CatModelSpawningServerRpc()
    {
        CatModelSpawningClientRpc();
    }

    [ClientRpc]
    private void CatModelSpawningClientRpc()
    {
        GameObject myModel = Instantiate(cat, gameObject.transform);
    }

    [ServerRpc]
    private void MouseModelSpawningServerRpc()
    {
        MouseModelSpawningClientRpc();
    }

    [ClientRpc]
    private void MouseModelSpawningClientRpc()
    {
        GameObject myModel = Instantiate(mouse, gameObject.transform);
    }

    void Update()
    {
        if(IsOwner) 
        { 
            cineCamera.m_Priority = 10; 
        }

        if(!IsOwner) 
        { 
            cineCamera.m_Priority = 0; 
        }

        if (!IsOwner) return;

        if(myId > 0)
        {
            jumpHeight = 0.0f;
        }

        if(canMove)
        {
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            Vector2 input = moveAction.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x, 0, input.y);

            move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
            move.y = 0.0f;

            controller.Move(move * Time.deltaTime * playerSpeed);

            // Changes the height position of the player..
            if (jumpAction.triggered && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            // Rotate model towards camera look
            Quaternion playerRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, playerRotation, 5f * Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            Cursor.lockState = CursorLockMode.None;
            //canMove = false;
        }


        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    // this method is called when the object is spawned
    // we will change the color of the objects
    public override void OnNetworkSpawn()
    {
        //GetComponent<MeshRenderer>().material.color = colors[(int)OwnerClientId];

        // check if the player is the owner of the object
        if (!IsOwner) return;
        // if the player is the owner of the object
        // enable the camera and the audio listener
        playerManager = GameObject.FindWithTag("Manager");
        playerManagerScript = playerManager.GetComponent<PlayerManager>();

        myId = playerManagerScript.newPlayerId.Value;

        myNetUI = GameObject.FindWithTag("NetUI");
        networkUI = myNetUI.GetComponent<NetworkManagerUI>();

        isCat = networkUI.getIsCat();

        Debug.Log("My ID is " + myId);

        if(myId == 0 && isCat)
        {
            OverheadAudioListener.enabled = true;
            OverheadCamera.enabled = true;
        }
        else
        {
            audioListener.enabled = true;
            camera.enabled = true;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;

        Debug.Log("Collision Detected");
        Debug.Log(hitObject.tag);


        //Stun Mouse
        if(myId == 0 && collision.gameObject.tag == "Mouse")
        {  
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            otherPlayer.freezePlayer();
        }
            
    }

    public void freezePlayer()
    {
        canMove = false;
        StartCoroutine(wait(2));
        canMove = true;
    }

    IEnumerator wait(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
