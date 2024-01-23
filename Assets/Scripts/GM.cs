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
    public Color p1Color;
    public Color p1ColorOff;
    public Color p2Color;
    public Color p2ColorOff;


    // Start is called before the first frame update
    void Start()
    {
        p1Color.a = 1;
        p2Color.a = 1;
        p1.color = p1Color;
        p2.color = p2Color;
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
        p1.color = p1Color;
        p2.color = p2ColorOff;
    }

    private void SetPlayerTwo(){
        currentPlayer = Player.TWO;
        p2.color = p2Color;
        p1.color = p1ColorOff;
    }

    //TODO this needs to be public to be called atm?
    public void SetPlayersOff(){
        p1.color = p1ColorOff;
        p2.color = p2ColorOff;
    }
}
