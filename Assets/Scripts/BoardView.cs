using UnityEngine;

public class BoardView : MonoBehaviour
{
    //create board array
    int[,] board = new int[8, 8];
    //1 black 2 white
    public int currentPlayer = 1;
    public GameObject squarePrefab;
    public GameObject discPrefab;
    public GameObject discPrefab2;
    //public playerturn playerturn;
    void Start()
    {
        Setup();
        Draw();
        //playerturn.EndTurn();
    }
    private void Update()
    {
        
    }
    void Draw()
    {
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                Vector3 position = new Vector3(col, -row, 0);
                Instantiate(squarePrefab, position, Quaternion.identity);
                if (board[row,col] == 1)
                {
                    Instantiate(discPrefab, position, Quaternion.identity);
                }
                if (board[row,col] == 2)
                {
                    Instantiate(discPrefab2, position, Quaternion.identity);
                }
            }
        }
    }
    public void Setup()
    {
        board[3, 3] = 2; 
        board[4, 4] = 2; 
        board[3, 4] = 1; 
        board[4, 3] = 1; 
    }
}