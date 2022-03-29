using GameComponents;
using Scenes;
using Levels;

namespace digtech_vault_game;

static class Program
{
    [STAThread]
    static void Main()
    {   
        string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        int i = userName.LastIndexOf('\\');
        userName = userName.Substring(i + 1, userName.Length - i - 1);

        string path = @"gamedata/profile/" + userName + ".card";
        if (!File.Exists(path)) {
            System.Security.Cryptography.RNGCryptoServiceProvider rngCsp = new();
            System.IO.Directory.CreateDirectory(@"gamedata/profile/");
            using (FileStream fs = File.Create(path))
            {

                byte[] info = new byte[500];
                rngCsp.GetBytes(info);
                fs.Write(info, 0, info.Length);
            }
        }

        ApplicationConfiguration.Initialize();
        Form1 mainForm = new Form1();
        MenuScene mainMenu = new();
        mainForm.sceneManager.openScene(mainMenu);
        
        Application.Run(mainForm);
    }    
}