The TurnBased-POC project concept consists of three main components that interact between each others :

1. The navigation graph, that defines the 3D surface where entities can be positioned and move around.
2. The decision tree, that defines what actions the entity is going to do (movement, attack).
3. The event action queue, that controls the execution flow of actions.

Basically the turn of an entity can be defined as : deciding which actions will be performed and send them to the event action queue that will update the navigation graph (and other systems not detailed here).

<svg-inline src="overview_overview.svg"><svg-inline/>

# Entity

An entity is an object that can be placed in the navigation graph and can interact with other entities. <br/>
The entity can execute actions in the world by spending action points. Action points are reset before the entity turn.

# Navigation graph

The navigation graph is a vector of navigation nodes that are connected together by a navigation link.

Navigation nodes are points positioned in 3D space. <br/>
Navigation links allow movement from one node to another. <br/>

## Querying the navigation graph

The navigation graph can be queried to find the shortest path between two points. This is done with an implementation of the A* pathfinding algorithm that calculates the cost of all possible path and return the one with minimum cost. The
algorithm returns a path which is an array of links.

For every path, the cost is calculated as the sum of the crossed distance.

To improve the algorithm efficiency, we can influence how the next calculated node is picked by associating an additional score to every node called "heuristic score". In our case, this heuristic is the distance between the evaluated node  
and the target node. By minimizing the path cost and the heuristic score, we can force the algorithm to pick nodes that are going in the direction of the target node. Thus, finding the shortest path quickly?

## Updating the navigation graph

The navigation graph can be updated anytime by adding or removing links between nodes. To simulate the presence of obstacles on a node, we can remove links that are connected to this node and the traversal algorithm won't take these links
into account anymore.

# Navigation engine

<svg-inline src="overview_navigationgraph.svg"><svg-inline/>

The navigation engine is the layer above the navigation graph that manage entity movement and it's consequence on the navigation graph.
> All entity movement must be executed through the navigation engine to ensure that related structures are properly updated.

It keeps track of the position of entities and make it easy to query which entities are at a specific navigation node.

**Trigger listener:**

The navigation engine allows it's consumer to register trigger listener at a navigation node. <br/>
When an entity move from one navigation node to another, it execute registered triggers attached to the target navigation. These trigger events can then be consumed to execute custom logic on the entity.

## Obstacle

If an entity is defined as an obstacle, this is translated by removing all navigation links that goes to the node where the entity is standing on. This will forbid the navigation graph to use these links when he a nvaigation path is queries. <br/>
When an entity move from navigation node A to B, the links previously removed that point to node A are restored because the entity is no longer standing on this point. The links that point to node B are removed.

# Decision tree

The decision tree is a structure that predict the next actions that an entity will do.

The decision tree lists all possible choices that an entity can do and assign a score to every possibilities.

## Building the tree

The tree is built by using functions that prioritize one type of action from another.

The game have two behaviors defined :

* Moving near enemy and attack them.
* Recover health by moving to a heal cross node.

## Score calculation

For every possible branches of the tree, a score is calculate to evaluate if the chain of action is worth to be executed by the entity. During the score calculation, we simulate the entity actions and see it's consequence on action point and health. All entity data that are updated are temporary values used only for the simulation of the score calculation.

The branch that have the highest score will be picked to be sent to the event action queue.

## Action nodes

We describe all action nodes that can be included in the behavior tree.

**MoveToNavigationNode** <br/>
The entity moves from it's position to a target navigation node. <br/>
The calculated score is the distance crossed to reach the target. <br/>
The amount of action point to spend for the entity to execute the action is equal to the total length of the navigation path. <br/>
For all behavior tree possibilities, the score is normalized according to the maximum distance travel that the entity can move based on it's current action points.

**AttackNode** <br/>
The entity execute an attack that lower the target entity health by spending an amount of action point. <br/>
The calculated score if the amount of health removed to the target.

**HealNode** <br/>
The entity recover health when moving to the designated navigation node. <br/>
The calculated score if the amount of health recovered.

# Event action queue

The TurnBased-POC project heavily uses events to execute actions that needs to be controlled for the proper execution of the game. The purpose of the event action queue is to translate the action nodes contained in the decision made by the
decision tree into action events that will be consumed sequentially.

## Action events

An action event can interact with any internal system of the game, they make things happen. Before an action event is consumed by the queue, it is removed from the queue stack so that an event can dynamically insert another other event at the top of queue.

**EntityCurrentNavigationNodeChange:**

Updates the navigation node value of the entity using the navigation engine.

**NavigationNodeMoveEvent:**

Physically moves the entity to the target navigation node. A navigation event can only be used to move to an adjacent navigation node.<br/>
Consumes action points based on the cost of the travel distance. <br/>
When the entity has reached his target, an EntityCurrentNavigationNodeChange is allocated and pushed at the beginning of the queue.

**EntityApplyDamageEvent:**

Subtract the entity health by the amount provided. <br/>
If the entity is dead, reset the entity by pushing an EntityCurrentNavigationNodeChange event to a null node and an EntityDestroyEvent.

**AttackEntityEvent:**

The translation of the AttackNode decision node. <br/>
Calculates the value of damage applied and push the EntityApplyDamageEvent at the beginning of the queue. <br/>
Consumes action points based on the cost of the attack. <br/>
The attack may involve animation before the damage is applied, so if an animation is provided then an AnimationVisualFeedbackPlaySyncEvent is pushed at the beginning of the queue.

**HealthRecoveryEvent:**

The translation of the HealNode decision node. <br/>
Add the health amount to the targeted entity.

