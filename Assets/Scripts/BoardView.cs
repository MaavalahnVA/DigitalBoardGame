using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class BoardView : MonoBehaviour
{
    // 0 = empty, 1 = black, 2 = white
    int[,] board = new int[8, 8];

    // Black starts first
    public int currentPlayer = 1;

    // Prefabs assigned in the Inspector
    public GameObject squarePrefab;
    public GameObject discPrefab;
    public GameObject discPrefab2;

    // Store the squares and pieces
    GameObject[,] squares = new GameObject[8, 8];
    GameObject[,] discs = new GameObject[8, 8];

    Color[,] squareColors = new Color[8, 8];

    // Stop players from moving when the game ends
    bool gameOver = false;

    void Start()
    {
        Setup();
        Draw();
        HighlightMoves();
    }

    void Update()
    {
        if (gameOver || Mouse.current == null)
        {
            return;
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Convert the mouse position to world coordinates
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            int col = Mathf.RoundToInt(mousePosition.x);
            int row = Mathf.RoundToInt(-mousePosition.y);

            // Check whether the click is on the board
            if (OnBoard(row, col))
            {
                MakeMove(row, col);
            }
        }
    }

    // Set up the four starting pieces
    public void Setup()
    {
        board[3, 3] = 2;
        board[4, 4] = 2;
        board[3, 4] = 1;
        board[4, 3] = 1;
    }

    // Create the board and starting pieces
    void Draw()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Vector3 position = new Vector3(col, -row, 0);

                // Create a square
                squares[row, col] = Instantiate(squarePrefab, position, Quaternion.identity);

                SpriteRenderer renderer = squares[row, col].GetComponent<SpriteRenderer>();

                squareColors[row, col] = renderer.color;

                if (board[row, col] != 0)
                {
                    DrawDisc(row, col);
                }
            }
        }
    }

    // Create or replace a helmet
    void DrawDisc(int row, int col)
    {
        // Remove the old helmet if there is one
        if (discs[row, col] != null)
        {
            Destroy(discs[row, col]);
        }

        // Position the helmet slightly in front of the square
        Vector3 position = new Vector3(col, -row, -1f);

        // Create a Black helmet
        if (board[row, col] == 1)
        {
            discs[row, col] = Instantiate(discPrefab, position, Quaternion.identity);
        }
        // Create a White helmet
        else if (board[row, col] == 2)
        {
            discs[row, col] = Instantiate(discPrefab2, position, Quaternion.identity);
        }
    }

    // Attempt to place a helmet on a square
    void MakeMove(int row, int col)
    {
        // Cannot place a helmet on an occupied square
        if (board[row, col] != 0)
        {
            return;
        }
        // Find all opponent helmets that would flip
        List<Vector2Int> flips = GetFlips(row, col);

        if (flips.Count == 0)
        {
            return;
        }
        // Place the new helmet
        board[row, col] = currentPlayer;
        DrawDisc(row, col);

        // Flip all captured opponent helmets
        foreach (Vector2Int piece in flips)
        {
            board[piece.x, piece.y] = currentPlayer;

            DrawDisc(piece.x, piece.y);
        }

        // Switch to the other player
        EndTurn();

        // Skip the other player if they cannot move
        if (!HasMove())
        {
            EndTurn();
        }

        // If neither player can move, end the game
        if (!HasMove())
        {
            gameOver = true;
            Debug.Log("Game Over!");

            // Display the final scores
            CountScore();
        }
        else
        {
            Debug.Log("Player " + currentPlayer + "'s turn");
        }

        // Update the available moves
        HighlightMoves();
    }

    // Find all opponent helmets that a move would flip
    List<Vector2Int> GetFlips(int row, int col)
    {
        List<Vector2Int> flips = new List<Vector2Int>();

        // Cannot place a helmet on an occupied square
        if (board[row, col] != 0)
        {
            return flips;
        }

        // Find the opponent
        int opponent;

        if (currentPlayer == 1)
        {
            opponent = 2;
        }
        else
        {
            opponent = 1;
        }

        // All eight directions
        int[,] directions =
        {
            { -1, 0 },  
            { 1, 0 },   
            { 0, -1 }, 
            { 0, 1 },   
            { -1, -1 }, 
            { -1, 1 },  
            { 1, -1 }, 
            { 1, 1 }    
        };

        // Check each direction
        for (int i = 0; i < 8; i++)
        {
            int rowDirection = directions[i, 0];
            int colDirection = directions[i, 1];

            int r = row + rowDirection;
            int c = col + colDirection;

            List<Vector2Int> possible = new List<Vector2Int>();

            // Find opponent helmets in this direction
            while (OnBoard(r, c) && board[r, c] == opponent)
            {
                possible.Add(new Vector2Int(r, c));

                r += rowDirection;
                c += colDirection;
            }

            // Check if our helmet is at the end
            if (possible.Count > 0 &&
                OnBoard(r, c) &&
                board[r, c] == currentPlayer)
            {
                flips.AddRange(possible);
            }
        }

        return flips;
    }

    // Check whether a position is inside the board
    bool OnBoard(int row, int col)
    {
        return row >= 0 && row < 8 && col >= 0 && col < 8;
    }

    // Check whether the current player has any legal moves
    bool HasMove()
    {
        // Check all 64 squares
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                // A legal move must flip at least one helmet
                if (GetFlips(row, col).Count > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Switch between Players
    void EndTurn()
    {
        if (currentPlayer == 1)
        {
            currentPlayer = 2;
        }
        else
        {
            currentPlayer = 1;
        }
    }

    // Highlight all available moves
    void HighlightMoves()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                SpriteRenderer renderer = squares[row, col].GetComponent<SpriteRenderer>();

                // Check whether this is a legal move
                bool valid = GetFlips(row, col).Count > 0;

                if (valid && !gameOver)
                {
                    // Highlight legal moves green
                    renderer.color = Color.green;
                }
                else
                {
                    renderer.color = squareColors[row, col];
                }
            }
        }
    }

    // Count the helmets when the game ends
    void CountScore()
    {
        int black = 0;
        int white = 0;

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if (board[row, col] == 1)
                {
                    black++;
                }
                if (board[row, col] == 2)
                {
                    white++;
                }
            }
        }

        // Print the final scores
        Debug.Log("Black: " + black + " White: " + white);

        if (black > white)
        {
            Debug.Log("Black Wins!");
        }
        else if (white > black)
        {
            Debug.Log("White Wins!");
        }
        else
        {
            Debug.Log("It's a Tie!");
        }
    }
}