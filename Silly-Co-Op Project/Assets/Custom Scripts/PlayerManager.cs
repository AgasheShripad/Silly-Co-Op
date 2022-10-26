using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField]
    private List<Transform> startingPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;
    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
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
        players.Add(player);

        
        //need to use the parent due to the structure of the prefab
        player.transform.position = startingPoints[players.Count - 1].position;

        //convert layer mask (bit) to an integer 
        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);
        //Debug.Log("Layer TO Add "+layerToAdd);
        //set the layer
        //player.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = layerToAdd;
        //add the layer

        player.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;

        //set the action in the custom cinemachine Input Handler
        //playerParent.GetComponentInChildren<InputHandler>().horizontal = player.actions.FindAction("Look");

    }
}
