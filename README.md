# ML Agents | Maze Solver

### Start Small
![Orthographic Small Maze](https://github.com/afewvowels/unity_00_10_mlagents_maze_solver/blob/master/small_ortho.png)
![Perspective Small Maze](https://github.com/afewvowels/unity_00_10_mlagents_maze_solver/blob/master/small_perspective.png)

### Dream Big
![Orthographic Big Maze](https://github.com/afewvowels/unity_00_10_mlagents_maze_solver/blob/master/large_ortho.png)
![Perspective Big Maze](https://github.com/afewvowels/unity_00_10_mlagents_maze_solver/blob/master/large_perspective.png)

Here's some good plain fun. A procedurally generated maze built off of [Ray Wenderlich's maze tutorial](https://www.raywenderlich.com/82-procedural-generation-of-mazes-with-unity) and tweaked heavily to support arbitrary map sizes, empty or filled interiors, as well as a (badly solved) path to follow, treasures, and obstacles!

I won't claim that this is the cleanest code out there, but it's functionally all there :)

The agent is equipped with raycast sensors and a render texture sensor. The agent is aware of four tags:
1. The maze mesh
2. Treasures
3. Obstacles
4. Goal

I haven't had much luck training visually before so that render texture sensor is really only going to let the agent know what reward level to expect from the treasure its about to pick up. It will also let it follow the trail of pink graphics from start to end (although my pathfinding algorithm only finds a path, not the shortest path so we'll see if the agent is able to discover a quicker way to the goal in time).

Anyway, I'm loving working with [Unity's ML Agents toolkit](https://github.com/Unity-Technologies/ml-agents) and if you haven't seen it before, definitely check it out!
