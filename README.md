# Flight Simulator Project Overview

This document outlines the key components and logic of the Flight Simulator project, designed in Unity 2022.3.11f1.

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

## Play the Flight Simulator

Experience the flight simulator firsthand by clicking the image below. For the best experience, play the game in full-screen mode.

[![Play Flight Simulator](/Images/VideoScreenshot.png)](https://cperos-xr.github.io/FlightDemoGame/)

*Click the image above to launch the game.*

## AI Companion Logic

The AI companion in the Flight Simulator is governed by a state machine designed to emulate intelligent behavior patterns. The primary objective of the AI is to maintain the player within a specific cone of vision, which simulates a realistic following behavior.

### State Machine Overview

## AI Companion Logic

The AI companion's behavior in the Flight Simulator is orchestrated by a state machine, with the "Follow Player" and "Avoid Obstacle" states being pivotal to its functionality.

### Follow Player State

In this state, the AI aims to keep the player within a designated cone of vision, simulating a wingman or follower's behavior. This ensures the AI maintains a realistic and strategic position relative to the player's aircraft.

### Avoid Obstacle State

The "Avoid Obstacle" state is activated when potential collisions are detected. The AI employs raycasting to understand its surroundings and to navigate through obstacles. During development, two main strategies were tested:

- **Longest Average Distance**: The AI would move towards the direction where the raycasts showed the longest average distance to obstacles, aiming for a quicker exit from the obstacle field.
- **Shortest Average Distance**: Alternatively, the AI would steer away from the direction with the shortest average distance to obstacles, focusing on immediate collision avoidance.

After experimenting with both approaches, I found that each had its merits in different scenarios. The longest average distance method allowed the AI to clear the obstacle field rapidly, which was beneficial in open areas. However, the shortest average distance strategy proved more effective in densely packed environments where collision avoidance was critical.

Ultimately, I decided to implement the shortest average distance approach. This method aligned better with the primary goal of maintaining safety and smooth flight, as it provided a more reliable avoidance of imminent collisions. The AI's ability to navigate complex environments using this strategy was, at times, more adept than manual piloting, which was a testament to the effectiveness of the algorithm.

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

## Gameplay Video

To check out some of the Gameplay in action, you can view the video here (Second Half part is AI): [Gameplay Video](https://drive.google.com/file/d/1mHDlRYuGt1mclay8npHNUDM1EV2_qTru/view?usp=sharing).

## AI Testing Video

For a visual demonstration of the AI testing in action, you can view the video here: [AI Testing Video](https://drive.google.com/file/d/1ozDzZCjxI1wwPa8Fw20fWct373e4KGEM/view?usp=sharing).

*Note: The controls in the top right corner of the video represent the current AI keystrokes.*


## Flight Physics

The flight simulator strives to replicate the dynamic and realistic behavior of a single-engine aircraft. The physics model is designed to account for key aerodynamic principles, ensuring that the plane's behavior in the simulator reflects real-world flight characteristics as closely as possible.

### Lift and Speed

Lift generation is a critical aspect of the flight model. It is calculated relative to the plane's speed, adhering to the principle that lift increases with the square of the velocity. This means that as the aircraft accelerates, the lift force grows exponentially, allowing the plane to take off and maintain altitude.

### Mass and Aerodynamics

The aircraft in the simulator has been assigned a mass that is representative of a typical single-engine plane, contributing to the authenticity of the flight dynamics. The takeoff speed is set around 100 km/hr, which aligns with the expected performance of a real-world counterpart. This speed threshold is where the generated lift overcomes the plane's weight, allowing for a smooth takeoff.

### Realistic Control Response

The control inputs for pitch, yaw, and roll are designed to mimic the responsiveness of an actual aircraft. The physics engine calculates the resultant forces and moments from the control surfaces in real-time, providing a tactile and responsive flight experience. The controls are mapped to a range of -1 to 1, akin to a real joystick, ensuring that the simulator is capable of being adapted for use with physical flight controls, such as those used in RC planes or flight simulation rigs.

### Future Plans & Enhancements

While the current model provides a solid foundation for realistic flight simulation, future enhancements could include more complex aerodynamic modeling, such as stall characteristics, wind and turbulence effects, and a more detailed engine performance curve. These additions would further refine the simulation, providing an even more immersive and accurate flight experience.

Given more time, the project could be expanded to include a multiplayer, a fighting system, damage model, more collectables such as weapons and sheilds, and a more sophisticated AI using Unity MLAgents.
