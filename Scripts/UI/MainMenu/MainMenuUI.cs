using RPG.SceneManagement;
using RPG.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        LazyValue<SavingWrapper> savingWrapper;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button newButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TMP_InputField newGameName;
        private void Awake()
        {
            savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }
        private void Start()
        {
            continueButton.onClick.AddListener(ContinueGame);
            newButton.onClick.AddListener(NewGame);
            quitButton.onClick.AddListener(QuitGame);
        }
        private SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }
        public void ContinueGame()
        {
            savingWrapper.value.ContinueGame();
        }
        public void NewGame()
        {
            // Add validation for any non alphanumeric characters
            char[] validatedText = newGameName.text.ToCharArray();

            validatedText = Array.FindAll<char>(validatedText, (c => (char.IsLetterOrDigit(c)
                                              || char.IsWhiteSpace(c))));
            newGameName.text = new string(validatedText);
            savingWrapper.value.NewGame(newGameName.text);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}