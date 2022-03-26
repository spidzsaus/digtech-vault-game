namespace Scenes;


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
    public Control? parent;
    public virtual void init(Control parent) {
        this.parent = parent;
    }
    public virtual void close() {
        this.parent.Controls.Clear();
    }
}

public class MenuScene : Scene {
    Button gameStart;
    Button exit;
    override public void init(Control parent) {
        base.init(parent);
        this.gameStart = new();
        this.gameStart.Location = new(100, 100);
        this.gameStart.Width = 300;
        this.gameStart.Height = 100;
        this.exit = new();
        this.exit.Location = new(100, 200);
        this.exit.Width = 300;
        this.exit.Height = 100;
        this.exit.Click += this.exitAction;
        

        parent.Controls.Add(gameStart);
        parent.Controls.Add(exit);
    }
        

    public void exitAction(object sender, EventArgs e) {
        this.close();
        Application.Exit();
    }
}
