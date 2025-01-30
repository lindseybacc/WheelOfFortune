using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LeapWoF.Interfaces;
using System.Text.RegularExpressions;

namespace LeapWoF
{

    /// <summary>
    /// The GameManager class, handles all game logic
    /// </summary>
    public class GameManager
    {

        /// <summary>
        /// The input provider
        /// </summary>
        private IInputProvider inputProvider;


        /// <summary>
        /// The output provider
        /// </summary>
        private IOutputProvider outputProvider;
        
        public string currentDashPuzzle;
        private string message = "";
        private string TemporaryPuzzle;

        public List<string> charGuessList = new List<string>();

        public GameState GameState { get; private set; }

        public GameManager() : this(new ConsoleInputProvider(), new ConsoleOutputProvider())
        {

        }

        public GameManager(IInputProvider inputProvider, IOutputProvider outputProvider)
        {
            if (inputProvider == null)
                throw new ArgumentNullException(nameof(inputProvider));
            if (outputProvider == null)
                throw new ArgumentNullException(nameof(outputProvider));

            this.inputProvider = inputProvider;
            this.outputProvider = outputProvider;

            GameState = GameState.WaitingToStart;
        }

        /// <summary>
        /// Manage game according to game state
        /// </summary>
        public void StartGame()
        {
            InitGame();

            while (true)
            {

                PerformSingleTurn();

                if (GameState == GameState.RoundOver)
                {
                    StartNewRound();
                    continue;
                }

                if (GameState == GameState.GameOver)
                {
                    outputProvider.WriteLine("Game over");
                    break;
                }
            }
        }
        public void StartNewRound()
        {
            TemporaryPuzzle = "Hello world";
            currentDashPuzzle = Regex.Replace(TemporaryPuzzle, "[a-zA-Z]", "_");

            // update the game state
            GameState = GameState.RoundStarted;
        }

        public void PerformSingleTurn()
        {
            outputProvider.Clear();
            DrawPuzzle();
            outputProvider.WriteLine("Type 1 to spin, 2 to solve");
            GameState = GameState.WaitingForUserInput;

            var action = inputProvider.Read();

            switch (action)
            {
                case "1":
                    Spin();
                    break;
                case "2":
                    Solve();
                    break;
            }

        }

        /// <summary>
        /// This will show the board and display the dashes for the letters of the phrase
        /// </summary>
        private void DrawPuzzle()
        {
            outputProvider.WriteLine("The puzzle is:");
            outputProvider.WriteLine(message);
            outputProvider.WriteLine(currentDashPuzzle);

            outputProvider.WriteLine();
        }

        /// <summary>
        /// Spin the wheel and do the appropriate action
        /// </summary>
        public void Spin()
        {
            outputProvider.WriteLine("Spinning the wheel...");
            //TODO - Implement wheel + possible wheel spin outcomes
            GuessLetter();
        }

        public void Solve()
        {
            outputProvider.Write("Please enter your solution:");
            var guess = inputProvider.Read();
        }
        public void GuessLetter()
        {
            outputProvider.Write("Please guess a letter: ");
            var guess = inputProvider.Read().ToLower();


            if (guess.Length != 1)
            {
                message = "Hey please choose one letter!";
            } else if (charGuessList.Contains(guess))
            {
                message = "You already guessed that letter!";
            }
            else if (TemporaryPuzzle.ToLower().Contains(guess))
            {
                for (int i = 0; i < TemporaryPuzzle.Length; i++)
                {    
                    if (TemporaryPuzzle[i].ToString().ToLower() == guess)
                    {
                        currentDashPuzzle = currentDashPuzzle.Remove(i, 1).Insert(i, TemporaryPuzzle[i].ToString());

                    }

                }

                message = "You guessed correctly!";
            }
            else
            {
                message = "Sorry, that letter is not in the puzzle!";
            }
    

        

            charGuessList.Add(guess);
        }

        /// <summary>
        /// Optional logic to accept configuration options
        /// </summary>
        public void InitGame()
        {

            outputProvider.WriteLine("Welcome to Wheel of Fortune!");
            StartNewRound();
        }
    }
}
