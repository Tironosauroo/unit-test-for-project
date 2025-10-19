using NUnit.Framework;
using UnityEngine;

public class ButtonManagerTests
{
    private GameObject managerObj;
    private ButtonManager manager;
    private GameObject mainPanel;
    private GameObject creditsPanel;
    private GameObject settingsPanel;

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

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(managerObj);
        Object.DestroyImmediate(mainPanel);
        Object.DestroyImmediate(creditsPanel);
        Object.DestroyImmediate(settingsPanel);
    }

    [Test]
    public void ShowCredits_EnablesCreditsPanel_DisablesMainPanel()
    {
        manager.ShowCredits();
        Assert.IsFalse(mainPanel.activeSelf, "Main panel should be hidden after showing credits.");
        Assert.IsTrue(creditsPanel.activeSelf, "Credits panel should be active.");
    }

    [Test]
    public void HideCredits_DisablesCreditsPanel_EnablesMainPanel()
    {
        manager.ShowCredits();
        manager.HideCredits();
        Assert.IsTrue(mainPanel.activeSelf, "Main panel should be active after hiding credits.");
        Assert.IsFalse(creditsPanel.activeSelf, "Credits panel should be hidden.");
    }

    [Test]
    public void ShowSettings_EnablesSettingsPanel_DisablesMainPanel()
    {
        manager.ShowSettings();
        Assert.IsFalse(mainPanel.activeSelf, "Main panel should be hidden after showing settings.");
        Assert.IsTrue(settingsPanel.activeSelf, "Settings panel should be active.");
    }

    [Test]
    public void HideSettings_DisablesSettingsPanel_EnablesMainPanel()
    {
        manager.ShowSettings();
        manager.HideSettings();
        Assert.IsTrue(mainPanel.activeSelf, "Main panel should be re-enabled after hiding settings.");
        Assert.IsFalse(settingsPanel.activeSelf, "Settings panel should be hidden.");
    }

    [Test]
    public void PlayGame_DoesNotThrowException()
    {
        Assert.DoesNotThrow(() => manager.PlayGame(), "PlayGame should not throw exceptions.");
    }
}
