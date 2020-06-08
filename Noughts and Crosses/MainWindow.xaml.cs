using System;
using System.Linq;
using System.Threading.Tasks;
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
        private MarkType[,] mResults;

        /// <summary>
        /// True if it is player 1's turn (X) or player 2's turn (O)
        /// </summary>
        private bool mPlayer1Turn;

        /// <summary>
        /// True if the game has ended
        /// </summary>
        private bool mGameEnded;

        /// <summary>
        /// True if AI should move first
        /// </summary>
        private bool mAIFirst = false;

        /// <summary>
        /// Holds the information about which game is being played (PVP or PVE etc)
        /// </summary>
        private GameTypes mGameType = GameTypes.PlayerVersusPlayer;
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            NewGameAsync();
        }

        #endregion

        /// <summary>
        /// Starts a new game and clears values back to default
        /// </summary>
        private async Task NewGameAsync()
        {
            // Create a new blank array of free cells
            mResults = new MarkType[3, 3];

            for (int i = 0; i < mResults.GetLength(0); i++)
            {
                for (int j = 0; j < mResults.GetLength(1); j++)
                {
                    mResults[i, j] = MarkType.Free;
                }
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

            if (mAIFirst)
            {
                Move bestMove = FindBestMove(mResults, true);
                mResults[bestMove.col, bestMove.row] = MarkType.Nought;
                var AIMoveButton = Container.Children.Cast<Button>().First(ButtonToMove => Grid.GetRow(ButtonToMove) == bestMove.row && Grid.GetColumn(ButtonToMove) == bestMove.col);
                AIMoveButton.Foreground = Brushes.Red;
                AIMoveButton.Content = "O";
            }
            else if (mGameType == GameTypes.AIVersusAI)
            {
                while (!mGameEnded)
                {
                    Move bestMove = FindBestMove(mResults, !mPlayer1Turn);
                    mResults[bestMove.col, bestMove.row] = !mPlayer1Turn ? MarkType.Nought : MarkType.Cross;
                    var AIMoveButton = Container.Children.Cast<Button>().First(ButtonToMove => Grid.GetRow(ButtonToMove) == bestMove.row && Grid.GetColumn(ButtonToMove) == bestMove.col);
                    if (!mPlayer1Turn)           
                        AIMoveButton.Foreground = Brushes.Red;
                    AIMoveButton.Content = mPlayer1Turn ? "X" : "O";

                    mPlayer1Turn = !mPlayer1Turn;
                    CheckForWinner();
                    await Task.Delay(500);
                }
            }
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
                NewGameAsync();
                return;
            }

            // Cast sender to a button
            var button = (Button)sender;

            // Find button position in array
            var column = Grid.GetColumn(button);
            var row = Grid.GetRow(button);

            // Return if cell isn't free
            if (mResults[column, row] != MarkType.Free)
                return;

            // Set the cell value based on turn
            mResults[column, row] = mPlayer1Turn ? MarkType.Cross : MarkType.Nought;

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
            if (!mPlayer1Turn && !mGameEnded && mGameType == GameTypes.PlayerVersusAI)
            {
                Move bestMove = FindBestMove(mResults, true);
                mResults[bestMove.col, bestMove.row] = MarkType.Nought;
                var AIMoveButton = Container.Children.Cast<Button>().First(ButtonToMove => Grid.GetRow(ButtonToMove) == bestMove.row && Grid.GetColumn(ButtonToMove) == bestMove.col);
                AIMoveButton.Foreground = Brushes.Red;
                AIMoveButton.Content = "O";
                mPlayer1Turn = !mPlayer1Turn;

                CheckForWinner();
            }
        }

        /// <summary>
        /// Check if there is a winner
        /// </summary>
        private void CheckForWinner()
        {
            // Check for horizontal win
            for (int row = 0; row < 3; row++)
            {
                if (mResults[0, row] != MarkType.Free && (mResults[0, row] & mResults[1, row] & mResults[2, row]) == mResults[0, row])
                {
                    // Game has ended
                    mGameEnded = true;

                    // Highlight winning cells in green
                    switch (row)
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
            for (int column = 0; column < 3; column++)
            {
                if (mResults[column, 0] != MarkType.Free && (mResults[column, 0] & mResults[column, 1] & mResults[column, 2]) == mResults[column, 0])
                {
                    // Game has ended
                    mGameEnded = true;

                    // Highlight winning cells in green
                    switch (column)
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
            if (mResults[0, 0] != MarkType.Free && (mResults[0, 0] & mResults[1, 1] & mResults[2, 2]) == mResults[0, 0])
            {
                // Game has ended
                mGameEnded = true;

                // Highlight winning cells in green
                Button0_0.Background = Button1_1.Background = Button2_2.Background = Brushes.Green;
            }
            else if (mResults[2, 0] != MarkType.Free && (mResults[2, 0] & mResults[1, 1] & mResults[0, 2]) == mResults[2, 0])
            {
                // Game has ended
                mGameEnded = true;

                // Highlight winning cells in green
                Button0_2.Background = Button1_1.Background = Button2_0.Background = Brushes.Green;
            }

            // Check for no winner and full board
            if (!mResults.Cast<MarkType>().Any(result => result == MarkType.Free))
            {
                // Game ended
                mGameEnded = true;

                // Turn all cells orange
                Container.Children.Cast<Button>().ToList().ForEach(button =>
                {
                    button.Background = Brushes.Orange;
                });
            }
        }

        /// <summary>
        /// Evaluate score of current board (used for min-maxing)
        /// </summary>
        private int EvaluateBoard(MarkType[,] board, bool IsNought)
        {
            // Check for horizontal win
            for (int row = 0; row < 3; row++)
            {
                if (board[0, row] != MarkType.Free && (board[0, row] & board[1, row] & board[2, row]) == board[0, row])
                {
                    if ((board[0, row] == MarkType.Nought && IsNought) || (board[0,row] == MarkType.Cross && !IsNought))
                        return +10;
                    else if ((board[0, row] == MarkType.Cross && IsNought) || (board[0,row] == MarkType.Nought && !IsNought))
                        return -10;
                }
            }

            // Check for vertical win
            for (int column = 0; column < 3; column++)
            {
                if (board[column, 0] != MarkType.Free && (board[column, 0] & board[column, 1] & board[column, 2]) == board[column, 0])
                {
                    if ((board[column, 0] == MarkType.Nought && IsNought) || (board[column, 0] == MarkType.Cross && !IsNought))
                        return +10;
                    else if ((board[column, 0] == MarkType.Cross && IsNought) || (board[column, 0] == MarkType.Nought && !IsNought))
                        return -10;
                }
            }

            // Check for diagonal win
            if (board[0,0] != MarkType.Free && (board[0,0] & board[1, 1] & board[2, 2]) == board[0,0])
            {
                if ((board[0, 0] == MarkType.Nought && IsNought) || (board[0, 0] == MarkType.Cross && !IsNought))
                    return +10;
                else if ((board[0, 0] == MarkType.Cross && IsNought) || (board[0, 0] == MarkType.Nought && !IsNought))
                    return -10;
            }
            else if (board[2,0] != MarkType.Free && (board[2,0] & board[1,1] & board[0,2]) == board[2,0])
            {
                if ((board[2, 0] == MarkType.Nought && IsNought) || (board[2, 0] == MarkType.Cross && !IsNought))
                    return +10;
                else if ((board[2, 0] == MarkType.Cross && IsNought) || (board[2, 0] == MarkType.Nought && !IsNought))
                    return -10;
            }

            // If no winning/losing position is found return 0
            return 0;
        }

        /// <summary>
        /// Mini max algorithm to determine best move by simulating moves on board and evaluating outcomes
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <param name="IsMax"></param>
        /// <returns></returns>
        private int MiniMax(MarkType[,] board, int depth, bool IsMax, bool IsNought)
        {
            int score = EvaluateBoard(board, !mPlayer1Turn);

            // Return score if maximizer has won
            if (score == 10)
                return score;

            // Return score if minimizer has won
            if (score == -10)
                return score;

            // Return 0 if no moves left and no winner
            if (!mResults.Cast<MarkType>().Any(result => result == MarkType.Free))
            {
                return 0;
            }

            // Check if maximizers move
            if (IsMax)
            {
                int best = -1000;

                // Traverse all cells
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Check if cell is empty
                        if (board[i, j] == MarkType.Free)
                        {
                            // Simulate move
                            board[i, j] = IsNought ? MarkType.Nought : MarkType.Cross;

                            // Call minimax recursively and choose maximum value
                            best = Math.Max(best, MiniMax(board, depth + 1, !IsMax, IsNought));

                            // Undo move
                            board[i, j] = MarkType.Free;
                        }
                    }
                }
                return best;
            }
            // Minimizers move
            else
            {
                int best = 1000;

                // Traverse all cells
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Check if cell is empty
                        if (board[i,j] == MarkType.Free)
                        {
                            // Simulate move
                            board[i, j] = IsNought ? MarkType.Cross : MarkType.Nought;

                            // Call minimax recursively and choose minimum value
                            best = Math.Min(best, MiniMax(board, depth + 1, !IsMax, IsNought));

                            // Undo move
                            board[i, j] = MarkType.Free;
                        }
                    }
                }
                return best;
            }
        }

        /// <summary>
        /// Finds the best move
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private Move FindBestMove(MarkType[,] board, bool IsNought)
        {
            int bestVal = -1000;
            Move bestMove = new Move
            {
                col = -1,
                row = -1
            };


            // Traverse all cells, evaluate minimax for empty cells then return optimal value
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i,j] == MarkType.Free)
                    {
                        board[i, j] = IsNought ? MarkType.Nought : MarkType.Cross;

                        int moveVal = MiniMax(board, 0, false, !mPlayer1Turn);

                        board[i, j] = MarkType.Free;

                        if (moveVal > bestVal)
                        {
                            bestMove.col = i;
                            bestMove.row = j;
                            bestVal = moveVal;
                        }
                    }
                }
            }

            return bestMove;
        }
        /// <summary>
        /// Event ran when combo box for game type changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = (ComboBox)sender;

            mGameType = (GameTypes)combobox.SelectedIndex;
            NewGameAsync();
        }

        /// <summary>
        /// AI Toggle checkbox code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AIToggle_Changed(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            mAIFirst = (bool)checkbox.IsChecked;
            NewGameAsync();
        }
    }
}
