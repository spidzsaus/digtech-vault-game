namespace Scenes;

using LogiWidgets;

public class SceneManager : Panel {
    Scene? scene;
    public void openScene(Scene scene) {
        if (this.scene is not null) {
            this.scene.close();
        }
        this.scene = scene;
        this.scene.init(this);
    }
}

abstract public class Scene {
    public SceneManager? parent;
    public virtual void init(SceneManager parent) {
        this.parent = parent;
    }
    public virtual void close() {
        this.parent.Controls.Clear();
    }
}

public class TestScene : Scene {
    LevelViewer levelViewer;
    public override void init(SceneManager parent)
    {
        base.init(parent);
        GameComponents.Scheme mainScheme = new();
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

        Levels.Level mainLevel = new();
        mainLevel.levelName = "Test";
        mainLevel.scheme = mainScheme;

        this.levelViewer = new();
        this.levelViewer.configure(10, 10, 100);
        this.levelViewer.openLevel(mainLevel);

        parent.Controls.Add(this.levelViewer);
    }
}

public class MenuScene : Scene {
    Button gameStart;
    Button exit;
    override public void init(SceneManager parent) {
        base.init(parent);
        this.gameStart = new();
        this.gameStart.Location = new(100, 100);
        this.gameStart.Width = 300;
        this.gameStart.Height = 100;
        this.gameStart.Click += this.openTestScene;
        this.exit = new();
        this.exit.Location = new(100, 200);
        this.exit.Width = 300;
        this.exit.Height = 100;
        this.exit.Click += this.exitAction;
        

        parent.Controls.Add(gameStart);
        parent.Controls.Add(exit);
    }

    public void openTestScene(object sender, EventArgs e) {
        this.parent.openScene(new TestScene());
    }    

    public void exitAction(object sender, EventArgs e) {
        this.close();
        Application.Exit();
    }
}
