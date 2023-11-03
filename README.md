# Flight Simulator Project Overview

This document outlines the key components and logic of the Flight Simulator project, designed for a job interview showcase.

## Control Scheme

The primary functions of the flight simulator are as follows:

- **Pitch**: `W` (nose down) / `S` (nose up)
- **Yaw**: `A` (turn left) / `D` (turn right)
- **Roll**: `Q` (left roll) / `E` (right roll)
- **Throttle**: `Up Arrow Key` (increase speed) / `Down Arrow Key` (decrease speed)

Auxiliary functions include:

- **Deploy AI Companion**: `Spacebar`
- **AI Companion Stats**: `Tab` to toggle
- **Change View**: `Page Up` / `Page Down`

The controls in the top right corner represent the current AI keystrokes.

## AI Companion Logic

The AI companion is controlled through a state machine, attempting to keep the player within a certain cone of vision.

### State Machine Diagram

![State Machine Diagram](/Images/chart2.svg)

## Code Hierarchy and Script Summaries

The project is structured with several key scripts that handle different aspects of the simulation.

### AIController.cs

This script manages the AI behavior of the companion plane, including state transitions and obstacle avoidance logic.

### PlaneController.cs

Responsible for processing input and applying physics to simulate plane movement.

### InputManager.cs

Utilizes Unity's new input system to broadcast control events to other scripts.

### Class Diagram

![Class Diagram](/Images/chart1.svg)

### Additional Diagrams

![Additional Diagram 3](/Images/chart3.svg)

## AI Testing Video

For a visual demonstration of the Testing the AI in action, check out this video:

[![AI Testing Video](/Images/VideoScreenshot.png)](https://drive.google.com/file/d/1ozDzZCjxI1wwPa8Fw20fWct373e4KGEM/view?usp=sharing)

*Note: Click on the image to view the video. The controls in the top right corner of the video represent the current AI keystrokes.*

## Future Plans

Given more time, the project could be expanded to include a fighting system, damage model, and more sophisticated AI using Unity MLAgents.
