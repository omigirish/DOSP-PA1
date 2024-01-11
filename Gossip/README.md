# DOSP Programming Assignment 3

## Team Information

- **Girish Vinayak Salunke**
  - Email: gsalunke@ufl.edu
  - UFID: 88908382
- **Janhavi Shriram Athalye**
  - Email: janhavi.athalye@ufl.edu
  - UFID: 76926526

## Environment Setup

The project is compatible with Windows and macOS operating systems. It requires the following software setup:

- F# .NET framework
- Visual Studio Code IDE
- AKKA actor framework

## Compilation Instructions

To compile and run the project, follow these steps:

1. Clone the repository.
2. Open the folder in Visual Studio Code.
3. Use the `dotnet run` command with the following parameters: 
   - Number of nodes
   - Topology (full, 2D, line, imp3D)
   - Algorithm (gossip, push-sum)
   - Syntax: <b> dotnet run \<numNodes> \<topology> \<algorithm> </b>

Here numNodes represents the desired number of peers in the network, and topology represents the type of topology to be used which can be one of full, 2D, line, imp3D and algorithm can have values of either "gossip" or "pushsum".

## Implementation

### Gossip Algorithm Implementation
 - <b>Functionality</b>: The Gossip algorithm for information propagation was fully implemented. Each actor efficiently selects a random neighbor and transmits the rumor, adhering to the specified termination condition.
Unzip the PA3_Team3 folder from the zipped packet and open it in Visual Studio Code IDE Open terminal and change directory to the folder after unzipping it
Run the command: dotnet run <numNodes> <topology> <algorithm> For example: dotnet run 10 2D gossip
- <b> Asynchronous Handling</b>: Leveraging the Akka framework, we ensured fully asynchronous communication among actors, which is crucial for the Gossip algorithm's effectiveness.
- <b> Termination Condition</b>: Actors successfully track the number of times a rumor is heard and cease transmission upon reaching the pre-set threshold, demonstrating correct algorithm behavior.

### Push-Sum Algorithm Implementation
- <b> State Management </b>: Each actor accurately maintains two quantities, s and w, and updates these values upon
receiving messages.
- <b>Sum Estimation</b>: The implementation correctly calculates the sum estimate at any given moment as s/w, aligning with the algorithm's requirements.
- <b>Termination Criteria</b>: The actors appropriately terminate when the ratio s/w stabilizes within the specified threshold over three consecutive rounds.

### Network Topologies
- <b>Full Network</b>: Every node is connected to every other node.
- <b>2D Grid</b>: Nodes are arranged in a grid, connecting to adjacent nodes.
- <b>Line</b>: Each node connects to its immediate neighbors in a line formation.
- <b>Imperfect 3D Grid</b>: A 3D grid with additional random connections.

## Code Structure

| File Name         | Description |
|-------------------|-------------|
| `Program.fs`      | This file is the entry point of the simulation, handling command line inputs to set up network topologies and initialize either the Gossip or Push Sum algorithm. It orchestrates the creation of nodes in the specified topology, manages their interactions, and tracks the performance and termination of the simulation. |
| `Gossip.fsproj`   | This file is the project configuration file for an F# application using .NET, specifying the project's structure, dependencies, and target framework (net7.0). It includes references to various source files (like Util.fs, Simulator.fs) and external packages (such as Akka and Deedle), essential for the simulation. |
| `PushSumNode.fs`  | This module defines the behavior of nodes in the Push Sum algorithm within a distributed system, managing state updates and message passing between nodes. It includes logic for initializing nodes, handling incoming messages, calculating sum estimates, and determining convergence based on a specified threshold. |
| `Simulator.fs`    | This module defines the Simulator actor in the distributed system, responsible for monitoring the convergence of nodes and logging the results of either the Gossip or Push Sum algorithm. It handles termination messages from nodes, calculates convergence times, and outputs performance metrics to a CSV file. |
| `Topology.fs`     | This file defines various network topology creation functions for a distributed system simulation, including line, 2D grid, imperfect 3D grid, and full network topologies. It maps each node to its neighbors based on the specified topology. |
| `Util.fs`         | This module provides utility functions for the simulation, including rounding node numbers for grid topologies, selecting random elements or neighbors, and appending data to a CSV file. These utilities support the core functionality of the simulation across various network topologies.|
| `FindNeighbour.fs`| This file contains functions to determine the neighbors of a given node in either a 2D or 3D grid topology. The functions calculate neighbors based on the node's position in the grid and the overall size of the grid. |
| `GossipNode.fs`   | This file defines the behavior of nodes participating in the Gossip algorithm, handling rumor spreading and reception. It includes logic for spreading a rumor to random neighbors, tracking the number of times a rumor is heard, and determining when to stop rumor transmission.|
| `output.csv`      | This file stores the specific output or results generated during the simulation, which includes the Algorithm,Topology,Nodes,ConvergenceTime. This file is used to generate a Graph. |

