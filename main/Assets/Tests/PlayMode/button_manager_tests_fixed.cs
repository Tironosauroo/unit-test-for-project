/// @file button_manager_tests_fixed.cs
/// @brief This file contains unit tests for the ButtonManager class in a Unity game environment.
///        It uses NUnit framework to test UI panel management functionalities like showing/hiding credits and settings panels.
///        The tests ensure proper activation and deactivation of panels without exceptions.
/// @details The ButtonManager is assumed to handle UI transitions in the game, such as switching between main, credits, and settings panels.
///          Tests cover panel visibility changes, exception handling, and setup/teardown for test isolation.
///          All tests are written for Unity's test runner and use reflection to set private fields for testing.

using NUnit.Framework;
using UnityEngine;

/// @brief Test class for ButtonManager UI functionality.
/// @details This class provides unit tests to validate the ButtonManager's methods for panel management.
///          It includes setup and teardown to create and destroy test objects, ensuring clean test environments.
///          Tests focus on state changes of UI panels and exception-free operations.
public class ButtonManagerTests
{
    private GameObject managerObj;
    private ButtonManager manager;
    private GameObject mainPanel;
    private GameObject creditsPanel;
    private GameObject settingsPanel;

    /// @brief Sets up the test environment before each test.
    /// @details Creates a ButtonManager component and panel GameObjects, initializes their active states,
    ///          and uses reflection to assign private fields for testing purposes.
    /// @throws None (setup failures would be handled by NUnit).
    [SetUp]
    public void Setup()
    {
        managerObj = new GameObject("ButtonManager");
        manager = managerObj.AddComponent<ButtonManager>();

        mainPanel = new GameObject("MainPanel");
        creditsPanel = new GameObject("CreditsPanel");
        settingsPanel = new GameObject("SettingsPanel");

        mainPanel.SetActive(true);
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);

        typeof(ButtonManager)
            .GetField("MainPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(manager, mainPanel);
        typeof(ButtonManager)
            .GetField("CreditsPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(manager, creditsPanel);
        typeof(ButtonManager)
            .GetField("SettingsPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(manager, settingsPanel);
    }

    /// @brief Tears down the test environment after each test.
    /// @details Destroys all created GameObjects to prevent memory leaks and ensure test isolation.
    /// @throws None (teardown failures would be handled by NUnit).
    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(managerObj);
        Object.DestroyImmediate(mainPanel);
        Object.DestroyImmediate(creditsPanel);
        Object.DestroyImmediate(settingsPanel);
    }

    /// @brief Tests that ShowCredits enables the credits panel and disables the main panel.
    /// @details Verifies the correct panel states after calling ShowCredits on the manager.
    /// @throws None (test assertions handle failures).
    [Test]
    public void ShowCredits_EnablesCreditsPanel_DisablesMainPanel()
    {
        manager.ShowCredits();
        Assert.IsFalse(mainPanel.activeSelf, "Main panel should be hidden after showing credits.");
        Assert.IsTrue(creditsPanel.activeSelf, "Credits panel should be active.");
    }

    /// @brief Tests that HideCredits disables the credits panel and enables the main panel.
    /// @details Ensures that hiding credits reverts the panel states correctly.
    /// @throws None (test assertions handle failures).
    [Test]
    public void HideCredits_DisablesCreditsPanel_EnablesMainPanel()
    {
        manager.ShowCredits();
        manager.HideCredits();
        Assert.IsTrue(mainPanel.activeSelf, "Main panel should be active after hiding credits.");
        Assert.IsFalse(creditsPanel.activeSelf, "Credits panel should be hidden.");
    }

    /// @brief Tests that ShowSettings enables the settings panel and disables the main panel.
    /// @details Verifies the correct panel states after calling ShowSettings on the manager.
    /// @throws None (test assertions handle failures).
    [Test]
    public void ShowSettings_EnablesSettingsPanel_DisablesMainPanel()
    {
        manager.ShowSettings();
        Assert.IsFalse(mainPanel.activeSelf, "Main panel should be hidden after showing settings.");
        Assert.IsTrue(settingsPanel.activeSelf, "Settings panel should be active.");
    }

    /// @brief Tests that HideSettings disables the settings panel and enables the main panel.
    /// @details Ensures that hiding settings reverts the panel states correctly.
    /// @throws None (test assertions handle failures).
    [Test]
    public void HideSettings_DisablesSettingsPanel_EnablesMainPanel()
    {
        manager.ShowSettings();
        manager.HideSettings();
        Assert.IsTrue(mainPanel.activeSelf, "Main panel should be re-enabled after hiding settings.");
        Assert.IsFalse(settingsPanel.activeSelf, "Settings panel should be hidden.");
    }

    /// @brief Tests that PlayGame does not throw any exceptions.
    /// @details Validates that the PlayGame method executes without errors, ensuring safe game transitions.
    /// @throws None (test assertions handle failures; method should not throw).
    [Test]
    public void PlayGame_DoesNotThrowException()
    {
        Assert.DoesNotThrow(() => manager.PlayGame(), "PlayGame should not throw exceptions.");
    }
}
