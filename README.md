# Distributed Operating System Principles

This repository contains implementations for three comprehensive programming assignments in the Distributed Operating System Principles course. Each assignment tackles unique aspects of distributed systems, showcasing a range of techniques and technologies.

## Programming Assignment 1 (PA1)
- **Objective**: Introduction to F# and Akka.Net with a focus on actor models and basic actor interactions.
- **Key Concepts**: F# programming, Akka.Net, Actor Model, basic actor communication.
- **Features**:
  - Implementing basic actor systems in F#.
  - Demonstrating actor-to-actor communication and understanding actor lifecycles.
- **Implementation**: [Checkout Socket Programming Implementation](https://github.com/omigirish/DOSP-PA1/tree/Gossip/Socket#readme "Checkout Socket Programming Implementation")
  

## Programming Assignment 2 (PA2)
- **Objective**: Implement the Chord protocol using F# to create a scalable Peer-to-Peer (P2P) lookup service.
- **Key Concepts**: Chord protocol, Peer-to-Peer systems, F# programming, AKKA framework.
- **Features**:
  - Creation of a network ring with dynamic node addition.
  - Implementation of scalable key lookup as described in the Chord paper.
  - Simulation of key lookups and calculation of average hops for requests.
- **Implementation**: [Checkout Chord Protocol Implementation](https://github.com/omigirish/DOSP-PA1/tree/Gossip/Chord#readme "Checkout Chord Protocol Implementation")

## Programming Assignment 3 (PA3)
- **Objective**: Implementing Gossip and Push-Sum algorithms using the actor model in F#.
- **Key Concepts**: Actor model, Asynchronous Gossip, Push-Sum algorithm.
- **Features**:
  - Development of a simulator for Gossip type algorithms based on actors written in F# using the Akka framework.
  - Experimentation with various network topologies: Full Network, 2D Grid, Line, Imperfect 3D Grid.
  - Analyzing convergence of algorithms in different network structures.
- **Implementation**: [Checkout Gossip and Push Sum Implementation](https://github.com/omigirish/DOSP-PA1/tree/Gossip/Gossip#readme "Checkout Gossip and Push Sum Implementation")

Please refer to the respective directories for each assignment for detailed documentation, source code, and analysis.

