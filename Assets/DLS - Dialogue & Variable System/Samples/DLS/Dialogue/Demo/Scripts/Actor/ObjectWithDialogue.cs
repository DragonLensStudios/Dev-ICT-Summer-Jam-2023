using System;
using System.Collections;
using System.Collections.Generic;
using DLS.Core;
using DLS.Dialogue;
using DLS.Game.Utilities;
using UnityEngine;

public class ObjectWithDialogue : MonoBehaviour, IDialogue
{
    public SerializableGuid LevelID { get; set; }
    public SerializableGuid PrefabID { get; set; }
    public SerializableGuid ID { get; set; }
    public string ObjectName { get; set; }
    public GameObject GameObject { get; }
    public GameObject TargetGameObject { get; set; }
    public bool IsInteracting { get; set; }
    public event Action<GameObject, GameObject> OnDialogueInteractAction;
    public event Action OnDialogueEndedAction;

    public Sprite Portrait { get; set; }
    public DialogueManager DialogueManager { get; set; }

    private void OnEnable()
    {
        OnDialogueInteractAction += OnDialogueInteract;
        OnDialogueEndedAction += OnDialogueEnded;
    }

    private void OnDisable()
    {
        OnDialogueInteractAction -= OnDialogueInteract;
        OnDialogueEndedAction -= OnDialogueEnded;
    }

    public void Interact()
    {
        if (TargetGameObject != null)
        {
            OnDialogueInteractAction.Invoke(gameObject, TargetGameObject);
        }    
    }

    public void OnDialogueInteract(GameObject source, GameObject target)
    {
        if (GameObject != target) return;
        if (DialogueManager == null) return;
        if (IsInteracting) return;
        if (!DialogueManager.StartDialogue()) return;
        DialogueUi.Instance.HideInteractionText();    }

    public void OnDialogueEnded()
    {
        IsInteracting = false;
        DialogueUi.Instance.HideInteractionText();
    }
}
