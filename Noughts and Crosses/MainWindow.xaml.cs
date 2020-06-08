using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Noughts_and_Crosses
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Members

        /// <summary>
        /// Hold the current results of the cells in the active game
        /// </summary>
        private MarkType[] mResults;

        /// <summary>
        /// True if it is player 1's turn (X) or player 2's turn (O)
        /// </summary>
        private bool mPlayer1Turn;

        /// <summary>
        /// True if the game has ended
        /// </summary>
        private bool mGameEnded;
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            NewGame();
        }

        #endregion

        /// <summary>
        /// Starts a new game and clears values back to default
        /// </summary>
        private void NewGame()
        {
            // Create a new blank array of free cells
            mResults = new MarkType[9];

            for (int i = 0; i < mResults.Length; i++)
            {
                mResults[i] = MarkType.Free;
            }

            // Ensure player 1 is active turn
            mPlayer1Turn = true;

            // Iterate every button on the grid
            Container.Children.Cast<Button>().ToList().ForEach(button =>
            {
                // Change background, foreground and content to default values
                button.Content = string.Empty;
                button.Background = Brushes.White;
                button.Foreground = Brushes.Blue;
            });

            // Make sure game isn't finished
            mGameEnded = false;
        }

        /// <summary>
        /// Handles a button click event
        /// </summary>
        /// <param name="sender">The button clicked</param>
        /// <param name="e">The events of the click</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Start a new game on click if game is ended
            if (mGameEnded)
            {
                NewGame();
                return;
            }

            // Cast sender to a button
            var button = (Button)sender;

            // Find button position in array
            var column = Grid.GetColumn(button);
            var row = Grid.GetRow(button);

            var index = column + (row * 3);

            // Return if cell isn't free
            if (mResults[index] != MarkType.Free)
                return;

            // Set the cell value based on turn
            mResults[index] = mPlayer1Turn ? MarkType.Cross : MarkType.Nought;

            // Set button content to result
            button.Content = mPlayer1Turn ? "X" : "O";

            // Set colour of Noughts to red
            if (!mPlayer1Turn)
                button.Foreground = Brushes.Red;

            // Toggle turn
            mPlayer1Turn = !mPlayer1Turn;

            // Check for a winner
            CheckForWinner();

            // Evaluate board if AI turn
            int AIScore = EvaluateBoard();
            MessageBox.Show(AIScore.ToString());
        }

        /// <summary>
        /// Check if there is a winner
        /// </summary>
        private void CheckForWinner()
        {
            // Check for horizontal win
            for (int index = 0; index < 3; index++)
            {
                if (mResults[0 + (index * 3)] != MarkType.Free && (mResults[0 + (index * 3)] & mResults[1 + (index * 3)] & mResults[2 + (index * 3)]) == mResults[0 + (index * 3)])
                {
                    // Game has ended
                    mGameEnded = true;

                    // Highlight winning cells in green
                    switch (index)
                    {
                        case 0:
                            {
                                Button0_0.Background = Button1_0.Background = Button2_0.Background = Brushes.Green;
                                break;
                            }
                        case 1:
                            {
                                Button0_1.Background = Button1_1.Background = Button2_1.Background = Brushes.Green;
                                break;
                            }
                        case 2:
                            {
                                Button0_2.Background = Button1_2.Background = Button2_2.Background = Brushes.Green;
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            
            // Check for vertical win
            for (int index = 0; index < 3; index++)
            {
                if (mResults[0 + (index)] != MarkType.Free && (mResults[0 + (index)] & mResults[3 + (index)] & mResults[6 + (index)]) == mResults[0 + (index)])
                {
                    // Game has ended
                    mGameEnded = true;

                    // Highlight winning cells in green
                    switch (index)
                    {
                        case 0:
                            {
                                Button0_0.Background = Button0_1.Background = Button0_2.Background = Brushes.Green;
                                break;
                            }
                        case 1:
                            {
                                Button1_0.Background = Button1_1.Background = Button1_2.Background = Brushes.Green;
                                break;
                            }
                        case 2:
                            {
                                Button2_0.Background = Button2_1.Background = Button2_2.Background = Brushes.Green;
                                break;
                            }
                        default:
                            break;
                    }
                }
            }

            // Check for diagonal win
            if (mResults[0] != MarkType.Free && (mResults[0] & mResults[4] & mResults[8]) == mResults[0])
            {
                // Game has ended
                mGameEnded = true;

                // Highlight winning cells in green
                Button0_0.Background = Button1_1.Background = Button2_2.Background = Brushes.Green;
            }
            else if (mResults[2] != MarkType.Free && (mResults[2] & mResults[4] & mResults[6]) == mResults[2])
            {
                // Game has ended
                mGameEnded = true;

                // Highlight winning cells in green
                Button0_2.Background = Button1_1.Background = Button2_0.Background = Brushes.Green;
            }

            // Check for no winner and full board
            if (!mResults.Any(result => result == MarkType.Free))
            {
                // Game ended
                mGameEnded = true;

                // Trurn all cells orange
                Container.Children.Cast<Button>().ToList().ForEach(button =>
                {
                    button.Background = Brushes.Orange;
                });
            }
        }

        /// <summary>
        /// Evaluate score of current board (used for min-maxing)
        /// </summary>
        private int EvaluateBoard()
        {
            var mResultsEvalute = new MarkType[9];

            // Check for horizontal win
            for (int index = 0; index < 3; index++)
            {
                if (mResultsEvalute[0 + (index * 3)] != MarkType.Free && (mResultsEvalute[0 + (index * 3)] & mResultsEvalute[1 + (index * 3)] & mResultsEvalute[2 + (index * 3)]) == mResultsEvalute[0 + (index * 3)])
                {
                    if (mResultsEvalute[0 + (index * 3)] == MarkType.Nought)
                        return +10;
                    else if (mResultsEvalute[0 + (index * 3)] == MarkType.Cross)
                        return -10;
                }
            }

            // Check for vertical win
            for (int index = 0; index < 3; index++)
            {
                if (mResultsEvalute[0 + (index)] != MarkType.Free && (mResultsEvalute[0 + (index)] & mResultsEvalute[3 + (index)] & mResultsEvalute[6 + (index)]) == mResultsEvalute[0 + (index)])
                {
                    if (mResultsEvalute[0 + (index)] == MarkType.Nought)
                        return +10;
                    else if (mResultsEvalute[0 + (index)] == MarkType.Cross)
                        return -10;
                }
            }

            // Check for diagonal win
            if (mResultsEvalute[0] != MarkType.Free && (mResultsEvalute[0] & mResultsEvalute[4] & mResultsEvalute[8]) == mResultsEvalute[0])
            {
                if (mResultsEvalute[0] == MarkType.Nought)
                    return +10;
                else if (mResultsEvalute[0] == MarkType.Cross)
                    return -10;
            }
            else if (mResultsEvalute[2] != MarkType.Free && (mResultsEvalute[2] & mResultsEvalute[4] & mResultsEvalute[6]) == mResultsEvalute[2])
            {
                if (mResultsEvalute[2] == MarkType.Nought)
                    return +10;
                else if (mResultsEvalute[2] == MarkType.Cross)
                    return -10;
            }

            // If no winning/losing position is found return 0
            return 0;
        }
    }
}
