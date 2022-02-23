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
        
        curr.connect(and, 0, 0);
        //curr.connect(not, 1, 0);
        curr.connect(and, 1, 1);
        and.connect(log, 0, 0);
        
        curr.doChainTick();


        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }    
}