using DLS.Core.Data_Persistence.Extensions;
using DLS.Core.Input;
using DLS.Game.Enums;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.Utilities;
using DLS.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuilderMenuController : MonoBehaviour
{
    [SerializeField] private InputActionReference actionReference;
    private PlayerInputActions playerInput;

    public GameObject activeObject;
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInputActions();
    }
    void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Interact.performed += Interact_performed;

    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext input)
    {
        if(activeObject != null)
        {
            activeObject.transform.parent = null;
            activeObject = null;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
        }
        

    }
    private void Update()
    {
        if (activeObject != null)
        {
            var player = PlayerManager.Instance.Player;

            if (player != null)
            {

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
        }
        else
        {

        }
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
                var instanceID = spawnedTower.GetObjectInstanceID();

                activeObject = spawnedTower;
                MessageSystem.MessageManager.SendImmediate<CrystalSpawnerMessage>(MessageChannels.Spawning, new CrystalSpawnerMessage(spawnedTower, 1, 3));
            }
        }
        
        
    }
}
