using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row : MonoBehaviour
{
    //keep array of tiles.

    public Tile[] tiles { get; private set;}
    // Start is called before the first frame update

    // method to get complete word from row of tiles.
    // public string word
    // {
    //     get
    //     {
    //         string word = "";
    //         for (int i = 0; i < tiles.Length; i++)
    //         {
    //             word += tiles[i].letter;
    //         }
    //         return word;
    //     }
    // }
    
   public void Awake(){
    tiles = GetComponentsInChildren<Tile>();
   }
}
