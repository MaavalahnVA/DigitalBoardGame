using UnityEngine;

public class playerturn : MonoBehaviour
{
    public int EndTurn(int currentPlayer)
    {
       if (currentPlayer == 1) 
        { 
            return 2; 
        }
        else 
        { 
            return 1; 
        }
    }
}
// this script is 110% Redundant, more of a concept than anything, when you need to implement turn switching into the final code, just set it to invert depending on the number. this code will 