using RPG.Core;
using RPG.Quests;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] private string playerName = "Darian";
        private Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        private AIConversant currentConversant = null;
        private bool isChoosing = false;

        public event Action onConversationUpdated;
        public float timeSinceLastDialogue = Mathf.Infinity;
        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            // Check timer so its not spammed
            if (timeSinceLastDialogue < 0.5f)
            {
                return;
            }
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            if (currentNode.GetText == "$skip")
            {
                Next();
            }
            else if (currentNode.GetText == "$random")
            {
                ChooseRandom();
            }
            TriggerEnterAction();
            onConversationUpdated?.Invoke();
            timeSinceLastDialogue = 0f;
        }
        List<IPredicateEvaluator> evaluators = new List<IPredicateEvaluator>();
        List<IPredicateEvaluator> GetEvaluators() => evaluators;

        void Awake()
        {
            // Populate evaluators and quest list so they don't cause lag later
            evaluators = GetComponents<IPredicateEvaluator>().ToList();
            Quest.PopulateQuestDictionary();
        }

        private void Update()
        {
            timeSinceLastDialogue += Time.deltaTime;
        }
        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }
            return currentNode.GetText;
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode choice)
        {
            currentNode = choice;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }
        public void ChooseRandom()
        {
            // If we get here, we're at an AI node
            DialogueNode[] childNodes = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            if (childNodes.Length == 0)
            {
                Quit();
                return;
            }
            // Pick a random node
            DialogueNode bestNode = childNodes[UnityEngine.Random.Range(0, childNodes.Length)];
            TriggerExitAction();
            currentNode = bestNode;
            TriggerExitAction();
            // Call update event
            onConversationUpdated();
        }
        public void Next()
        {
            // Check if we're at a player node
            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }
            // If we get here, we're at an AI node
            DialogueNode[] childNodes = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            if (childNodes.Length == 0)
            {
                Quit();
                return;
            }
            // Pick the highest priority node
            DialogueNode bestNode = childNodes[0];
            foreach (DialogueNode node in childNodes)
            {
                if (node.GetPriority() > bestNode.GetPriority())
                {
                    bestNode = node;
                }
                else if (node.GetPriority() == bestNode.GetPriority())
                {
                    // Randomly pick one of the nodes if they have equal priority
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        bestNode = node;
                    }
                }
            }
            // Choose a random response
            TriggerExitAction();
            currentNode = bestNode;
            TriggerEnterAction();
            // Call update event
            onConversationUpdated();
        }

        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentConversant = null;
            currentNode = null;
            isChoosing = false;
            onConversationUpdated();
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else if (!currentNode.GetOverwriteName().IsNullOrWhitespace())
            {
                return currentNode.GetOverwriteName();
            }
            else
            {
                return currentConversant.GetName();
            }
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (DialogueNode node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }


        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") return;

            DialogueTrigger[] dialogueTriggers = currentConversant.GetComponents<DialogueTrigger>();
            foreach (DialogueTrigger trigger in dialogueTriggers)
            {
                trigger.Trigger(action);
            }
        }
    }
}