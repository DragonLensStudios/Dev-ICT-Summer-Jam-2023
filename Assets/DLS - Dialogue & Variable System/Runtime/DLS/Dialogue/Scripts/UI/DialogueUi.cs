using System;
using System.Collections;
using System.Linq;
using DLS.Core;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using DLS.Utilities.Extensions.StringExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XNode;

namespace DLS.Dialogue
{
    public class DialogueUi : MonoBehaviour
    {
        public static DialogueUi Instance { get; private set; }

        [SerializeField]
        private float typewriterTextDelayTime = 0.05f;


        [SerializeField]
        private TMP_Text actorNameUnderPortraitText,actorNameOverPortraitText, dialogueText, interactionText;

        [SerializeField]
        private Image actorImage;

        [SerializeField]
        private GameObject backgroundActorNameUnderPortraitBox, backgroundActorNameOverDialogueBox, backgroundPortraitBox, dialogueBox, dialogueChoiceButtonPrefab, dialogueChoiceContainer, continueButton;

        [SerializeField]
        private ChoiceDialogueNode activeSegment;
        [SerializeField]
        private DialogueGraph selectedGraph;

        private Coroutine _parser, _typeWriter;
        private EventSystem _eventSystem;

        public float TypewriterTextDelayTime { get => typewriterTextDelayTime; set => typewriterTextDelayTime = value; }
        public TMP_Text ActorNameUnderPortraitText { get => actorNameUnderPortraitText; set => actorNameUnderPortraitText = value; }
        public TMP_Text ActorNameOverPortraitText { get => actorNameOverPortraitText; set => actorNameOverPortraitText = value; }
        public TMP_Text DialogueText { get => dialogueText; set => dialogueText = value; }
        public TMP_Text InteractionText { get => interactionText; set => interactionText = value; }
        public Image ActorImage { get => actorImage; set => actorImage = value; }
        public GameObject BackgroundPortraitBox { get => backgroundPortraitBox; set => backgroundPortraitBox = value; }
        public GameObject BackgroundActorNameUnderPortraitBox { get => backgroundActorNameUnderPortraitBox; set => backgroundActorNameUnderPortraitBox = value; }
        public GameObject BackgroundActorNameOverDialogueBox { get => backgroundActorNameOverDialogueBox; set => backgroundActorNameOverDialogueBox = value; }
        public GameObject DialogueBox { get => dialogueBox; set => dialogueBox = value; }
        public GameObject DialogueChoiceButtonPrefab { get => dialogueChoiceButtonPrefab; set => dialogueChoiceButtonPrefab = value; }
        public GameObject DialogueChoiceContainer { get => dialogueChoiceContainer; set => dialogueChoiceContainer = value; }
        public ChoiceDialogueNode ActiveSegment { get => activeSegment; set => activeSegment = value; }
        public DialogueGraph SelectedGraph { get => selectedGraph; set => selectedGraph = value; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance);
            }
            else
            {
                _eventSystem = FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
                Instance = this;
            }
        }

        /// <summary>
        /// Shows the Interaction Text with the provided string
        /// </summary>
        /// <param name="text"></param>
        public void ShowInteractionText(string text)
        {
            if (!interactionText.gameObject.activeSelf)
            {
                interactionText.gameObject.SetActive(true);
            }
            if (!interactionText.text.Equals(text))
            {
                interactionText.text = text;
            }
        }

        /// <summary>
        /// Hides the Interaction Text
        /// </summary>
        public void HideInteractionText()
        {
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Starts a dialogue with the provided <see cref="DialogueGraph"/>
        /// </summary>
        /// <param name="graph"></param>
        public void StartDialogue(DialogueGraph graph)
        {
            try
            {
                if(graph != null)
                {
                    selectedGraph = graph;
                }

                if (selectedGraph != null)
                {
                    if (!dialogueBox.activeSelf)
                    {
                        dialogueBox.SetActive(true);
                    }

                    foreach (var node in selectedGraph.nodes)
                    {
                        var b = (BaseNode)node;
                        if (b.GetNodeType() != nameof(StartNode))
                            continue; //"b" is a reference to whatever node it's found next. It's an enumerator variable 
                        selectedGraph.current = b; //Make this node the starting point. The [g] sets what graph to Use in the array OnTriggerEnter
                        break;
                    }
                    _parser = StartCoroutine(ParseNode());
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"ERROR: Selected Graph Exception\n {ex.Message}");
            }
        }

        /// <summary>
        /// Clears the Dialogue Choices.
        /// </summary>
        private void ClearChoiceSelection()
        {
            if (dialogueChoiceContainer.activeSelf)
            {
                foreach (Transform child in dialogueChoiceContainer.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        /// <summary>
        /// This method updates the Choice Dialogue values and display.
        /// </summary>
        /// <param name="node"></param>
        private IEnumerator UpdateDialogue(ChoiceDialogueNode node)
        {
            activeSegment = node;
            if (_typeWriter != null)
            {
                StopCoroutine(_typeWriter);
                _typeWriter = null;
            }
            _typeWriter = StartCoroutine(TypewriterText(node.DialogueText.ParseObject(node), typewriterTextDelayTime, dialogueText));

            ClearChoiceSelection();
            int selectedIndex = 0;

            yield return new WaitUntil(() => dialogueText.maxVisibleCharacters >= node.DialogueText.ParseObject(node).Length);
            continueButton.SetActive(false);
            foreach (var answer in node.Answers)
            {
                var btnObj = Instantiate(dialogueChoiceButtonPrefab, dialogueChoiceContainer.transform); //spawns the buttons 
                btnObj.GetComponentInChildren<TMP_Text>().text = answer.ParseObject(node);
                var index = selectedIndex;
                var btn = btnObj.GetComponentInChildren<Button>();
                if(btn != null)
                {
                    if(selectedIndex == 0) { SetSelectedGameObject(btnObj); }
                    btn.onClick.AddListener((() => { AnswerClicked(index); }));
                }

                selectedIndex++;
            }

        }

        /// <summary>
        /// This method gets the port for the Answers dialogue and sets the <see cref="selectedGraph"/> to that connection node.
        /// </summary>
        /// <param name="clickedIndex"></param>
        private void AnswerClicked(int clickedIndex)
        { //Function when the choices button is pressed 
            dialogueChoiceContainer.SetActive(false);
            var port = activeSegment.GetPort("Answers " + clickedIndex);

            if (port.IsConnected)
            {
                selectedGraph.current = port.Connection.node as BaseNode;
                _parser = StartCoroutine(ParseNode());
            }
            else
            {
                dialogueBox.SetActive(false);
                NextNode("input");
                Debug.LogError("ERROR: ChoiceDialogue port is not connected");
                //NextNode("exit"); 

            }
        }

        private IEnumerator TypewriterText(string text,float delay, TMP_Text outputText)
        {
            //TODO: Find a way to not set the eventSystem.enabled to false?
            _eventSystem.enabled = false;
            continueButton.SetActive(true);
            SetSelectedGameObject(continueButton);
            outputText.maxVisibleCharacters = 0;
            outputText.text = text;
            for (int i = 0; i < text.Length; i++)
            {
                outputText.maxVisibleCharacters++;
                yield return new WaitForSeconds(delay);
                //TODO: Find a way to not set the eventSystem.enabled to true to re-enable the submit input action?
                _eventSystem.enabled = true;
            }
        }

        /// <summary>
        /// This IEnumerator method handles each node and their actions.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ParseNode()
        {
            interactionText.gameObject.SetActive(false);
            BaseNode b = selectedGraph.current;
            string nodeName = b.GetNodeType();
            var targetActor = b.TargetActor;
            var sourceActor = b.SourceActor;
            var sourceGameObject = b.SourceGameobject;
            
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ClearChoiceSelection();

            switch (nodeName)
            {
                case "StartNode":
                    NextNode("exit");
                    break;
                case "DialogueNode":
                    var dn = b as DialogueNode;
                    dialogueBox.SetActive(true);
                    if (dn != null)
                    {
                        if (dn.UseSourceActor)
                        {
                            dn.ActorName = sourceActor.ObjectName;
                            dn.Sprite = sourceActor.Portrait;
                        }
                        else if (dn.UseTargetActor)
                        {
                            dn.ActorName = targetActor.ObjectName;
                            dn.Sprite = targetActor.Portrait;
                        }
                        actorNameUnderPortraitText.text = dn.ActorName.ParseObject(dn);
                        actorNameOverPortraitText.text = dn.ActorName.ParseObject(dn);
                        if (_typeWriter != null)
                        {
                            StopCoroutine(_typeWriter);
                            _typeWriter = null;
                        }

                        _typeWriter = StartCoroutine(TypewriterText(dn.DialogueText.ParseObject(dn), typewriterTextDelayTime, dialogueText));
                    }

                    backgroundPortraitBox.SetActive(b.GetSprite() != null);
                    actorImage.sprite = b.GetSprite();
                    backgroundActorNameUnderPortraitBox.SetActive(!string.IsNullOrWhiteSpace(actorNameUnderPortraitText.text) && b.GetSprite() != null);
                    backgroundActorNameOverDialogueBox.SetActive(!string.IsNullOrWhiteSpace(actorNameUnderPortraitText.text) && b.GetSprite() == null);
                    break;
                case "ChoiceDialogueNode":
                    var cdn = b as ChoiceDialogueNode;
                    dialogueChoiceContainer.SetActive(true);
                    continueButton.SetActive(false);
                    if (cdn != null)
                    {
                        if (cdn.UseSourceActor)
                        {
                            cdn.ActorName = sourceActor.ObjectName;
                            cdn.Sprite = sourceActor.Portrait;
                        }
                        else if (cdn.UseTargetActor)
                        {
                            cdn.ActorName = targetActor.ObjectName;
                            cdn.Sprite = targetActor.Portrait;
                        }
                        actorNameUnderPortraitText.text = cdn.ActorName.ParseObject(cdn);
                        actorNameOverPortraitText.text = cdn.ActorName.ParseObject(cdn);
                        backgroundPortraitBox.SetActive(b.GetSprite() != null);
                        actorImage.sprite = b.GetSprite();
                        backgroundActorNameUnderPortraitBox.SetActive(!string.IsNullOrWhiteSpace(actorNameUnderPortraitText.text) && b.GetSprite() != null);
                        backgroundActorNameOverDialogueBox.SetActive(!string.IsNullOrWhiteSpace(actorNameUnderPortraitText.text) && b.GetSprite() == null);
                        StartCoroutine(UpdateDialogue(cdn)); //Instantiates the buttons 
                    }

                    break;
                case "VariableNode":
                    var vn = b as VariableNode;
                    Variable<int> intVar = null;
                    Variable<long> longVar = null;
                    Variable<short> shortVar = null;
                    Variable<double> doubleVar = null;
                    Variable<decimal> decimalVar = null;
                    Variable<float> floatVar = null;
                    Variable<bool> boolVar = null;
                    Variable<string> stringVar = null;
                    Variable<ComparableVector2> vector2Var = null;
                    Variable<ComparableVector3> vector3Var = null;
                    Variable<DateTime> dateTimeVar = null;

                    if (vn != null)
                    {
                        var existingIntVar = vn.Variables.IntVariables[vn.VariableName];
                        var existingLongVar = vn.Variables.LongVariables[vn.VariableName];
                        var existingShortVar = vn.Variables.ShortVariables[vn.VariableName];
                        var existingDoubleVar = vn.Variables.DoubleVariables[vn.VariableName];
                        var existingDecimalVar = vn.Variables.DecimalVariables[vn.VariableName];
                        var existingFloatVar = vn.Variables.FloatVariables[vn.VariableName];
                        var existingBoolVar = vn.Variables.BoolVariables[vn.VariableName];
                        var existingStringVar = vn.Variables.StringVariables[vn.VariableName];
                        var existingVector2Var = vn.Variables.Vector2Variables[vn.VariableName];
                        var existingVector3Var = vn.Variables.Vector3Variables[vn.VariableName];
                        var existingDateTimeVar = vn.Variables.DateTimeVariables[vn.VariableName];


                        try
                        {
                            switch (vn.VariableType)
                            {
                                case VariableType.Int:
                                    intVar = new Variable<int>(vn.VariableName,(int)vn.VariableValue);
                                    break;
                                case VariableType.Long:
                                    longVar = new Variable<long>(vn.VariableName, (long)vn.VariableValue);
                                    break;
                                case VariableType.Short:
                                    shortVar = new Variable<short>(vn.VariableName,(short)vn.VariableValue);
                                    break;
                                case VariableType.Double:
                                    doubleVar = new Variable<double>(vn.VariableName,(double)vn.VariableValue);
                                    break;
                                case VariableType.Decimal:
                                    decimalVar = new Variable<decimal>(vn.VariableName, (decimal)vn.VariableValue);
                                    break;
                                case VariableType.Float:
                                    floatVar = new Variable<float>(vn.VariableName,(float)vn.VariableValue);
                                    break;
                                case VariableType.Bool:
                                    boolVar = new Variable<bool>(vn.VariableName,(bool)vn.VariableValue);
                                    break;
                                case VariableType.String:
                                    stringVar = new Variable<string>(vn.VariableName,(string)vn.VariableValue);
                                    break;
                                case VariableType.Vector2:
                                    vector2Var = new Variable<ComparableVector2>(vn.VariableName,(ComparableVector2)vn.VariableValue);
                                    break;
                                case VariableType.Vector3:
                                    vector3Var = new Variable<ComparableVector3>(vn.VariableName,(ComparableVector3)vn.VariableValue);
                                    break;
                                case VariableType.DateTime:
                                    dateTimeVar = new Variable<DateTime>(vn.VariableName, (DateTime)vn.VariableValue);
                                    break;
                            }
                            switch (vn.OperatorType)
                            {
                                case Operator.Add:
                                    switch (vn.VariableType)
                                    {
                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    existingIntVar.Value += intVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(intVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(intVar.Name))
                                                    {
                                                        vn.Variables.IntVariables.Variables.Add(new Variable<int>(intVar.Name, intVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }

                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    existingLongVar.Value += longVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(longVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(longVar.Name))
                                                    {
                                                        vn.Variables.LongVariables.Variables.Add(new Variable<long>(longVar.Name, longVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    existingShortVar.Value += shortVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(shortVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(shortVar.Name))
                                                    {
                                                        vn.Variables.ShortVariables.Variables.Add(new Variable<short>(shortVar.Name, shortVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    existingDoubleVar.Value += doubleVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(doubleVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(doubleVar.Name))
                                                    {
                                                        vn.Variables.DoubleVariables.Variables.Add(new Variable<double>(doubleVar.Name, doubleVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    existingDecimalVar.Value += decimalVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(decimalVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(decimalVar.Name))
                                                    {
                                                        vn.Variables.DecimalVariables.Variables.Add(new Variable<decimal>(decimalVar.Name, decimalVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    existingFloatVar.Value += floatVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(floatVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(floatVar.Name))
                                                    {
                                                        vn.Variables.FloatVariables.Variables.Add(new Variable<float>(floatVar.Name, floatVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Bool:
                                            if (boolVar != null)
                                            {
                                                if (existingBoolVar != null)
                                                {
                                                    existingBoolVar.Value = true;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(boolVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(boolVar.Name))
                                                    {
                                                        vn.Variables.BoolVariables.Variables.Add(new Variable<bool>(boolVar.Name, boolVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.String:
                                            if (stringVar != null)
                                            {
                                                if (existingStringVar != null)
                                                {
                                                    existingStringVar.Value += stringVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(stringVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(stringVar.Name))
                                                    {
                                                        vn.Variables.StringVariables.Variables.Add(new Variable<string>(stringVar.Name, stringVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }

                                                }
                                            }
                                            
                                            break;
                                        case  VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    existingVector2Var.Value += vector2Var.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(vector2Var.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(vector2Var.Name))
                                                    {
                                                        vn.Variables.Vector2Variables.Variables.Add(new Variable<ComparableVector2>(vector2Var.Name, vector2Var.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            
                                            break;
                                        case  VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    existingVector3Var.Value += vector3Var.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(vector3Var.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(vector3Var.Name))
                                                    {
                                                        vn.Variables.Vector3Variables.Variables.Add(new Variable<ComparableVector3>(vector3Var.Name, vector3Var.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                        case VariableType.DateTime:
                                            if (dateTimeVar != null)
                                            {
                                                if (existingDateTimeVar != null)
                                                {
                                                    existingDateTimeVar.Value = dateTimeVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(dateTimeVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(dateTimeVar.Name))
                                                    {
                                                        vn.Variables.DateTimeVariables.Variables.Add(new Variable<DateTime>(dateTimeVar.Name, dateTimeVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                    }

                                    NextNode("exitTrue");
                                    break;
                                case Operator.Subtract:
                                    switch (vn.VariableType)
                                    {

                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    existingIntVar.Value -= intVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    existingLongVar.Value -= longVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    existingShortVar.Value -= shortVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    existingDoubleVar.Value -= doubleVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    existingDecimalVar.Value -= decimalVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    existingFloatVar.Value -= floatVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }  
                                            }
                                            break;
                                        case VariableType.Bool:
                                            if (boolVar != null)
                                            {
                                                if (existingBoolVar != null)
                                                {
                                                    existingBoolVar.Value = false;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.String:
                                            if (stringVar != null)
                                            {
                                                if (existingStringVar != null)
                                                {
                                                    existingStringVar.Value.Remove(stringVar.Value.Length);
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    existingVector2Var.Value -= vector2Var.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    existingVector3Var.Value -= vector3Var.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.DateTime:
                                            if (dateTimeVar != null)
                                            {
                                                if (existingDateTimeVar != null)
                                                {
                                                    existingDateTimeVar.Value = default;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                    }
                                    NextNode("exitTrue");
                                    break;
                                case Operator.Multiply:
                                    switch (vn.VariableType)
                                    {
                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    existingIntVar.Value *= intVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    existingLongVar.Value *= longVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    existingShortVar.Value *= shortVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    existingDoubleVar.Value *= doubleVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    existingDecimalVar.Value *= decimalVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    existingFloatVar.Value *= floatVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Bool:
                                            if (boolVar != null)
                                            {
                                                if (existingBoolVar != null)
                                                {
                                                    existingBoolVar.Value = true;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.String:
                                            NextNode("exitFalse");
                                            break;
                                        case VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    existingVector2Var.Value *= vector2Var.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    existingVector3Var.Value *= vector3Var.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.DateTime:
                                            NextNode("exitFalse");
                                            break;
                                    }
                                    NextNode("exitTrue");
                                    break;
                                case Operator.Divide:
                                    switch (vn.VariableType)
                                    {
                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    existingIntVar.Value /= intVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    existingLongVar.Value /= longVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    existingShortVar.Value /= shortVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    existingDoubleVar.Value /= doubleVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    existingDecimalVar.Value /= decimalVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    existingFloatVar.Value /= floatVar.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Bool:
                                            NextNode("exitFalse");
                                            break;
                                        case VariableType.String:
                                            NextNode("exitFalse");
                                            break;
                                        case VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    existingVector2Var.Value /= vector2Var.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    existingVector3Var.Value /= vector3Var.Value;
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.DateTime:
                                            NextNode("exitFalse");
                                            break;
                                    }
                                    NextNode("exitTrue");
                                    break;
                                case Operator.Set:
                                    switch (vn.VariableType)
                                    {
                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    existingIntVar.Value = intVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(intVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(intVar.Name))
                                                    {
                                                        vn.Variables.IntVariables.Variables.Add(new Variable<int>(intVar.Name, intVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    existingLongVar.Value = longVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(longVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(longVar.Name))
                                                    {
                                                        vn.Variables.LongVariables.Variables.Add(new Variable<long>(longVar.Name, longVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    existingShortVar.Value = shortVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(shortVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(shortVar.Name))
                                                    {
                                                        vn.Variables.ShortVariables.Variables.Add(new Variable<short>(shortVar.Name, shortVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    existingDoubleVar.Value = doubleVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(doubleVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(doubleVar.Name))
                                                    {
                                                        vn.Variables.DoubleVariables.Variables.Add(new Variable<double>(doubleVar.Name, doubleVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    existingDecimalVar.Value = decimalVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(decimalVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(decimalVar.Name))
                                                    {
                                                        vn.Variables.DecimalVariables.Variables.Add(new Variable<decimal>(decimalVar.Name, decimalVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    existingFloatVar.Value = floatVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(floatVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(floatVar.Name))
                                                    {
                                                        vn.Variables.FloatVariables.Variables.Add(new Variable<float>(floatVar.Name, floatVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                        case VariableType.Bool:
                                            if (boolVar != null)
                                            {
                                                if (existingBoolVar != null)
                                                {
                                                    existingBoolVar.Value = boolVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(boolVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(boolVar.Name))
                                                    {
                                                        vn.Variables.BoolVariables.Variables.Add(new Variable<bool>(boolVar.Name, boolVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                        case VariableType.String:
                                            if (stringVar != null)
                                            {
                                                if (existingStringVar != null)
                                                {
                                                    existingStringVar.Value = stringVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(stringVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(stringVar.Name))
                                                    {
                                                        vn.Variables.StringVariables.Variables.Add(new Variable<string>(stringVar.Name, stringVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                        case VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    existingVector2Var.Value = vector2Var.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(vector2Var.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(vector2Var.Name))
                                                    {
                                                        vn.Variables.Vector2Variables.Variables.Add(new Variable<ComparableVector2>(vector2Var.Name, vector2Var.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    existingVector3Var.Value = vector3Var.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(vector3Var.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(vector3Var.Name))
                                                    {
                                                        vn.Variables.Vector3Variables.Variables.Add(new Variable<ComparableVector3>(vector3Var.Name, vector3Var.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                        case VariableType.DateTime:
                                            if (dateTimeVar != null)
                                            {
                                                if (existingDateTimeVar != null)
                                                {
                                                    existingDateTimeVar.Value = dateTimeVar.Value;
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrWhiteSpace(dateTimeVar.Name))
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                    else if (!DoesVariableExist(dateTimeVar.Name))
                                                    {
                                                        vn.Variables.DateTimeVariables.Variables.Add(new Variable<DateTime>(dateTimeVar.Name, dateTimeVar.Value));
                                                    }
                                                    else
                                                    {
                                                        NextNode("exitFalse");
                                                    }
                                                }
                                            }
                                            break;
                                    }

                                    NextNode("exitTrue");
                                    break;
                                case Operator.EqualTo:
                                    switch (vn.VariableType)
                                    {
                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    NextNode(existingIntVar.Value.Equals(intVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    NextNode(existingLongVar.Value.Equals(longVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    NextNode(existingShortVar.Value.Equals(shortVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    NextNode(existingDoubleVar.Value.Equals(doubleVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    NextNode(existingDecimalVar.Value.Equals(decimalVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    NextNode(existingFloatVar.Value.Equals(floatVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Bool:
                                            if (boolVar != null)
                                            {
                                                if (existingBoolVar != null)
                                                {
                                                    NextNode(existingBoolVar.Value.Equals(boolVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.String:
                                            if (stringVar != null)
                                            {
                                                if (existingStringVar != null)
                                                {
                                                    NextNode(existingStringVar.Value.Equals(stringVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                } 
                                            }
                                            break;
                                        case VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    NextNode(existingVector2Var.Value.Equals(vector2Var.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    NextNode(existingVector3Var.Value.Equals(vector3Var.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.DateTime:
                                            if (dateTimeVar != null)
                                            {
                                                if (existingDateTimeVar != null)
                                                {
                                                    NextNode(existingDateTimeVar.Value.Equals(dateTimeVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                    }

                                    break;
                                case Operator.NotEqualTo:
                                    switch (vn.VariableType)
                                    {
                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    NextNode(!existingIntVar.Value.Equals(intVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    NextNode(!existingLongVar.Value.Equals(longVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    NextNode(!existingShortVar.Value.Equals(shortVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    NextNode(!existingDoubleVar.Value.Equals(doubleVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    NextNode(!existingDecimalVar.Value.Equals(decimalVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    NextNode(!existingFloatVar.Value.Equals(floatVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Bool:
                                            if (boolVar != null)
                                            {
                                                if (existingBoolVar != null)
                                                {
                                                    NextNode(!existingBoolVar.Value.Equals(boolVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.String:
                                            if (stringVar != null)
                                            {
                                                if (existingStringVar != null)
                                                {
                                                    NextNode(!existingStringVar.Value.Equals(stringVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    NextNode(!existingVector2Var.Value.Equals(vector2Var.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    NextNode(!existingVector3Var.Value.Equals(vector3Var.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.DateTime:
                                            if (dateTimeVar != null)
                                            {
                                                if (existingDateTimeVar != null)
                                                {
                                                    NextNode(!existingDateTimeVar.Value.Equals(dateTimeVar.Value) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                case Operator.GreaterThan:
                                    switch (vn.VariableType)
                                    {
                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    NextNode(existingIntVar.GreaterThan(intVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    NextNode(existingLongVar.GreaterThan(longVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    NextNode(existingShortVar.GreaterThan(shortVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }  
                                            }
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    NextNode(existingDoubleVar.GreaterThan(doubleVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    NextNode(existingDecimalVar.GreaterThan(decimalVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    NextNode(existingFloatVar.GreaterThan(floatVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Bool:
                                            if (boolVar != null)
                                            {
                                                if (existingBoolVar != null)
                                                {
                                                    NextNode(existingBoolVar.GreaterThan(boolVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            
                                            break;
                                        case VariableType.String:
                                            if (stringVar != null)
                                            {
                                                if (existingStringVar != null)
                                                {
                                                    NextNode(existingStringVar.GreaterThan(stringVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    NextNode(existingVector2Var.GreaterThan(vector2Var) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    NextNode(existingVector3Var.GreaterThan(vector3Var) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.DateTime:
                                            if (dateTimeVar != null)
                                            {
                                                if (existingDateTimeVar != null)
                                                {
                                                    NextNode(existingDateTimeVar.GreaterThan(dateTimeVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                case Operator.GreaterThanOrEqual:
                                    switch (vn.VariableType)
                                    {
                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    NextNode(existingIntVar.GreaterThanOrEqual(intVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    NextNode(existingLongVar.GreaterThanOrEqual(longVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    NextNode(existingShortVar.GreaterThanOrEqual(shortVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    NextNode(existingDoubleVar.GreaterThanOrEqual(doubleVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    NextNode(existingDecimalVar.GreaterThanOrEqual(decimalVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    NextNode(existingFloatVar.GreaterThanOrEqual(floatVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Bool:
                                            if (boolVar != null)
                                            {
                                                if (existingBoolVar != null)
                                                {
                                                    NextNode(existingBoolVar.GreaterThanOrEqual(boolVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.String:
                                            if (stringVar != null)
                                            {
                                                if (existingStringVar != null)
                                                {
                                                    NextNode(existingStringVar.GreaterThanOrEqual(stringVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    NextNode(existingVector2Var.GreaterThanOrEqual(vector2Var) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    NextNode(existingVector3Var.GreaterThanOrEqual(vector3Var) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.DateTime:
                                            if (dateTimeVar != null)
                                            {
                                                if (existingDateTimeVar != null)
                                                {
                                                    NextNode(existingDateTimeVar.GreaterThanOrEqual(dateTimeVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                case Operator.LessThan:
                                    switch (vn.VariableType)
                                    {
                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    NextNode(existingIntVar.LessThan(intVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    NextNode(existingLongVar.LessThan(longVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    NextNode(existingShortVar.LessThan(shortVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    NextNode(existingDoubleVar.LessThan(doubleVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    NextNode(existingDecimalVar.LessThan(decimalVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    NextNode(existingFloatVar.LessThan(floatVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Bool:
                                            if (boolVar != null)
                                            {
                                                if (existingBoolVar != null)
                                                {
                                                    NextNode(existingBoolVar.LessThan(boolVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.String:
                                            if (stringVar != null)
                                            {
                                                if (existingStringVar != null)
                                                {
                                                    NextNode(existingStringVar.LessThan(stringVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    NextNode(existingVector2Var.LessThan(vector2Var) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    NextNode(existingVector3Var.LessThan(vector3Var) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.DateTime:
                                            if (dateTimeVar != null)
                                            {
                                                if (existingDateTimeVar != null)
                                                {
                                                    NextNode(existingDateTimeVar.LessThan(dateTimeVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                } 
                                            }
                                            break;
                                    }
                                    break;
                                case Operator.LessThanOrEqual:
                                    switch (vn.VariableType)
                                    {
                                        case VariableType.Int:
                                            if (intVar != null)
                                            {
                                                if (existingIntVar != null)
                                                {
                                                    NextNode(existingIntVar.LessThanOrEqual(intVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Long:
                                            if (longVar != null)
                                            {
                                                if (existingLongVar != null)
                                                {
                                                    NextNode(existingLongVar.LessThanOrEqual(longVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Short:
                                            if (shortVar != null)
                                            {
                                                if (existingShortVar != null)
                                                {
                                                    NextNode(existingShortVar.LessThanOrEqual(shortVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Double:
                                            if (doubleVar != null)
                                            {
                                                if (existingDoubleVar != null)
                                                {
                                                    NextNode(existingDoubleVar.LessThanOrEqual(doubleVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Decimal:
                                            if (decimalVar != null)
                                            {
                                                if (existingDecimalVar != null)
                                                {
                                                    NextNode(existingDecimalVar.LessThanOrEqual(decimalVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                } 
                                            }
                                            break;
                                        case VariableType.Float:
                                            if (floatVar != null)
                                            {
                                                if (existingFloatVar != null)
                                                {
                                                    NextNode(existingFloatVar.LessThanOrEqual(floatVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Bool:
                                            if (boolVar != null)
                                            {
                                                if (existingBoolVar != null)
                                                {
                                                    NextNode(existingBoolVar.LessThanOrEqual(boolVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.String:
                                            if (stringVar != null)
                                            {
                                                if (existingStringVar != null)
                                                {
                                                    NextNode(existingStringVar.LessThanOrEqual(stringVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector2:
                                            if (vector2Var != null)
                                            {
                                                if (existingVector2Var != null)
                                                {
                                                    NextNode(existingVector2Var.LessThanOrEqual(vector2Var) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                        case VariableType.Vector3:
                                            if (vector3Var != null)
                                            {
                                                if (existingVector3Var != null)
                                                {
                                                    NextNode(existingVector3Var.LessThanOrEqual(vector3Var) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                } 
                                            }
                                            break;
                                        case VariableType.DateTime:
                                            if (dateTimeVar != null)
                                            {
                                                if (existingDateTimeVar != null)
                                                {
                                                    NextNode(existingDateTimeVar.LessThanOrEqual(dateTimeVar) ? "exitTrue" : "exitFalse");
                                                }
                                                else
                                                {
                                                    NextNode("exitFalse");
                                                }
                                            }
                                            break;
                                    }
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Error: {e.Message}");
                        }
                    }

                    break;
            
                case "ReferenceStateNode":
                    var rsn = b as ReferenceStateNode;
                    targetActor.DialogueManager.CurrentReferenceState = rsn.ReferenceState;
                    NextNode("exitTrue");
                    break;
                case "ExitNode_NoLoop_toStart":

                    dialogueBox.SetActive(false);
                    interactionText.gameObject.SetActive(true);
                    //TODO: Remove possibly? If this is not in here controllers activate the dialogue again on a single press
                    yield return new WaitForSeconds(0.05f);
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Dialogue, new DialogueEndedMessage());
                    break;
                case "ExitNode":
                    dialogueBox.SetActive(false);
                    interactionText.gameObject.SetActive(true);
                    selectedGraph.Start();
                    //TODO: Remove possibly? If this is not in here controllers activate the dialogue again on a single press
                    yield return new WaitForSeconds(0.05f);
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Dialogue, new DialogueEndedMessage());
                    break;
            }
        }

        /// <summary>
        /// This method handles navigation to the next node based on the exit node string value.
        /// </summary>
        /// <param name="fieldName"></param>
        private void NextNode(string fieldName)
        {
            actorNameUnderPortraitText.text = "";
            dialogueText.text = "";
            ClearChoiceSelection();
            if (_parser != null)
            {
                StopCoroutine(_parser);
                _parser = null;
            }
            try
            {
                foreach (NodePort p in selectedGraph.current.Ports)
                {
                    try
                    {
                        if (p.fieldName == fieldName)
                        {
                            selectedGraph.current = p.Connection.node as BaseNode;
                            break;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Debug.LogError("ERROR: Port is not connected");
                    }
                }
            }
            catch (NullReferenceException)
            {
                Debug.LogError("ERROR: One of the elements on the Graph array at NodeParser is empty. Or, the Dialogue Graph is empty");
            }

            _parser = StartCoroutine(ParseNode());

        }


        public void NextDialogue()
        {
            if (dialogueBox.activeSelf)
            {
                var dialogueNode = selectedGraph.current as DialogueNode;
                var choiceNode = selectedGraph.current as ChoiceDialogueNode;
                if(dialogueNode != null)
                {
                    if(dialogueText.maxVisibleCharacters < dialogueNode.DialogueText.ParseObject(dialogueNode).Length)
                    {
                        if (_typeWriter != null)
                        {
                            StopCoroutine(_typeWriter);
                            _typeWriter = null;
                        }
                        dialogueText.maxVisibleCharacters = dialogueNode.DialogueText.ParseObject(dialogueNode).Length;
                    }
                    else
                    {
                        NextNode("exit");
                    }
                }
                if(choiceNode != null)
                {
                    if (dialogueText.maxVisibleCharacters < choiceNode.DialogueText.ParseObject(choiceNode).Length)
                    {
                        if (_typeWriter != null)
                        {
                            StopCoroutine(_typeWriter);
                            _typeWriter = null;
                        }
                        dialogueText.maxVisibleCharacters = choiceNode.DialogueText.ParseObject(choiceNode).Length;
                    }
                    else
                    {
                        NextNode("exit");
                    }
                }
            }
        }

        private void SetSelectedGameObject(GameObject go)
        {
            _eventSystem.SetSelectedGameObject(null);
            _eventSystem.SetSelectedGameObject(go);
        }
        
        private bool DoesVariableExist(string variableName)
        {
            return selectedGraph.current.Variables.IntVariables.Variables.Any(v => v.Name == variableName) ||
                   selectedGraph.current.Variables.LongVariables.Variables.Any(v => v.Name == variableName) ||
                   selectedGraph.current.Variables.ShortVariables.Variables.Any(v => v.Name == variableName) ||
                   selectedGraph.current.Variables.DoubleVariables.Variables.Any(v => v.Name == variableName) ||
                   selectedGraph.current.Variables.DecimalVariables.Variables.Any(v => v.Name == variableName) ||
                   selectedGraph.current.Variables.FloatVariables.Variables.Any(v => v.Name == variableName) ||
                   selectedGraph.current.Variables.BoolVariables.Variables.Any(v => v.Name == variableName) ||
                   selectedGraph.current.Variables.StringVariables.Variables.Any(v => v.Name == variableName) ||
                   selectedGraph.current.Variables.Vector2Variables.Variables.Any(v => v.Name == variableName) ||
                   selectedGraph.current.Variables.Vector3Variables.Variables.Any(v => v.Name == variableName) ||
                   selectedGraph.current.Variables.DateTimeVariables.Variables.Any(v => v.Name == variableName);
        }
    }
}