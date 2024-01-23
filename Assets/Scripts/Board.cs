using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[]{
        // KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,  KeyCode.F, 
        // KeyCode.G,  KeyCode.H, KeyCode.I, KeyCode.J,  KeyCode.K,  KeyCode.L, 
        // KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q,  KeyCode.R, 
        // KeyCode.S,  KeyCode.T, KeyCode.U,  KeyCode.V,  KeyCode.W, KeyCode.X, 
        // KeyCode.Y, KeyCode.Z, 
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, 
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, 
        KeyCode.Alpha9, KeyCode.Alpha0
        };


    private string answerNumber;
    private bool flippingTiles = false;

    private Row[] rows;
    private int rowIndex;
    private int colIndex;


    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;
    
    [Header("UI references")]
    public TextMeshProUGUI invalidWordText;
    public Button tryAgainButton;
    public Button newGameButton;


    public GameObject gm; 

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
        //TODO put GM here.
    }

    // Update is called once per frame

    private void Start()
    {
        SetSingleGame();
    }


    public void SetSingleGame(){
        ClearBoard();
        setRandomNumber();
        enabled = true;
    }


    //keep scores, get next number, next player has first go.
    // IS THIS THE BEST GAME PLAY IDEA THOUGH?? EACH WINNER THEN GETS 2ND GO EACH TIME.
    public void NewGame()
    {   
        SetSingleGame();
        gm.GetComponent<GM>().NextPlayersTurn();
    }

    //reset best of 5 games.
    public void ResetGame()
    {
        ClearBoard();
        setRandomNumber();
        gm.GetComponent<GM>().SetupGameSeries();
        enabled = true;
    }


    private void setRandomNumber(){
        answerNumber = Random.Range(10000,100000).ToString();
    }

    private void Update()
    {
        Row currentRow = rows[rowIndex];

        if (flippingTiles == false){
            if (Input.GetKeyDown(KeyCode.Backspace)){
                // need to delete index first to go back, as cursor auto moves.
                // will choose max of the two. so clamps on lower bound, will always take 0.
                // Clamp function is double bounding in one function call.
                // so doesnt go into negatives/errors.
                colIndex = Mathf.Max(colIndex -1 , 0);
                currentRow.tiles[colIndex].SetState(emptyState);
                currentRow.tiles[colIndex].SetDigit('\0');

                invalidWordText.gameObject.SetActive(false);

            }

            else if (colIndex >= currentRow.tiles.Length){
                // if reaches the end of the row & clicks enter, submit
                if (Input.GetKeyDown(KeyCode.Return)){
                    SubmitRow(currentRow);
                }
            }

            else
            { 
                for (int i = 0; i < SUPPORTED_KEYS.Length; i++){
                if (Input.GetKeyDown(SUPPORTED_KEYS[i])){
                    currentRow.tiles[colIndex].SetDigit((char)SUPPORTED_KEYS[i]);
                    currentRow.tiles[colIndex].SetState(occupiedState);
                    colIndex++;
                    break;
                    }
                }   
            }
        }
    }


    private void SubmitRow(Row row)
    {
        //starting with a simple logic. We will later do edge cases.
        //compare every tile w the digit in the answerNumber
        // Edge cases about repeat letters/if only 1 but guessed twice.
        // my suggestion move between two arrays?
        //OLD WAY:
        //  for (int i = 0; i < row.tiles.Length ; i++){
        //      Tile tile = row.tiles[i];
        //      if (tile.digit == answerNumber[i]){
        //          tile.SetState(correctState);
        //      } else if (answerNumber.Contains(tile.digit.ToString())){
        //             tile.SetState(wrongSpotState);
        //      } else {
        //          tile.SetState(incorrectState);
        //      }
        //  }

        string remaining = answerNumber;

        // For loop 1, check if correct or incorrect.
        for (int i = 0; i < row.tiles.Length; i++){
            Tile tile = row.tiles[i];
            if (tile.digit == answerNumber[i]){
                tile.SetState(correctState);

                // remove at index i, 1 thing
                remaining = remaining.Remove(i,1);
                // to keep length the same, insert a space.
                remaining = remaining.Insert(i," ");
            }
            else if (!answerNumber.Contains(tile.digit.ToString())){
                tile.SetState(incorrectState);

            }
        }
        // For loop 2, check remaining letters to see if in right spot or not.
        for (int i = 0; i < row.tiles.Length; i++){
            Tile tile = row.tiles[i];

            // this is why we wanted class not a struct (object referencing)
            // as cant compare struct things so easily, would have to make
            // own comparable method.
            if (tile.state != correctState && tile.state != incorrectState){
                //check if remaining answerNumber contains digit we know theres a 2nd instance
                if (remaining.Contains(tile.digit.ToString())){
                    //2nd of that digit
                    tile.SetState(wrongSpotState);

                    //find actual digit in answerNumber & remove it.
                    int index = remaining.IndexOf(tile.digit);
                    remaining = remaining.Remove(index,1);
                    remaining = remaining.Insert(index," ");
                }
                else
                {   //only 1, 2nd guess is wrong.
                    tile.SetState(incorrectState);
                }
            }
        }

        StartCoroutine(FlipTiles(row)); 
    }


    //TODO ANIM DOESNT WORK ON SECOND GAME??
    IEnumerator FlipTiles(Row row){
        flippingTiles = true;
        //Debug.Log("im flipping tiles");
        for (int i = 0; i < row.tiles.Length; i++){
            row.tiles[i].RotateAnimation();
            row.tiles[i].ChangeState();
            yield return new WaitForSeconds(.5f);
        }
        flippingTiles = false;
        AfterRowSubmission(row);
    }


    public void AfterRowSubmission(Row row){
        //check if has won
        if (HasWon(row)){
            gm.GetComponent<GM>().AddToScore();
            enabled = false;
            gm.GetComponent<GM>().SetPlayersOff();
        } else { //if not keep progressing
            rowIndex++;
            colIndex = 0;
             if (rowIndex >= rows.Length){ // then check if have lost
                enabled = false;
                gm.GetComponent<GM>().SetPlayersOff();
            } else { // has not won or lost, keep playing
                 gm.GetComponent<GM>().NextPlayersTurn();
            }
        }
    }

    private bool HasWon(Row row)
    {
        for (int i = 0; i < row.tiles.Length; i++)
        {
            if (row.tiles[i].state != correctState){
                return false;
            }
        }
        return true;
    }

    public void ClearBoard()
    {
        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].tiles.Length; col++)
            {
                rows[row].tiles[col].SetDigit('\0');
                rows[row].tiles[col].SetState(emptyState);
                rows[row].tiles[col].ChangeState();

            }
        }
        rowIndex = 0;
        colIndex = 0;
    }


    private void OnEnable(){
        tryAgainButton.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(false);
    }

    private void OnDisable(){
        tryAgainButton.gameObject.SetActive(true);
        newGameButton.gameObject.SetActive(true);
    }

}