using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM : MonoBehaviour
{
    public Player currentPlayer = Player.ONE;
    //enums - essentially named numbers
    public enum Player {ONE, TWO};

    public Image p1;
    public Image p2;

    // Start is called before the first frame update
    void Start()
    {
        SetPlayerOne();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextPlayersTurn(){
        if (currentPlayer == Player.ONE) {
            SetPlayerTwo();
        } else { SetPlayerOne();
        }
        Debug.Log(currentPlayer);

    }

    private void SetPlayerOne(){
        currentPlayer = Player.ONE;
        p1.color = Color.yellow;
        p2.color = Color.black;
        
    }

    private void SetPlayerTwo(){
        currentPlayer = Player.TWO;
        p1.color = Color.black;
        p2.color = Color.yellow;
    }
}
