//slots are for move targeting purposes. If you target a slot instead of a specific monster, you prevent bugs that happen when a player switches.
public class Slot
{  
        private string slotName;
        public Monster monsterInSlot {get; set;}

        public Slot(Monster monster, string name) {
            slotName = name;
            monsterInSlot = monster;
        }

        public void SetMonster(Monster monster){
            monsterInSlot = monster;
        }
}