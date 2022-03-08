using GameComponents;

namespace digtech_vault_game;

static class Program
{
    [STAThread]
    static void Main()
    {   
        GameComponents.Scheme mainScheme = new();


        GameComponents.DummyInputGate i1 = new();
        GameComponents.DummyInputGate i2 = new();
        GameComponents.DummyOutputGate o1 = new();
        GameComponents.AndGate and = new();
        
        i1.connect(and, 0, 0);
        i2.connect(and, 0, 1);
        and.connect(o1, 0, 0);

        mainScheme.addGate(i1);
        mainScheme.addGate(i2);
        mainScheme.addGate(o1);
        mainScheme.addGate(and);

        mainScheme.compile();

        Console.WriteLine(mainScheme.run(new bool[2] {true, true})[0]);

        ApplicationConfiguration.Initialize();

        Form1 mainForm = new Form1();

        mainForm.addGate(i1);
        mainForm.addGate(i2);
        mainForm.addGate(o1);
        mainForm.addGate(and);

        Application.Run(mainForm);
    }    
}