# zwx7f3u2
Card Match Game - Unity Prototype

Overview

This is a 3D card-matching game prototype developed in Unity 2021.3.22f1 LTS.
The game allows players to flip cards, match pairs, and track their score and turns. It supports multiple difficulty levels and saves progress between sessions.

Development Choices

DOTween: Used for card flipping, and UI popups. Familiarity with DOTween allows for smooth and easy-to-tune animations.

PlayerPrefs: Chosen for saving game progress since the saved data is small (rows, columns, score, turns, card states). No need for JSON or external files.

How to Run

Open the project in Unity 2021.3.22f1 LTS.
Open the MainMenu scene.
Select a difficulty or continue a saved game.
Play the game!

Build Folders

Desktop: Contains the executable build for PC.
Android: Contains the APK build for Android devices.
