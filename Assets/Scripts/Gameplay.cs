using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

public class Gameplay : MonoBehaviour
{

    [Header("UI Elements")]
    public TextMeshProUGUI attempsLeft;
    public TextMeshProUGUI CurrentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;

    [Header("Input")]
    public TMP_InputField guessInputField;
    public Button submitButton;
    public Button newgameButton;

    [Header("Game Settings")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttemps = 12;

    private int targetNumber;
    private int currentAttemps;
    private bool isPlayerTurn;
    private bool gameActive;

    private int computerMinGuess;
    private int computerMaxGuess;
    private List<int> computerGuesses;

    void InitializeUI()
    {
        submitButton.onClick.AddListener(SubmitGuess);
        newgameButton.onClick.AddListener(StartNewGame);
        guessInputField.onSubmit.AddListener(delegate { SubmitGuess(); });
    }

    void SubmitGuess()
    {
        if (!gameActive || !isPlayerTurn) return;

        string input = guessInputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        int guess;
        if (!int.TryParse(input, out guess))
        {
            gameState.text = "Please enter a valid number";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text = $"Please enter a number between {minNumber} - {maxNumber}";
            return;
        }
        ProcessGuess(guess, true);
        guessInputField.text = "";
    }

    void ProcessGuess(int guess, bool isPlayerTurn)
    {
        currentAttemps++;
        string PlayerName = isPlayerTurn ? "Player Turn" : "Computer Turn";

        gameLog.text += $"{PlayerName} guessd: {guess}\n";

        if (guess == targetNumber)
        {
            //win
            gameLog.text += $"{PlayerName} got it right!\n <sprite=\"symbols01\" index=61> <sprite=\"usesymbols\" index=21>";
            EndGame();
        }

        else if (currentAttemps >= maxAttemps)
        {
            //lose
            gameLog.text += $"Game Over! The correct number was {targetNumber}\n ";
            EndGame();
        }

        else
        {
            //Wrong guess - give hint
            string hint = guess < targetNumber ? "Top Low <sprite=\"symbols01\" index=30> <sprite=\"usesymbols\" index=24>" : "Too High <sprite=\"symbols01\" index=30> <sprite=\"usesymbols\" index=24>";
            gameLog.text += $"{hint}\n";

            //Switch player
            isPlayerTurn = !isPlayerTurn;
            CurrentPlayer.text = isPlayerTurn ? "Player" : "Computer";
            attempsLeft.text = $"Attemps Left: {maxAttemps - currentAttemps}";

            if (!isPlayerTurn)
            {
                guessInputField.interactable = false;
                submitButton.interactable = false;
                StartCoroutine(ComputerTurn(guess < targetNumber));
            }
            else
            {
                guessInputField.interactable=true;
                submitButton.interactable=true;
                guessInputField.Select();
                guessInputField.ActivateInputField();

            }
        }
    }

    IEnumerator ComputerTurn(bool targetISHigher)
    {
        yield return new WaitForSeconds(2f);
        if (!gameActive) yield break;
        if (computerGuesses.Count >0)
        {
            int lastGuess = computerGuesses[computerGuesses.Count - 1];
            if (targetISHigher)
            {
                computerMinGuess = lastGuess + 1;
            }
            else
            {
                computerMaxGuess = lastGuess - 1;
            }
        }
        int computerGuess = (computerMinGuess + computerMaxGuess) / 2;

        computerGuesses.Add(computerGuess);
    
        ProcessGuess(computerGuess, false);
    }

    void EndGame()
    {
        gameActive = false;
        guessInputField.interactable = false;
        submitButton.interactable =false;
        CurrentPlayer.text = "Game Over - Click New Game to start again";
        Canvas.ForceUpdateCanvases();
    }

    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttemps = 0;
        isPlayerTurn = true;
        gameActive = true;

        CurrentPlayer.text = "Player Turn";
        attempsLeft.text = $"Attemps Left: {maxAttemps}";
        gameLog.text = "=== Game Log ===\n";
        gameState.text = "New game started! Player goes first";

        guessInputField.interactable = true;
        submitButton.interactable = true;
        guessInputField.text = " ";
        guessInputField.Select();
        guessInputField.ActivateInputField();

        computerMinGuess = minNumber;
        computerMaxGuess = maxNumber;
        computerGuesses = new List<int>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeUI();
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
