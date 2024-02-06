# Uni-2DGame-Y1T2
"Breakout"

<!-- ROADMAP -->
## V6

- [x] Updated/Working main menu
- [x] Option menu
- [x] Pause menu
- [x] Enemies can drop items
- [x] Scroll to change attacks
- [ ] XP/Level system

What has been discontinued:
* The box system has been scrapped as it is too complicated

What is next:
* Adding an XP/Level system where when enemies die, the players level will increase by a random amount based on two floats which is set on the enemy controller

## V5

- [x] Player Controller
- [x] NPCs
- [x] Enemies
- [x] Keys
- [x] Doors (Locked/Unlocked)
- [x] Local player inventory
- [ ] Box inventory

What needs fixing:
* BoxInteraction.cs will not transfer objects correctly to the inventory and back, this relates to ItemSlotDragDropHandler.cs, InventoryController.cs and InventoryUI.cs