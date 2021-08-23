//this script populates the UI with the information from the player's party.
//it also handles any UI events related to switching or sending out a new monster.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadP1MonstersDynamic : MonoBehaviour
{
    public combatController combatController;

    //UI BUTTONS
    [Header("UI Buttons")]
    public GameObject SwitchButtonText;
    public GameObject MonBtnList;
    public GameObject MonBtn1;
    public GameObject MonBtn2;
    public GameObject MonBtn3;
    public GameObject MonBtn4;
    public GameObject MonBtn5;
    public GameObject MonBtn6;



    private Sprite[] categsprites;
    private List<Monster> party;

    private List<GameObject> btnListAux;

    void Awake()
    {
        btnListAux = new List<GameObject>() { MonBtn1, MonBtn2, MonBtn3, MonBtn4, MonBtn5, MonBtn6 };
    }

    // Start is called before the first frame update
    void Start(){}

    public void LoadMonstersIntoUI()
    {

        //in here we get the monsters from the player's party and make the buttons match them.
        //We add a listener to each button
        party = combatController.getP1Party();
       
        int ind = 0;
        foreach (GameObject button in btnListAux)
        {  
            Monster p1Mon = combatController.player1Monster;
            Monster current = party[ind];
            if (current != null)
            {
                button.GetComponentInChildren<Text>().text = current.name;
                button.GetComponentInChildren<Button>().onClick.AddListener(delegate { 
                    turnQueuer(new TurnAction(current, combatController.player1Monster));
                });

                if (current.currentHP == 0 || current == combatController.player1Monster)
                {
                    button.GetComponentInChildren<Button>().interactable = false;
                    if (current.currentHP == 0)
                    {
                        button.GetComponentInChildren<Text>().text += "[FNT]";
                    }
                }
                else
                {
                    button.GetComponentInChildren<Button>().interactable = true;
                }
                if (current.type1 == MonsterList.monsterNone.type1) //type1 should never be null so this is a safe comparison
                {
                    button.SetActive(false);
                }
                else
                {
                    button.SetActive(true);
                }
            } else { button.SetActive(false); }

            ind++;
        }

    }

    private void turnQueuer(TurnAction playerAction)
    {
        //check if the player is swapping an existing monster or sending out a new one
        if(combatController.player1Monster == null)
        {
            LoadMonstersIntoUI();
            StartCoroutine(combatController.SummonNewMon(playerAction.switchMonster, 1));
        }
        else
        {
            //this gets both moves from each monster and begins the turn
            //the next 3 lines of code are to be replaced whenever netcode is implemented.
            List<Move> enemyMoveList = combatController.getP2Moves();
            Move enemyMove = enemyMoveList[Random.Range(0, enemyMoveList.Count - 1)];


            StartCoroutine(combatController.ExecuteTurn(playerAction, new TurnAction(enemyMove, combatController.player2Monster, combatController.player1Slot1)));
           
        }
        ToggleMonsterList();
    }

    // Update is called once per frame
    void Update()
    {
        if(combatController.reloadUI){
            LoadMonstersIntoUI();
        }
            
        if(combatController.isTurnInProgress){
            GetComponent<Button>().interactable = false;
        }else{
            GetComponent<Button>().interactable = true;
        }
    }

    /* 
    private Sprite spriteByType(Type type){ //returns the different symbol images for every type from the spritesheet. This will eventually be changed when the UI is made prettier
       return typesprites[(int) type]; //can't believe this works
    } 
    */


    public void ToggleMonsterList()
    { //this is behavior for the "Switch" button
        if (MonBtnList.activeSelf)
        {
            MonBtnList.SetActive(false);
            SwitchButtonText.GetComponentInChildren<Text>().text = "Switch";
        }
        else
        {
            MonBtnList.SetActive(true);
            SwitchButtonText.GetComponentInChildren<Text>().text = "Back";
        }
    }

    public void DisableMonsterList()
    { 
        if (MonBtnList.activeSelf)
        {
            MonBtnList.SetActive(false);
            SwitchButtonText.GetComponentInChildren<Text>().text = "Switch";
        }
    }

}