This table provides an overview of the key files in the project and their respective functionalities.

## Performance and Convergence Analysis

- <b>Convergence Measurement</b>: The system accurately measures and outputs the time taken to achieve
convergence in each scenario.
- <b>Scalability and Efficiency</b>: Our tests across various network sizes demonstrated the system's scalability and efficiency in handling large numbers of actors.

## Dependencies

The project utilizes several dependencies:

- `Akka.FSharp`: F# API for the Akka actor framework.
- `FSharp.Core`: Core library for F# programming.
- Other necessary .NET libraries for network and concurrent programming.

## Output Analysis

- The project outputs are analyzed in terms of convergence time and algorithm efficiency.
- It also prints how many nodes have converged.
- For the Push Sum algorithm it also outputs the s/w value for each Actor Node.
- Output data is captured in `output.csv` for further analysis.

### Screenshots

Screenshots demonstrating the program output for different scenarios and topologies:
- dotnet run 10 line gossip
![Output 1](./Gossip/Outputs/Output1.png "Our Team Logo")
- dotnet run 10 2D pushsum
![Output 2](./Gossip/Outputs/Output2.png "Our Team Logo")


## Largest Network

In our largest network tests, the Gossip algorithm with 10,000 nodes showed the Full Network topology as the most efficient, converging in 2.749 seconds, followed by the 2D Grid and Imperfect 3D Grid. For the Push Sum algorithm in a 5,000-node network, the Full Network again excelled with a convergence time of 4.215 seconds. These results highlight the Full Network's superior performance in large-scale networks.

## Convergence Time Results

Based on the various simulations that we ran nf the Gossip and Push Sum algorithms across various network topologies and node counts, the following trends and observations can be highlighted:
1. The Gossip algorithm consistently shows faster convergence times compared to the Push Sum algorithm across all topologies.
2. The Full Network topology yields the quickest convergence times for both algorithms, likely due to the direct connectivity between all nodes.
3. As the number of nodes increases, there is a marked increase in convergence time for both algorithms. This trend is more pronounced in the Push Sum algorithm, indicating its sensitivity to network size.
4. The Gossip algorithm scales better with the increasing number of nodes, especially in the Full Network and Imperfect 3D Grid topologies.
5. In the Line topology, both algorithms exhibit a significant increase in convergence time as the node count grows, with the Push Sum algorithm being particularly affected. This indicates that the Line topology is less efficient for information propagation and sum calculation.
6. The 2D Grid and Imperfect 3D Grid topologies offer a balance between the extremes of the Line and Full Network topologies, with moderate convergence times for both algorithms.


## Results Visualization
1. Convergence Time vs NumNodes for all Topologies in Gossip Algorithm

![Output 3](./Gossip/Outputs/Output3.png "Our Team Logo")

2. Convergence Time vs NumNodes for all Topologies in Push Sum Algorithm

![Output 3](./Gossip/Outputs/Output4.png "Our Team Logo")

3. Comparison of Various Topologies in Gossip Algorithm

![Output 3](./Gossip/Outputs/Output5.png "Our Team Logo")
![Output 3](./Gossip/Outputs/Output6.png "Our Team Logo")

4. Comparison of Various Topologies in Push Sum Algorithm

![Output 3](./Gossip/Outputs/Output7.png "Our Team Logo")

![Output 3](./Gossip/Outputs/Output8.png "Our Team Logo")

![Output 3](./Gossip/Outputs/Output9.png "Our Team Logo")



## Assumptions for the Simulation

### Network Stability and Reliability
- <b>Constant Network Topology:</b> It is assumed that the network topology (Full, Line, 2D Grid, Imperfect 3D Grid) remains constant throughout the simulation. No nodes are added or removed, and the connections between nodes do not change.
- <b>Reliable Communication:</b> All messages sent between nodes are assumed to be reliably delivered without loss, corruption, or reordering.

### Node Behavior and Capabilities
- <b>Homogeneity of Nodes:</b> Each node in the network is assumed to have uniform capabilities in terms of processing power, memory, and network bandwidth.
- <b>Synchronous Operation:</b> Nodes are assumed to operate in a synchronous manner, with a uniform delay for message processing and transmission.

## Conclusion
In this project, we effectively implemented and analyzed the Gossip and Push Sum algorithms in distinct network topologies using F# and Akka. The findings reveal significant differences in the algorithms' performance across topologies like Full Network, Line, 2D Grid, and Imperfect 3D Grid. The Gossip algorithm proved faster in spreading information, while Push Sum was more efficient in calculating network-wide sums. These results underscore the critical influence of network structure on distributed algorithm efficiency. The project not only deepened our understanding of these algorithms but also demonstrated the practical capabilities of the Akka framework in a distributed setting.

