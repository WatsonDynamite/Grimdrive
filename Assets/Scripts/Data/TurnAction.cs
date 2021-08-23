//this defines the action that a player takes in a turn.
//it's essentially an encapsulation for the 3 types of action a player can take on their turn.


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    MOVE,
    SWITCH,
    REST
}

public class TurnAction
{
    public ActionType actionType{get;}
    public Move move{get;}
    public Monster user{get;}
    public Slot target{get;} 
    public Monster switchMonster{get;}
    public int priority{get;}

        //Turn action for performing a move. mv is the move to be used, user is the using monster and target is the opponent (a Slot, not a monster).
    public TurnAction(Move mv, Monster usr, Slot trg){
            actionType = ActionType.MOVE;
            move = mv;
            user = usr;
            target = trg;
            priority = mv.priority;
    }

    public TurnAction(ActionType at){
            actionType = at;
            move = MoveList.moveNone;
    }

        //Turn action for switching. mn is the monster to switch to. user is the monster that's being switched out.
    public TurnAction(Monster mn,  Monster usr){
            actionType = ActionType.SWITCH;
            user = usr;
            move = MoveList.moveNone;
            switchMonster = mn;
            priority = 999;
    }
}
