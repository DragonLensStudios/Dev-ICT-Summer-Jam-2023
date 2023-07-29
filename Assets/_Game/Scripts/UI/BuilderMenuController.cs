using DLS.Core.Data_Persistence.Extensions;
using DLS.Core.Input;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderMenuController : MonoBehaviour
{
    private PlayerInputActions playerInput;

    private GameObject activeObject;
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
        }
        

    }

    void OnDisable()
    {
        playerInput.Player.Interact.performed -= Interact_performed;
        playerInput.Disable();

    }
    public async void SpawnTower(GameObject prefab)
    {
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
                var spawnedTower = await PrefabAssetLoader.Instance.SpawnPrefab(prefabID.Guid.ToString(), prefab.name, placementPositon, Quaternion.identity, Vector3.one, player.transform);
                activeObject = spawnedTower;
            }
        }
        
        
    }
}
