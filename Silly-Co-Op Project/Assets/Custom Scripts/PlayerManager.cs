using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private static List<PlayerInput> Activeplayers = new List<PlayerInput>();

    [SerializeField]
    public List<Transform> _startingPoints;

    public static List<Transform> startingPoints = new List<Transform>();

    [SerializeField]
    private List<LayerMask> playerLayers = new List<LayerMask>();

    [SerializeField]
    private List<GameObject> playerPrefabs = new List<GameObject>();

    private static Queue<int> playerInactiveQueue = new Queue<int>();

    private static int playerCount = 1;
    private int lastMaterial = 0;
    private int maxPlayer;


    private PlayerInputManager playerInputManager;

    private void Start()
    {
        startingPoints = _startingPoints;
        maxPlayer = playerPrefabs.Count;
    }

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void SetNextPlayerPrefab()
    {

        if (playerInactiveQueue.Count <= 0 && Activeplayers.Count < maxPlayer)
        {
            playerInputManager.playerPrefab = playerPrefabs[playerCount]; // 1
        }
        if (playerInactiveQueue.Count > 0 && Activeplayers.Count < maxPlayer)
        {
            if (playerInactiveQueue.TryDequeue(out var PlayerNumber))
            {
                playerInputManager.playerPrefab = playerPrefabs[PlayerNumber];
            } 
        }
    }


     public static void PlayerDead(PlayerInput player, int number)
     {
        if (Activeplayers.Contains(player))
        {   
            Activeplayers.Remove(player);
            playerCount--;
            if(!playerInactiveQueue.Contains(number-1)) playerInactiveQueue.Enqueue(number-1);
        }
     }

     private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
    }

    private void Update()
    {
        /*
       if(players!=null && players.Count > 0)
            foreach(PlayerInput p in players)
                Debug.Log(p.transform.parent.name+" "+p.transform.parent.GetComponentInChildren<Camera>().cullingMask);*/
    }

    public void AddPlayer(PlayerInput player)
    {
        Activeplayers.Add(player);

        
        //need to use the parent due to the structure of the prefab
        player.transform.position = startingPoints[Activeplayers.IndexOf(player)].position;

        //convert layer mask (bit) to an integer 
        int layerToAdd = (int)Mathf.Log(playerLayers[Activeplayers.Count - 1].value, 2);
        //Debug.Log("Layer TO Add "+layerToAdd);
        //set the layer
        //player.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = layerToAdd;
        //add the layer
        
        player.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
        SetNextPlayerPrefab();
        //set the action in the custom cinemachine Input Handler
        //playerParent.GetComponentInChildren<InputHandler>().horizontal = player.actions.FindAction("Look");

        playerCount++;

    }
}
