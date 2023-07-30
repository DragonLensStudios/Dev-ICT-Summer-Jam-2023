using DLS.Core.Data_Persistence.Extensions;
using DLS.Core.Input;
using DLS.Game.Enums;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.Utilities;
using DLS.Utilities;
using System.Collections;
using System.Collections.Generic;
using DLS.Game.PLayers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuilderMenuController : MonoBehaviour
{
    [SerializeField] private Button earthTowerButton;
    [SerializeField] private Button iceTowerButton;
    [SerializeField] private Button fireTowerButton;

    [SerializeField] private InputActionReference actionReference;
    private PlayerInputActions playerInput;

    public GameObject activeObject;

    private PlayerController player;
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInputActions();
        player = PlayerManager.Instance.Player;
    }
    void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Interact.performed += Interact_performed;

    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext input)
    {
        if (activeObject == null) return;
        if(player == null) return;
        player.Resources -= 1;
        activeObject.transform.parent = null;
        activeObject = null;
        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));


    }
    private void Update()
    {
        if (player == null) return;
        if (player.Resources <= 0)
        {
            earthTowerButton.interactable = false;
            iceTowerButton.interactable = false;
            fireTowerButton.interactable = false;
        }
        else
        {
            earthTowerButton.interactable = true;
            iceTowerButton.interactable = true;
            fireTowerButton.interactable = true;
        }
        if (activeObject == null) return;

        var placementPositon = activeObject.transform.position;
        if (player.lastMovementPosition == Vector2.up)//instant player input update needed
        {
            placementPositon = player.transform.position + Vector3.up;
        }
        else if (player.lastMovementPosition == Vector2.down)
        {
            placementPositon = player.transform.position + Vector3.down;
        }
        else if (player.lastMovementPosition == Vector2.right)
        {
            placementPositon = player.transform.position + Vector3.right;
        }
        else if (player.lastMovementPosition == Vector2.left)
        {
            placementPositon = player.transform.position + Vector3.left;
        }
        activeObject.transform.position = placementPositon;
    }
    void OnDisable()
    {
        playerInput.Player.Interact.performed -= Interact_performed;
        playerInput.Disable();
    }

     public async void SpawnTower(GameObject prefab)
    {
        string buttonName = InputHelper.GetButtonNameForAction(actionReference);
        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"Press {buttonName} to Place", PopupType.Notification, PopupPosition.Top));
        if (activeObject != null)
        {
            DestroyImmediate(activeObject);
            activeObject = null;
        }
        var prefabID = prefab.GetObjectPrefabID();
        var player = PlayerManager.Instance.Player;
        
        if(player != null)
        {
            var placementPositon = player.transform.position + Vector3.up;
            if (player.lastMovementPosition == Vector2.up) 
            {
                placementPositon = player.transform.position + Vector3.up;
            }
            else if(player.lastMovementPosition == Vector2.down)
            {
                placementPositon = player.transform.position + Vector3.down;
            }
            else if (player.lastMovementPosition == Vector2.right)
            {
                placementPositon = player.transform.position + Vector3.right;
            }
            else if (player.lastMovementPosition == Vector2.left)
            {
                placementPositon = player.transform.position + Vector3.left;
            }
            if (prefabID != null) 
            {
                var spawnedTower = await PrefabAssetLoader.Instance.SpawnPrefab(prefabID.Guid.ToString(), prefab.name, placementPositon, Quaternion.identity, Vector3.one, player.transform);//place tower
                activeObject = spawnedTower;
                MessageSystem.MessageManager.SendImmediate<CrystalSpawnerMessage>(MessageChannels.Spawning, new CrystalSpawnerMessage(spawnedTower, 1, 3));
            }
        }
        
        
    }
}
