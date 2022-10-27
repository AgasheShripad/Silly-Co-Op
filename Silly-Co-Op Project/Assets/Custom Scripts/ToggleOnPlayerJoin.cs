using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleOnPlayerJoin : MonoBehaviour
{
    [SerializeField]
    private PlayerInputManager playerInputManager;


    private void Awake()
    {
       if(playerInputManager==null) playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += ToggleAudioThis;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= ToggleAudioThis;
    }

    private void ToggleAudioThis(PlayerInput player)
    {
        this.GetComponent<AudioListener>().enabled = false;
    }
}

