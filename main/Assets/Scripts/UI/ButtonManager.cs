/// @file ButtonManager.cs
/// @brief This file contains the ButtonManager class, which is responsible for managing UI button interactions in a Unity game.
///        It handles actions such as exiting the application, showing/hiding panels (Credits and Settings), and starting the game.
///        The class is designed for use in the main menu scene and assumes the presence of a LevelManager script for scene loading.

using UnityEngine;

/// @brief A MonoBehaviour class that manages button interactions for the main menu in a Unity game.
///        This class controls the visibility of UI panels and handles game start and exit functionality.
///        It is attached to a GameObject in the scene and uses serialized fields to reference UI panels.
public class ButtonManager : MonoBehaviour
{
    /// @brief Reference to the main menu panel GameObject.
    ///        This panel is typically active by default and is toggled when showing/hiding other panels.
    [SerializeField]
    private GameObject MainPanel;

    /// @brief Reference to the credits panel GameObject.
    ///        This panel displays game credits and is shown/hidden via button interactions.
    [SerializeField]
    private GameObject CreditsPanel;

    /// @brief Reference to the settings panel GameObject.
    ///        This panel allows users to adjust game settings and is shown/hidden via button interactions.
    [SerializeField]
    private GameObject SettingsPanel;

    /// @brief Exits the application when called (typically from an exit button).
    ///        This method calls Application.Quit(), which works in built applications but does nothing in the Unity editor.
    ///        @throws None. Application.Quit() does not throw exceptions.
    public void ExitButton()
    {
        Application.Quit();
    }

    /// @brief Shows the credits panel and hides the main panel.
    ///        This is called when the user clicks a "Credits" button.
    ///        @throws None. SetActive does not throw exceptions.
    public void ShowCredits()
    {
        MainPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }

    /// @brief Hides the credits panel and shows the main panel.
    ///        This is called when the user clicks a "Back" or close button from the credits panel.
    ///        @throws None. SetActive does not throw exceptions.
    public void HideCredits()
    {
        CreditsPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    /// @brief Shows the settings panel and hides the main panel.
    ///        This is called when the user clicks a "Settings" button.
    ///        @throws None. SetActive does not throw exceptions.
    public void ShowSettings()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    /// @brief Hides the settings panel and shows the main panel.
    ///        This is called when the user clicks a "Back" or close button from the settings panel.
    ///        @throws None. SetActive does not throw exceptions.
    public void HideSettings()
    {
        SettingsPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    /// @brief Starts the game by loading the first scene.
    ///        This method delegates to LevelManager.LoadFirstScene(), assuming LevelManager is a static class or singleton.
    ///        @throws System.Exception if LevelManager.LoadFirstScene() throws (e.g., if the scene does not exist).
    public void PlayGame()
    {
        LevelManager.LoadFirstScene();
    }
}