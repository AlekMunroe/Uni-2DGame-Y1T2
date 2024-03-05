# Uni-2DGame-Y1T2
"Breakout"

<!-- ROADMAP -->
## V13.5

- [x] Imported special op spritesheet.
- [x] Started creating the first cutscene, it has been commented but is incomplete.
- [x] Updated main menu (Incomplete)
- [ ] The rest of the sound effects
- [ ] Background music
- [ ] Updated UI
- [ ] Need to remake idle animations

## V13

- [x] Allow the player to hold health items
- [x] Fixed bug where UI shows half health if the sprites are not set correctly when updating the UI.
- [x] Created a script that will destroy an item if it has previously been picked up.
- [ ] The rest of the sound effects
- [ ] Background music
- [ ] Updated UI

## V12

- [x] Added save states for room locations
- [ ] Allow the player to hold health items
- [ ] The rest of the sound effects
- [ ] Background music
- [ ] Updated UI

## V11

- [x] Updated player spritesheet
- [ ] Allow the player to hold health items
- [ ] Add save states for room locations
- [ ] The rest of the sound effects
- [ ] Background music
- [ ] Updated UI

## V10

- [x] Added save states for health and if the doors are locked or unlocked
- [ ] Allow the player to hold health items
- [ ] Add save states for room locations
- [ ] The rest of the sound effects
- [ ] Background music
- [ ] Updated UI

## V9

- [x] Start of sound effects have been added
- [ ] The rest of the sound effects
- [ ] Background music
- [ ] Updated UI

## V8

- [x] Levelling system
- [ ] Sound effects

## V7

- [x] Enemy animations for Idle, move, attack and die
- [x] Updated attack function
- [x] Updated death function to IEnumerator
- [x] Optimised how the enemy checks how close it is to the player
- [ ] XP/Level system

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