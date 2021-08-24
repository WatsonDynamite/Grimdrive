//this script fills up the buttons for the moves in the UI.
//it also handles any UI events related to attacking
//in this version, it also picks the enemy monster's move at random.
//that's as much AI as I feel like making atm


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadP1MovesDynamic : MonoBehaviour
{
    public combatController combatController;

    //UI BUTTONS
     [Header("UI Buttons")]
    public GameObject FightButtonText;
    public GameObject AtkBtnList;
    public GameObject AtkBtn1;
    public GameObject AtkBtn2;
    public GameObject AtkBtn3;
    public GameObject AtkBtn4;

    [Header("Tooltip Objects")]
    public GameObject MoveToolTip;

    
    private Sprite[] categsprites;
    private List<Move> moveList;

    void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start() 
    {  
       categsprites = Resources.LoadAll<Sprite>("UISprites/PhysSpecStaInd");
    }

    public void LoadMovesIntoUI(){
        this.moveList = combatController.getP1Moves();
        //in here we get the moves from the player's current active monster and make the buttons match them.
        //We add a listener to each button

        AtkBtnList.SetActive(false);
        GameObject[] buttons = {AtkBtn1, AtkBtn2, AtkBtn3, AtkBtn4};

        int i = 0;
        foreach (GameObject button in buttons)
        {
            var ButtonComponent = button.GetComponent<Button>();
            ButtonComponent.onClick.RemoveAllListeners();
            Move mv = moveList[i];
            TurnAction playerAction = new TurnAction(mv, combatController.player1Monster, combatController.player2Slot1);
            button.GetComponentInChildren<Text>().text = mv.name;
            button.GetComponentsInChildren<Image>()[1].sprite = TypeUtils.spriteByType(mv.type);
            ButtonComponent.onClick.AddListener(delegate { turnQueuer(playerAction); });
            ButtonComponent.onClick.AddListener(delegate { DisableMoveToolTip(); });
            ButtonComponent.interactable = (combatController.player1AP >= mv.cost);
            i++;
        }
    }

    private void turnQueuer(TurnAction playerAction)
    { 
        combatController.TurnQueuer(playerAction);
        DisableMoveList();
    }

    // Update is called once per frame
    void Update()
    {
        if(combatController.reloadUI){
            LoadMovesIntoUI();
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

    private Color colorByType(Type type){ //returns different colors for every type. This will eventually be changed when the UI is made prettier
        Color temp = new Color32(0, 0, 0, 255);
        switch(type){
            case Type.NONE: temp = new Color32(0, 0, 0, 255); break;
            case Type.WATER: temp = new Color32(74, 134, 232, 255); break;
            case Type.FIRE: temp = new Color32(255, 0, 0, 255); break;
            case Type.NATURE: temp = new Color32(0, 255, 0, 255); break;
            case Type.ICE: temp = new Color32(0, 255, 255, 255); break;
            case Type.ELECTRIC: temp = new Color32(255, 255, 0, 255); break;
            case Type.TOXIC: temp = new Color32(153, 0, 255, 255); break;
            case Type.SHADOW: temp = new Color32(32, 18, 77, 255); break;
            case Type.MIND: temp = new Color32(255, 0, 255, 255); break;
            case Type.LIGHT: temp = new Color32(255, 229, 153, 255); break;
            case Type.MARTIAL: temp = new Color32(166, 28, 0, 255); break;
            case Type.EARTH: temp = new Color32(133, 32, 12, 255); break;
            case Type.METAL: temp = new Color32(137, 137, 137, 255); break;
            case Type.WIND: temp = new Color32(164, 194, 244, 255); break;
            case Type.ARCANE: temp = new Color32(246, 178, 107, 255); break;
        }
        return temp;
    } 

    public void ToggleMoveList(){ //this is behavior for the "Fight" button
        if(AtkBtnList.activeSelf){
            AtkBtnList.SetActive(false);
            FightButtonText.GetComponentInChildren<Text>().text = "Fight";
        }else{
            AtkBtnList.SetActive(true);
            FightButtonText.GetComponentInChildren<Text>().text = "Back";
        }
    }

    public void DisableMoveList(){
        if(AtkBtnList.activeSelf){
            AtkBtnList.SetActive(false);
            FightButtonText.GetComponentInChildren<Text>().text = "Fight";
        }
    }

    public void EnableMoveToolTip(GameObject btn){
        Move move = null;
        ToolTipSnapToCursor();
        MoveToolTip.SetActive(true);
            if(btn == AtkBtn1){
                    move = moveList[0];
            }
            else if(btn == AtkBtn2){
                    move = moveList[1];
            }
            else if(btn == AtkBtn3){
                    move = moveList[2];
            }
            else if(btn == AtkBtn4){
                    move = moveList[3];
            }
        MoveToolTip.transform.Find("Txt_Name").GetComponent<Text>().text = move.name;
        MoveToolTip.transform.Find("Txt_Description").GetComponent<Text>().text = move.desc;
        MoveToolTip.transform.Find("Txt_Cost").GetComponent<Text>().text = move.cost.ToString();
        MoveToolTip.transform.Find("Txt_Power").GetComponent<Text>().text = move.power.ToString() == "0"? "--": move.power.ToString();
        MoveToolTip.transform.Find("Image").GetComponent<Image>().sprite = categsprites[(int) move.cat];
    }

    public void DisableMoveToolTip(){
        MoveToolTip.SetActive(false);
    }

    public void ToolTipSnapToCursor(){
        MoveToolTip.transform.position = new Vector3(Input.mousePosition.x + 6, Input.mousePosition.y + 6, Input.mousePosition.z);
    }

}
