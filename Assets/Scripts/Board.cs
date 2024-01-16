using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[]{
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,  KeyCode.F, 
        KeyCode.G,  KeyCode.H, KeyCode.I, KeyCode.J,  KeyCode.K,  KeyCode.L, 
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q,  KeyCode.R, 
        KeyCode.S,  KeyCode.T, KeyCode.U,  KeyCode.V,  KeyCode.W, KeyCode.X, 
        KeyCode.Y, KeyCode.Z
    };

    private string[] solutions;
    private string[] validWords; //valid words you can guess.
    private string word;

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


    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();

    }
    // Update is called once per frame

    private void Start()
    {
        LoadData();
        NewGame();
    }


    public void NewGame()
    {
        ClearBoard();
        setRandomWord();
        enabled = true;

    }

    public void TryAgain()
    {
        ClearBoard();
        //keep same word
        enabled = true;
        
    }

    private void LoadData()
    {
        //get textfile from resources folder.
        TextAsset textFile = Resources.Load("Official_wordle_all") as TextAsset;
        validWords = textFile.text.Split('\n'); //read whole doc, splitting on enters
    
        //just reassigning the same variable
        textFile = Resources.Load("Official_wordle_common") as TextAsset;
        solutions = textFile.text.Split('\n'); //read whole doc, splitting on enters
    }


    private void setRandomWord()
    {
        word = solutions[Random.Range(0, solutions.Length)];
        // as safety, make sure lower case & trimmed.
        word = word.ToLower().Trim();
    }

    private void Update()
    {
        Row currentRow = rows[rowIndex];

        if (Input.GetKeyDown(KeyCode.Backspace)){
            // need to delete index first to go back, as cursor auto moves.
            // will choose max of the two. so clamps on lower bound, will always take 0.
            // Clamp function is double bounding in one function call.
            // so doesnt go into negatives/errors.
            colIndex = Mathf.Max(colIndex -1 , 0);
            currentRow.tiles[colIndex].SetState(emptyState);
            currentRow.tiles[colIndex].SetLetter('\0');

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
                currentRow.tiles[colIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                currentRow.tiles[colIndex].SetState(occupiedState);
                colIndex++;
                break;
                }
            }   
        }
    }


    private void SubmitRow(Row row)
    {
        //starting with a simple logic. We will later do edge cases.
        //compare every tile w the letter in the word
        // Edge cases about repeat letters/if only 1 but guessed twice.
        // my suggestion move between two arrays?
        //OLD WAY:
        // for (int i = 0; i < row.tiles.Length ; i++){
        //     Tile tile = row.tiles[i];
        //     if (tile.letter == word[i]){
        //         tile.SetState(correctState);
        //     } else if (word.Contains(tile.letter.ToString())){
        //             tile.SetState(wrongSpotState);
        //     } else {
        //         tile.SetState(incorrectState);
        //     }
        // }

        if (!IsValidWord(row.word)){
            // if it is not valid, dont allow it. Need to feedback to user.
            invalidWordText.gameObject.SetActive(true);
            return;
        }

        string remaining = word;

    
        // For loop 1, check if correct or incorrect.
        for (int i = 0; i < row.tiles.Length; i++){
            Tile tile = row.tiles[i];
            if (tile.letter == word[i]){
                tile.SetState(correctState);
                // remove at index i, 1 thing
                remaining = remaining.Remove(i,1);
                // to keep length the same, insert a space.
                remaining = remaining.Insert(i," ");
            }
            else if (!word.Contains(tile.letter.ToString())){
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
                //check if remaining word contains letter we know theres a 2nd instance
                if (remaining.Contains(tile.letter.ToString())){
                    //2nd of that letter
                    tile.SetState(wrongSpotState);

                    //find actual letter in word & remove it.
                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index,1);
                    remaining = remaining.Insert(index," ");
                }
                else
                {   //only 1, 2nd guess is wrong.
                    tile.SetState(incorrectState);
                }
            }

        }

        if (HasWon(row)){
            enabled = false;
        }

        rowIndex++;
        colIndex = 0;

        if (rowIndex >= rows.Length){
            enabled = false;
            //just disable script when you reach the end so wont call update anymore.
        }
    }

    // check if word is in valid list, called from submit row.
    private bool IsValidWord(string word){
        for (int i = 0; i <validWords.Length; i++){
            if (word == validWords[i]){
                return true;
            }
            }
            return false;
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
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetState(emptyState);
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