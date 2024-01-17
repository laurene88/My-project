using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    //normally he would make this a struct, 
    //but in future need to check & use a class..?
    // declaring state in board instead as consistent over entire game.
    [System.Serializable]
    public class State
    {
        public Color fillColor; 
        public Color outlineColor;
        public Color textColor;
    }

    //public getter, private setter
    public char digit { get; private set;}
    public State state { get; private set;}

    private TextMeshProUGUI text;
    private Image fill;
    private Outline outline;


    private void Awake(){
    // reference to text component.
    text = GetComponentInChildren<TextMeshProUGUI>();
    fill = GetComponent<Image>();
    outline = GetComponent<Outline>();
    }

    public void SetDigit(char n){
        this.digit = n;
        text.text = n.ToString();
    }

    public void SetState(State state){
        this.state = state;
        fill.color = state.fillColor;
        outline.effectColor = state.outlineColor;
        text.color = state.textColor;
    }
}
