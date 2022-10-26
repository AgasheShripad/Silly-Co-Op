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
        playerInputManager.onPlayerJoined += ToggleThis;
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= ToggleThis;
    }

    private void ToggleThis(PlayerInput player)
    {
        this.gameObject.SetActive(false);
    }
}

