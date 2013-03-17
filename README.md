XNA_GameEngine_CS680
====================
Compiling and Running:
This project requires the following things to compile:
1.) Windows 7
2.) Visual Studio 2010
3.) XNA Game Studio 4.0

This project can be run on any windows computer with the XNA Game Studio 4.0 redistributable installed.

Switching Demos:
The class CoreMain.cs has all of the demos that have been coded in the LoadContent method. To switch between the demos there is a demoNumber variable. Here are descriptions of the demos:
// 0: Gaseous Circles Demo (many small circles)
// 1: Small mass object hitting large mass object Demo
// 2: large object squashing small object demo (low Velocity)
// 3: large object squashing small object demo (medium Velocity)
// 4: large object squashing small object demo (High Velocity)
// 5: Single Player Square Object (controlled by arrow keys)
// 6: Gaseous Squares Demo (many small squares)
// 7: Gaseous Rectangles Demo (many small rectangles) collision intensive demo
// 8: Stacked Boxes Demo
// 9: Single Player Square Object (controlled by arrow keys) with small sphere and large sphere
// 10: Single Player Square Object (controlled by arrow keys) with two small spheres
// 11: Network play demo

Demo 7 is a newly added demo that was not presented in class. It contains 360 rectangular objects that are given a random initial velocty. This demo was primarily created to show off the speed increase of the quad tree versus non-quad tree code.

= Physics Module =
The entry point to the physics module is in the PhysicsWorld class. This class has an Update method which performs a complete physics update every time it is called.
In the latest code the update method essentually just calls the update method on the QuadTree object.

The QuadTree object is the one that is primarily responsible for creating, maintaining, fixing, and updating the data structure that holds the objects.
Before the QuadTree the code tested collisions between every single object in the game, afterwards the collision tests have been drastically reduced.
To show this we created Demo 7 which has 360 rectangular objects (the most expensive in our system to collide against). We also tested every demo but the others were much less telling.

Here are the results:
Demo	# objects	Regular (ms)	Quad Tree (ms) 	Speed Increase
0	50	0	0	0
1	2	0	0	0
2	2	0	0	0
3	2	0	0	0
4	2	0	0	0
5	1	0	0	0
6	50	3	0	Infinity
7	360	174	8	2175%
8	50	1	0	Infinity
9	3	0	0	0
10	3	0	0	0
11	4	0	0	0

The results show that demo 7 ran with a 2175% speed increase with the quad tree than without.

== Supported Colliders ==
Rectangles
Circles
Lines (The angular velocity and linear velocity attributes of lines are undefined. Creating real objects with lines as colliders will result in undefined behavior. These were only used as boundaries for the scene.)

== Collision Heirarchy ==
The first most broad collision detection is the Quad Tree. It ensures that collisions are only tested between objects that belong to the same locality island in the scene. The Quad Tree is adaptive and will change shape / depth as objects move around in it.
A weak bound of the number of objects that each node in the Quad Tree can hold is 4 * the depth that the node is at. This is weak because the node will subdivide at this number (4 * depth) but there is not guarantee that all of the objects will be placed in the children.
If an object belongs in two child regions, then it is left in the parent to reduce in space and simplify the code. It would also be possible to place the same object once in each child, however this would make it more difficult to later maintain the tree when objects move around.

The second level of collision detection is the axis aligned bounding box. Each collider has one and most objects utilize it. Circle on circle collisions do not use the axis aligned bounding box since these collisions are relatively cheap.

The third and final level of collision detection is precise and is very expensive for complex objects. The most complex object in this engine is the rectangle and as demo 7 has shown performing just level 2 and 3 of collision detection is not enough for heavily populated scenes.
