# Project "Elf Extravaganza"

### Student Info

-   Name: Luke Lepkowski (lpl6448@rit.edu)
-   Section: 04

## Simulation Design

This is a Christmas-themed game where you must help Santa's elves meet their quotas. Elves will walk around the workshop, grabbing parts and making toys, as evil snowmen try to stop them by throwing snowballs into the workshop.

### Controls

-  Hold down right click to enter Camera mode.
    -   Move the mouse to rotate the camera.
    -   Zoom in and out using the scroll wheel.
-  The Crafting panel shows which items still need to be crafted.
    -   You can expand and collapse the ingredients required to build certain items by clicking the button to the left of the item.
    -   If the crafting button to the right of an item is green, you can click it to enter Craft mode.
    -   Once in Craft mode, all stations that can craft this item have a green circle underneath them.
        - If a station has a red circle, it is either already crafting an item, or the item that it just finished crafting must be moved.
    -   Click a station highlighted green to craft the item at that station.
    -   Once a root-level item (like a Toy Car or a Teddy Bear) is crafted, the elves will move it to the launch pad, where it will be launched into the sky and delivered to Santa.

## Elf

The elf will construct parts and toys and carry them to different stations in the workshop.
There are three kinds of tasks that elves can take:
- Gather - Gather an item from a particular station and choose a destination for this item.
- Deliver - Carry an item to a particular destination station.
- Use - Use a particular station for a certain amount of time.
These tasks are stored separately from the elf's states, but they influence how the elf behaves and where the elf goes.

### Waiting for Task

**Objective:** Wander around the map until a task becomes available.

#### Steering Behaviors

- Behaviors
   - Wander around the map
   - Stay within the workshop bounds
   - Separate from nearby agents
- Obstacles - Stations (other than the chosen station)
- Predictive/Custom Separation - Elves, Snowmen
   
#### State Transistions

- Initial state
- When this elf completes a task and cannot find a new task
   
### Walking to Task

**Objective:** Walk to the station required by the current task.

#### Steering Behaviors

- Behaviors
   - Seek chosen station (station will be chosen in the transition to this state). To improve obstacle avoidance, the seek and obstacle avoidance functionalities are combined.
   - Stay within the workshop bounds
   - Separate from nearby agents
- Obstacles (Custom Avoidance) - Stations (other than the chosen station), dropped parts/toys
- Predictive/Custom Separation - Elves, Snowmen
   
#### State Transistions

- When the user starts a new task and this elf chooses it
- When this elf completes a task and successfully finds a new task
   
### Processing Task

**Objective:** Stay at the current station for a certain amount of time.

#### Steering Behaviors

- Behaviors
   - If necessary, play any animations to make the part/toy
- Obstacles - None (because the elf is stationary)
- Separation - None
   
#### State Transistions

- When this elf approaches the chosen station in the Walking to Task state

## Snowman

The snowman will wander around the outside of the map, throwing snowballs onto the map that create temporary snow piles for elves to avoid.

### Wandering

**Objective:** Wander around the outside of the map.

#### Steering Behaviors

- Behaviors
   - Wander
   - Stay outside of the workshop bounds
   - Stay within the larger map bounds
   - Separate from nearby agents
- Obstacles - Trees
- Separation - Snowmen, Elves
   
#### State Transistions

- Initial state
- After this snowman finishes throwing a snowball
   
### Throwing

**Objective:** Stop, turn red (indicator), and throw a snowball onto a random location in the workshop.

#### Steering Behaviors

- Behaviors
   - Stop (custom steering behavior) and face in the direction the snowball will be thrown
- Obstacles - None
- Separation - None
   
#### State Transistions

- Random chance while wandering

## Sources

-   The snowman model and animation was created by my cousin for a previous project.
-   All other models and assets were created by me.

## Make it Your Own

-   I created a more advanced state system for elves, letting them accept and complete various tasks, working as a team to craft items.
-   The game is in 3D and lets the player rotate the camera around the workshop.
-   I created the game's voxel art using MagicaVoxel and added some extra environment art to make the game look more interesting.

## Known Issues

-   WebGL cannot properly "lock" the cursor while in Camera mode, so the cursor can occasionally go out of the game's frame while moving the camera.
-   The custom separation and obstacle avoidance algorithms still can cause elves to get stuck occasionally, especially if there are many elves in the same region.

### Requirements not completed

All requirements were completed.
