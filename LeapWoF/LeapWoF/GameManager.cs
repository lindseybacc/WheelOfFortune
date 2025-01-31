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
        private int CurrentTotal = 0;
        private int[] values = { 100, 300, 500, 1000, 0 };
        private string[] messages = { "Hello World", "You are a Leaper", "The more you plan the less you recode"};

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
            Random random = new Random();
            TemporaryPuzzle = messages[random.Next(0, 3)];
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
            outputProvider.WriteLine("The puzzle is: " + currentDashPuzzle);
            outputProvider.WriteLine(message);
            outputProvider.WriteLine();
            outputProvider.WriteLine("Your Total Money: $" + CurrentTotal);
            outputProvider.WriteLine();
        }

        /// <summary>
        /// Spin the wheel and do the appropriate action
        /// </summary>
        public void Spin()
        {
            Random random = new Random();
            int temp_money = random.Next(0, 5);
            outputProvider.WriteLine("Spinning the wheel for..." + "  $" + values[temp_money]);
            GuessLetter(temp_money);
        
        }
        public void checkSolve(string guess)
        {
            if (guess.ToLower() == TemporaryPuzzle.ToLower())
            {
                outputProvider.Clear();
                message = "Congratulations! You solved the puzzle!";

                currentDashPuzzle = TemporaryPuzzle;

                DrawPuzzle();
                GameState = GameState.GameOver;
            }
        }
        public void Solve()
        {
            outputProvider.Write("Please enter your solution:");
            var guess = inputProvider.Read();

           checkSolve(guess);

           message = "Sorry, that is not the correct solution!";
        }
        public void GuessLetter(int temp_money)
        {
            if (temp_money == 4)
            {
                message = "Bankrupt!";
                CurrentTotal = 0;
            }
            else
            {
                outputProvider.Write("Please guess a letter: ");
                var guess = inputProvider.Read().ToLower();

                int counter = 0;

                if (guess.Length != 1)
                {
                    message = "Hey please choose one letter!";
                }
                else if (charGuessList.Contains(guess))
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

                            counter += 1;
                        }
                    }

                    CurrentTotal += values[temp_money] * counter;

                    message = "You guessed correctly!";
                    checkSolve(currentDashPuzzle);
                }
                else
                {
                    message = "Sorry, that letter is not in the puzzle!";
                }

                charGuessList.Add(guess);
            }
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
