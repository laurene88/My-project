using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GM : MonoBehaviour
{
    public Player currentPlayer = Player.TWO;
    //enums - essentially named numbers
    public enum Player {ONE, TWO};

    public Image p1;
    public Image p2;
    public Color p1Color;
    public Color p1ColorOff;
    public Color p2Color;
    public Color p2ColorOff;
    public int p1score = 0;
    public int p2score = 0;

    public Image p1scoreImage;
    public Image p2scoreImage;

    public Sprite tally1;
    public Sprite tally2;
    public Sprite tally3;
    public Sprite tally4;
    public Sprite tally5;

    // Start is called before the first frame update
    void Start()
    {
        SetupGameSeries();
    }


    public void SetupGameSeries(){
        //Set marker image colors
        p1.color = p1Color;
        p2.color = p2Color;
        //Hide score images to start, set same color as player.
        p1scoreImage.enabled = false;
        p2scoreImage.enabled = false;        
        p1scoreImage.color = p1Color;
        p2scoreImage.color = p2Color;
        // Reset zero scores & score images for a zero game.
        p1score = 0;
        p2score = 0;
        UpdateScoreImages(p1score, p1scoreImage);
        UpdateScoreImages(p2score, p2scoreImage);
        NextPlayersTurn();   
    }


    public void NextPlayersTurn(){
        if (currentPlayer == Player.ONE) {
            //Debug.Log("next is two");
            SetPlayerTwo();
        } else { 
            //Debug.Log("next is one");
            SetPlayerOne();
        }
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


    public void AddToScore(){
        if (currentPlayer == Player.ONE) {
            p1score += 1;
        } else { 
            p2score += 1;
        }
        UpdateScoreImages(p1score, p1scoreImage);
        UpdateScoreImages(p2score, p2scoreImage);
        CheckHasWonFiveGames();
    }


    public void UpdateScoreImages(int score, Image i){
        Sprite chosenScoreImage = DecideScoreImage(score);
        if (chosenScoreImage != null){
            i.sprite = chosenScoreImage;
            i.enabled = true;
        }
    }

    public Sprite DecideScoreImage(int score){
        switch(score)
        {
            case 1:
                return tally1;
            case 2:
                return tally2;
            case 3:
                return tally3;
            case 4:
                return tally4;
            case 5:
                return tally5;
            default:
                return null;
        }
    }

    public void CheckHasWonFiveGames()
    {
        if (p1score == 5){
            FiveGamesWon(Player.ONE);
        }
        if (p2score == 5){
            FiveGamesWon(Player.TWO);
        }
    }


    public void FiveGamesWon(Player player){
        Debug.Log("WINNER:"+player);
        SetPlayersOff();
    }

}
