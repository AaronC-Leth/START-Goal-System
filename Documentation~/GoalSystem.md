# Goal System Documentation

## Table of Contents
1. [Introduction](#introduction)
2. [Key Components](#key-components)
3. [Getting Started](#getting-started)
4. [Creating Goals](#creating-goals)
5. [Managing Goals](#managing-goals)
6. [Updating Requirements](#updating-requirements)
7. [Advanced Features](#advanced-features)
8. [Best Practices](#best-practices)
9. [Troubleshooting](#troubleshooting)

## Introduction

The Goal System is a flexible framework for Unity that allows developers to create, manage, and track complex goal structures in their games or applications. It provides a robust way to define objectives, set requirements, and monitor progress, enhancing gameplay mechanics and user engagement.

## Key Components

### GoalManager
The central component that manages all goals in your scene. It handles goal activation, completion tracking, and requirement updates.

### GoalSO (Scriptable Object)
Represents a single goal in your system. It contains a list of requirements that need to be met for the goal to be completed.

### RequirementSO (Scriptable Object)
The base class for all types of requirements. The system includes:
- BoolRequirementSO
- IntRequirementSO
- ConversationRequirementSO

### RequirementLevelManager
Manages goals specific to a level or scene, allowing for easy setup and cleanup of goals.

### RequirementUpdater
A component that can be attached to GameObjects to update requirements based on in-game events.

### GoalListener
Listens for goal activation and completion events, allowing you to trigger actions when goals change state.

## Getting Started

1. Import the Goal System package into your Unity project.
2. Add a GoalManager component to an empty GameObject in your scene.
3. Create your first goal using the ScriptableObject menu (Create > START > Goal).
4. Add requirements to your goal by creating RequirementSO assets.

## Creating Goals

To create a new goal:

1. Right-click in the Project window.
2. Select Create > START > Goal.
3. Name your new goal.
4. In the inspector, add requirements to the goal by dragging RequirementSO assets into the requirements list.

Example:
```csharp
// Assuming you have a reference to your GoalSO
GoalManager.Instance.AddGoal(myGoalSO);
GoalManager.Instance.ActivateGoal(myGoalSO);
```

## Managing Goals

The GoalManager provides methods to add, activate, and check the status of goals:

```csharp
// Add a goal to the manager
GoalManager.Instance.AddGoal(goalSO);

// Activate a goal
GoalManager.Instance.ActivateGoal(goalSO);

// Check if a goal is active
bool isActive = GoalManager.Instance.IsGoalActive(goalSO);

// Check if a goal is completed
bool isCompleted = GoalManager.Instance.IsGoalCompleted(goalSO);

// Get all active goals
List<GoalSO> activeGoals = GoalManager.Instance.GetActiveGoals();

// Get all completed goals
List<GoalSO> completedGoals = GoalManager.Instance.GetCompletedGoals();
```

## Updating Requirements

Requirements can be updated through code or using the RequirementUpdater component:

```csharp
// Update a bool requirement
GoalManager.Instance.SetBoolRequirement(boolRequirementSO, true);

// Update an int requirement
GoalManager.Instance.IncrementIntRequirement(intRequirementSO, 5);
```

Using RequirementUpdater:
1. Add the RequirementUpdater component to a GameObject.
2. In the inspector, add the requirements you want to update and specify how they should be updated.
3. Call the `UpdateRequirements()` method on the RequirementUpdater when you want to update the requirements.

## Advanced Features

### RequirementLevelManager
Use this component to manage level-specific goals. It allows you to:
- Automatically activate goals when a level starts.
- Define actions to take when goals are activated or completed.

### GoalListener
Use this component to respond to goal state changes:
1. Add the GoalListener component to a GameObject.
2. Specify which goal to listen to.
3. Define actions to take when the goal is activated or completed.

### RequirementTextUpdater
This component can automatically update UI text based on the current value of an IntRequirement:
1. Add the RequirementTextUpdater to a GameObject with a TextMeshProUGUI component.
2. Specify the IntRequirementSO to track.
3. Set the text format string (e.g., "Collected: {value}").

## Best Practices

1. Keep goals and requirements as ScriptableObjects for easy reuse and management.
2. Use meaningful names for your goals and requirements.
3. Break complex goals into smaller, manageable sub-goals.
4. Use the RequirementLevelManager for level-specific goal management.
5. Implement a UI system to display active goals and progress to the player.

## Troubleshooting

- If goals are not activating, ensure the GoalManager is present in your scene.
- Check that all requirements are properly assigned to your goals.
- Use the GoalDebugWindow (Window > START > Goal Debug) to inspect the current state of all goals and requirements.
- If requirements are not updating, verify that the RequirementUpdater is set up correctly and being triggered.

For further assistance, please file an issue on our GitHub repository.