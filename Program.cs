using GameComponents;
using Scenes;
using Levels;

namespace digtech_vault_game;

static class Program
{
    [STAThread]
    static void Main()
    {   
        ApplicationConfiguration.Initialize();
        Form1 mainForm = new Form1();
        MenuScene mainMenu = new();
        mainForm.sceneManager.openScene(mainMenu);
        Application.Run(mainForm);
    }    
}