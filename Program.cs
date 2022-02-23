using GameComponents;

namespace digtech_vault_game;

static class Program
{
    [STAThread]
    static void Main()
    {
        GameComponents.AndGate and = new GameComponents.AndGate();
        GameComponents.CurrentGate curr = new GameComponents.CurrentGate();
        GameComponents.NotGate not = new GameComponents.NotGate();
        GameComponents.LogGate log = new GameComponents.LogGate();
        
        and.setPosition(2, 1);
        curr.setPosition(0, 1);
        not.setPosition(1, 2);
        log.setPosition(3, 1);


        curr.connect(and, 0, 0);
        curr.connect(not, 1, 0);
        not.connect(and, 0, 1);
        and.connect(log, 0, 0);
        
        curr.doChainTick();

        ApplicationConfiguration.Initialize();

        Form1 mainForm = new Form1();

        mainForm.addGate(and);
        mainForm.addGate(curr);
        mainForm.addGate(not);
        mainForm.addGate(log);

        Application.Run(mainForm);
    }    
}