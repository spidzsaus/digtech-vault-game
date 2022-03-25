using GameComponents;

namespace digtech_vault_game;

static class Program
{
    [STAThread]
    static void Main()
    {   
        GameComponents.Scheme mainScheme = new();
        ApplicationConfiguration.Initialize();

        GameComponents.ButtonInputGate i1 = new();
        GameComponents.ButtonInputGate i2 = new();
        GameComponents.ButtonInputGate i3 = new();
        GameComponents.ButtonInputGate i4 = new();
        GameComponents.LampOutputGate o1 = new();
        GameComponents.XorGate xor = new();
        GameComponents.AndGate and = new();
        GameComponents.NotGate not = new();
        GameComponents.OrGate xnor = new();
        
        i1.connect(xor, 0, 0);
        i2.connect(xor, 0, 1);
        i3.connect(not, 0, 0);
        xor.connect(and, 0, 0);
        i4.connect(and, 0, 1);  
        not.connect(xnor, 0, 1);
        and.connect(xnor, 0, 0);     
        xnor.connect(o1, 0, 0); 

        i1.setPosition(0, 0);
        i2.setPosition(0, 2);
        i3.setPosition(0, 3);
        i4.setPosition(0, 1);
        xor.setPosition(1, 0);
        and.setPosition(2, 1);
        not.setPosition(2, 3);
        xnor.setPosition(3, 2);
        o1.setPosition(4, 1);

        mainScheme.addGate(i1);
        mainScheme.addGate(i2);
        mainScheme.addGate(i3);
        mainScheme.addGate(i4);
        mainScheme.addGate(not);
        mainScheme.addGate(xnor);
        mainScheme.addGate(xor);
        mainScheme.addGate(o1);
        mainScheme.addGate(and);

        mainScheme.compile();

        foreach (var pair in mainScheme.truthTable!) {
            Console.WriteLine($"{pair.Key[0]} : {pair.Value[0]}");
        }

        Form1 mainForm = new Form1();

        Level mainLevel = new Level();
        mainLevel.scheme = mainScheme;

        mainForm.openLevel(mainLevel);

        Application.Run(mainForm);
    }    
}