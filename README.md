# TurnBased-POC

This repository is the source code of a proof of concept of a Unity game.

TurnBased-POC is a proof of concept that aims to show the feasibility and practical application of :

* Entity turn lifecycle management
  * Ordering and stacking execution of events that involes interaction of Entities.
  * Execution of TurnAction per Entity that consumes ActionPoints
* A* pathfinding
  * With dynamic obstacle avoidance
  
This repository contains source code only, there is no asset files. I decided to strip assets from git history as they were taking space for nothing. The whole unity project with assets is available [here](https://drive.google.com/file/d/1z_Xk5o0m9eFeluYiwCw1Kq2WxEzHGnHv/view?usp=sharing).

## Demo

A playable demo is available [here](https://github.com/ldalzotto/TurnBased-POC/releases/tag/0.9).

![](https://i.imgur.com/4HYH1P6.gif)

### Controls

You don't control anything.

Press Enter to start the level. Agents will act and move automatically.

Press Delete to restart the level.

## Features

The application features (links redirect to source code implementation) :

* [Turn based action sequencing](https://github.com/ldalzotto/TurnBased-POC/tree/master/Assets/Core/TurnTimeline)
* [A* path findind.](https://github.com/ldalzotto/TurnBased-POC/tree/master/Assets/Core/NavigationGraph)
* [Logic execution when an Entity is moving to a node that have a trigger Entity.](https://github.com/ldalzotto/TurnBased-POC/tree/master/Assets/Core/NavigationEngine)
* [Entity interaction](https://github.com/ldalzotto/TurnBased-POC/tree/master/Assets/Core/Entity/Events) :
  * Moving Entity :
    * Can attack other moving entities
    * Can die when health < 0
  * Trigger Entity :
    * [Add health to the moving Entity that walks at the same cell.](https://github.com/ldalzotto/TurnBased-POC/tree/master/Assets/Core/Entity/Component/Trigger)
* [Agent action decision tree that picks to most favorable actions.](https://github.com/ldalzotto/TurnBased-POC/tree/master/Assets/Core/AI/DecisionTree)
* [Centralized Event queue that monitor all actions in the game.](https://github.com/ldalzotto/TurnBased-POC/tree/master/Assets/Core/EventQueue)
* [Non unity Entity <-> Component relationship.](https://github.com/ldalzotto/TurnBased-POC/tree/master/Assets/Core/Entity)
* [Non unity game loop execution order.](https://github.com/ldalzotto/TurnBased-POC/tree/master/Assets/GameLoop)

## Dependencies

The application uses the OdinInspector package. The package is not included, you can buy it [here](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041).
