# IceCold Save Service

A flexible, type-safe, and asynchronous save service for Unity. It is built on top of the **IceCold Core** module and is designed to be easily configurable and extensible.

## Key Features

*   **Property-Based API:** Instead of saving monolithic data objects, you manage individual properties (like player score, name, or settings).
*   **Type-Safe:** Uses generics (`IProperty<T>`) to ensure you always get and set the correct data type.
*   **Pluggable Save Methods (Strategy Pattern):** Choose where your data is saved using `ScriptableObject`s. Out-of-the-box support for `PlayerPrefs` and persistent local files.
*   **Extensible:** Easily create your own save methods to support cloud saves, encrypted files, or any other backend.
*   **Automatic Save Strategies:** Configure the service to save data automatically at intervals, aggressively on every change, or only when you command it.
*   **Safe Asynchronous Quitting:** Implements the `IQuittingService` interface to ensure all data is safely saved during the application quit sequence, even for slow operations, without freezing the game.
*   **JSON Serialization:** Uses the robust Newtonsoft JSON library to serialize any data type.

---

## Installation

The Save Service is a module that depends on the **IceCold Core** foundation. The recommended installation method is via the **IceCold Installer**.

1.  **Install the Installer:** If you haven't already, add the installer to your project via the Unity Package Manager using the Git URL: `https://github.com/Iskaald/IceColdInstaller.git`.
2.  **Install the Core:** Open the installer from **IceCold > Installer**. Find **IceCold Core** in the list and install it.
3.  **Install the Save Service:** In the same installer window, find **IceCold Save Service** and click **Install**. The installer will automatically handle all dependencies, including the required Newtonsoft JSON package.

---

## Core Concepts

Understanding these three components is key to using the system effectively.

*   **`ISaveService`**: This is the main service and your primary entry point. You get it from the `Core` and use it to access all other features.
*   **`IProperty<T>`**: A "smart variable." It holds your data (e.g., an `int` for score) and knows how to save and load itself using the configured save method. You interact with its `.Value` property just like a normal variable.
*   **`ISaveMethod`**: The "backend" that defines *where* and *how* data is saved. This is a `ScriptableObject` you assign in a config file, allowing you to switch from saving in `PlayerPrefs` to saving in separate files without changing any of your game logic code. You can implement your own method, eg. for cloud save.

---

## Setup & Configuration

After installation, you need to create two configuration assets.

1.  **Create a Save Method Asset:**
    Decide where you want to save your data. Go to **Assets > Create > IceCold > Save System** and choose one:
    *   **Local Method:** Saves data using Unity's `PlayerPrefs`. Simple and good for prototypes or web builds.
    *   **Persistent Method:** Saves each property as a separate `.json` file in the `Application.persistentDataPath`. This is more robust and suitable for standalone builds.
    *   **Your Method:** You can write you own implementation and choose it to handle the saving.

2.  **Create the Main Save Service Config:**
    *   Go to **Assets > Create > IceCold > Save System > Save Service Config**.
    *   This asset controls the behavior of the `SaveService`.

3.  **Assign the Save Method and Strategy:**
    *   Select your newly created `SaveServiceConfig` asset.
    *   Drag your `LocalSaveMethodSO` or `PersistentSaveMethodSO`, or your self implemented asset into the **Save Method** slot.
    *   Choose a **Save Strategy**:
        *   **Manual:** Data is only saved when you explicitly call `SaveAll()`.
        *   **OnQuit:** Data is automatically saved when the application quits. (This is the default behavior if the strategy is not Manual).
        *   **Aggressive:** Saves a property every time its value is requested via `GetProperty`. Use with caution.
        *   **Interval:** Automatically saves all properties every `X` seconds, defined by the `Save Interval In Seconds` field.

---

## Usage Guide

Using the service in your code is straightforward.

### 1. Get the Service

First, get an instance of the `ISaveService` from the `Core`, typically in an `Initialize` method or `Start`.

```csharp
using IceCold.SaveService.Interface;

public class GameManager
{
    private ISaveService saveService;

    public void Initialize()
    {
        saveService = Core.GetService<ISaveService>();
    }
}
```

### 2. Get or Create a Property

Use the `GetProperty<T>(key, defaultValue)` method. The system will load the property if it exists or create it with the default value if it doesn't.

```csharp
private IProperty<int> score;
private IProperty<string> playerName;
private IProperty<bool> hasCompletedTutorial;

public void Initialize()
{
    saveService = Core.GetService<ISaveService>();
    
    // Define the properties you want to track
    score = saveService.GetProperty<int>("player_score", 0);
    playerName = saveService.GetProperty<string>("player_name", "Adventurer");
    hasCompletedTutorial = saveService.GetProperty<bool>("tutorial_complete", false);
}
```

### 3. Read and Write Data

Interact with the `.Value` property of your `IProperty<T>` object.

```csharp
public void PrintScore()
{
    Debug.Log($"Current score for {playerName.Value} is: {score.Value}");
}

public void AddScore(int points)
{
    score.Value += points;
    Debug.Log($"New score is: {score.Value}");
}

public void CompleteTutorial()
{
    hasCompletedTutorial.Value = true;
}
```

### 4. Saving Manually

If you are using the `Manual` save strategy, you must call `SaveAll()` to persist the data. This method is asynchronous.

```csharp
public async Task SaveGameAsync()
{
    Debug.Log("Saving game state...");
    await saveService.SaveAll();
    Debug.Log("Game saved!");
}
```

---

## Advanced Topics

### Save on Quit Behavior

The `SaveService` implements the `IQuittingService` interface. This means that if your **Save Strategy** is set to anything other than `Manual`, the service will automatically call `SaveAll()` when the user tries to quit the application. This process is fully asynchronous, preventing the game from freezing while it saves and ensuring data is not lost.

### Creating a Custom Save Method

You can easily extend the system to save to a cloud backend, an encrypted file, or a database.

1.  **Create a new class** that implements the `ISaveMethod` interface.
2.  **Implement the logic** for `SaveProperty(key, jsonValue)` and `Exists(key, out value)`.
3.  **(Recommended)** Create a `ScriptableObject` wrapper that inherits from `SaveMethodSO` to make your new method assignable in the `SaveServiceConfig`.

### Utility: Clearing Save Data

For debugging purposes, you can easily clear all saved data.

*   **For Local Method:** Call the static method `LocalSaveMethod.Clear()`.
*   **For Persistent Method:** Call `PersistentSaveMethod.Clear("your_directory_name")`. You can find the directory name in your `PersistentSaveMethodSO` asset.