# TurnBased-POC

This repository is the source code of a proof of concept of a Unity game.

This project contains source code only, there is no asset files. I decided to strip assets from git history as they were taking space for nothing.

## Purpose

The purpose of this repository is to share pieces of code that can be inspiring for people who wants to develop similar features.

This project has no external documentation, your guide to understand how things work together will be comments (when there are some 😋). Feel free to dig into code to get a grasp of what is happening.

If you want more details on a specific feature, feel free to post an issue and I will answer to you as best as I can.

## Demo

A playable demo is available on [itch.io](https://loic-dal-zotto.itch.io/poc-turnbasedtacticalmovement) or as a [git release](https://github.com/ldalzotto/TurnBased-POC/releases/tag/0.9).

**Insert youtube video**

### Controls

You don't control anything. 

Press Enter to start the level. Agents will act and move automatically.

Press Delete to restart the level.

## Features

The application features (links redirect to source code implementation) :

* [Turn based action sequencing](https://github.com/ldalzotto/TurnBased-POC/tree/master/Assets/Core/TurnTimeline.)
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

## License

Feel free to steal it.
