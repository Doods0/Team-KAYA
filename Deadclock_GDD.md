# Deadclock - Game Design Document (Team KAYA)

## Game Identity & Pitch
**Title:** Deadclock
**Core Hook:** A bullet-hell roguelike where time itself is the resource you're managing under two opposite failure states—survive as long as possible while keeping time balanced!

## Design Pillars & Player Experience
To ensure our players feel the right mix of excitement and tension, we are building Deadclock around three core explicit pillars:
1. **Tight Resource-Juggling:** Time is both your weapon and your enemy. Players must constantly weigh the risk of speeding time up against the danger of letting it slow down to a halt.
2. **Escalating Tension & Risk:** The longer you survive, the harder it is to keep time stable. Escalating risk means players are always on the edge of their seats, balancing fast-paced bullet-hell dodging with strategic pickup collection.
3. **Eerie-but-Vibrant Atmosphere:** We want the player to feel a sense of eeriness mixed with bright, wholesome aesthetics. It's a magical forest where the flow of time is fractured and strange.

## Visual Identity & Theme
- **Theme:** An eerie, spiritual, but vibrant and somewhat wholesome magical forest. We are moving slightly away from purely dark gothic to a brighter environment that still holds an underlying souls-like dread. 
- **Map:** A magical forest with a distinct blue-ish tint where the flow of time feels visibly altered.
- **Enemies:** Corrupted entities, possessed creatures (like the blue slimes), dead magicians, and lost souls. They maintain a cute, somewhat wholesome look despite being deadly.
- **UI:** Minimal HUD. The main focal point is the large time-scale clock in the corner that indicates the current speed of time. The shop menu will be clean and easy to read.

## Gameplay Concept & Core Loop
**Main Objective:** Survive as long as possible while keeping time balanced.

### Core Loop Sequence
1. **Spawn & Survive:** Enter the forest and face endless, escalating waves of enemies.
2. **Manipulate Time:** Time slows down passively. Killing enemies and collecting their specific drops speeds time up or slows it down.
3. **Manage the Thresholds:** 
   - *Time too slow* -> Time stops, Lose condition (Death).
   - *Time too fast* -> Enemies become insanely fast, increasing damage risk -> Lose condition (Death by health loss).
4. **Trigger the Economy:** If the player has a shop token, they can activate it by altering time either speeding it up too much or slowing it down, the shop menu is then triggered automatically.
5. **Upgrade & Repeat:** Collecting the shop pickup pauses the tension, opening the shop UI. Here, you spend accumulated points on upgrades before dropping back into the escalating chaos.

### Enemies
We are introducing varied AI behaviors to keep the bullet-hell aspect challenging:
- **Normal:** Standard movement, deals damage on touch.
- **Exploders:** Detonate upon getting close or on death, forcing players to keep their distance and plan kills carefully.
- **Throwers:** Ranged attackers that force the player to dodge projectiles, complicating the time-juggling mechanics.
- **Rushers:** They act like bulls, rushing towards the player and repositioning themselves.

### Weapons
The player bends time to their advantage using distinct weapon playstyles. Weapons can be swapped out in the shop, but purchased upgrades remain persistent.
- **Heavy Melee:** Higher damage, lower range. Forces close-quarters combat, meaning time-altering pickups are almost unavoidable when they drop.
- **Light Weapon:** Dual-mode (switchable via keybind).
  - *Throw (Ranged):* Lower damage, kills happen far away, making pickups easily avoidable if you need time to naturally slow down.
  - *Slash (Melee-range):* Range sits between Heavy Melee and Throw. Can hold tempting buffs or curses.

### Economy, Shop & Upgrades
We are fully integrating an in-run economy to give players strategic choices between waves.
- **The Shop Pickup:** Collecting it opens the shop UI when triggered and acts as a breather.
- **Purchases:** Players spend points (dropped by enemies) to buy new weapons (Light or Heavy) and upgrades.
- **General Upgrades:** Upgrades are detached from specific weapons. They buff the player's overall stats (defense, speed, health) or enhance general weapon attributes (e.g., firing a special beam on every 4th hit). 
- **Synergies:** Players are meant to combine upgrades for deep build variety. For example, you might grab an upgrade that applies a "Curse" status with the Light Weapon, then purchase a general buff that makes Heavy Weapons deal double damage to cursed enemies.

