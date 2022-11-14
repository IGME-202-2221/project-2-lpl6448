# Project _NAME_

[Markdown Cheatsheet](https://github.com/adam-p/markdown-here/wiki/Markdown-Here-Cheatsheet)

_REPLACE OR REMOVE EVERYTING BETWEEN "\_"_

### Student Info

-   Name: Luke Lepkowski (lpl6448@rit.edu)
-   Section: 04

## Simulation Design

This will be a Christmas-themed game where help Santa's elves meet their quotas. Elves will walk around the workshop, grabbing parts and making toys, as evil snowmen try to stop them by chasing and throwing snowballs at the elves.

### Controls

-   _List all of the actions the player can have in your simulation_
    -   _Include how to preform each action ( keyboard, mouse, UI Input )_
    -   _Include what impact an action has in the simulation ( if is could be unclear )_

## Elf

The elf will construct parts and toys and carry them to different stations in the workshop.

### Looking for Work

**Objective:** Approach a station with a task that needs completing.

#### Steering Behaviors

- Behaviors
   - Seek chosen station (station will be chosen in the transition to this state)
   - Flee from snowmen within a certain radius
   - Stay within the workshop bounds
- Obstacles - Stations (other than the chosen station)
- Separation - Elves, Snowmen
   
#### State Transistions

- When this elf delivers a part/toy to a station
- When this elf is no longer frozen
   
### Working

**Objective:** Stay at the current station for a certain amount of time.

#### Steering Behaviors

- Behaviors
   - Play animation to make part/toy
- Obstacles - None (because the elf is stationary)
- Separation - None
   
#### State Transistions

- When this elf approaches a station (if looking for work)
   
### Carrying

**Objective:** Deliver a part/toy to a particular station.

#### Steering Behaviors

- Behaviors
   - Seek chosen station (station will be chosen in the transition to this state)
   - Flee from snowmen within a certain radius
   - Stay within the workshop bounds
- Obstacles - Stations (other than the chosen station), dropped parts/toys
- Separation - Elves, Snowmen
   
#### State Transistions

- When this elf finishes work at a station
- When this elf touches a dropped part/toy
   
### Frozen

**Objective:** Wait for the cooldown to become unfrozen.

#### Steering Behaviors

- Behaviors
   - Play frozen effect
   - Drop current part/toy
- Obstacles - None (because the elf is stationary)
- Separation - None
   
#### State Transistions

- When this elf is touched by a snowman
- When this elf is hit by a snowball

## Snowman

The snowman will wander around the map, attempting to freeze elves by touching them or throwing snowballs.

### Throwing

**Objective:** Wander around the outside of the map, throwing snowballs at nearby elves.

#### Steering Behaviors

- Behaviors
   - Wander
   - Stay outside of the workshop bounds
   - Stay within the map bounds
   - Throw snowballs at nearby elves occasionally
- Obstacles - Stations, dropped parts/toys
- Separation - Snowmen, elves
   
#### State Transistions

- When this snowman touches and freezes an elf
   
### Charging

**Objective:** Run into the workshop and freeze an elf.

#### Steering Behaviors

- Behaviors
   - Seek the nearest elf
   - Stay within the map bounds
- Obstacles - Stations, dropped parts/toys
- Separation - Snowmen
   
#### State Transistions

- Random chance after throwing a snowball

## Sources

-   _List all project sources here –models, textures, sound clips, assets, etc._
-   _If an asset is from the Unity store, include a link to the page and the author’s name_

## Make it Your Own

- I added two more states to the elf (Working and Frozen). These states do not have steering forces because they effectively freeze the elf.
- I will make the game in 3D (using a mostly topdown perspective) and will let the player rotate the camera around the workshop.
- I will make the elves and toys/parts out of voxels to fit the 3D style.

## Known Issues

_List any errors, lack of error checking, or specific information that I need to know to run your program_

### Requirements not completed

_If you did not complete a project requirement, notate that here_

