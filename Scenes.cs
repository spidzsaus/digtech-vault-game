namespace Scenes;

using LogiWidgets;

public class SceneManager : Panel {
    Scene? scene;
    public System.Media.SoundPlayer player;
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

public class GameScene : Scene {
    public GameScene(Levels.Level level) {
        this.level = level;
    }
    Levels.Level level;
    LevelViewer levelViewer;
    public override void init(SceneManager parent)
    {
        base.init(parent);

        this.levelViewer = new();
        this.levelViewer.configure(10, 10, 100);
        this.levelViewer.openLevel(this.level);

        parent.Controls.Add(this.levelViewer);

        if (parent.player != null) parent.player.Stop();
        parent.player = new System.Media.SoundPlayer(@"resources/game.wav");
        parent.player.PlayLooping();


    }
}

public class LevelSelectScene : Scene {
    Levels.Level[] levels;
    Button[] levelButtons;
    public override void init(SceneManager parent)
    {
        base.init(parent);
        string[] levelPaths = System.IO.Directory.GetFiles(@"gamedata/levels/", "*.json",
                                            System.IO.SearchOption.TopDirectoryOnly);
        this.levels = new Levels.Level[levelPaths.Length];
        this.levelButtons = new Button[levelPaths.Length];
        for (int i = 0; i < levelPaths.Length; i++)
        {
            this.levels[i] = new();
            this.levels[i].fromJson(System.IO.File.ReadAllText(levelPaths[i]), true);
            Levels.Level curlevel = this.levels[i];
            string curpath = levelPaths[i];
            void openThisLevel(object sender, EventArgs e){
                curlevel.fromJson(System.IO.File.ReadAllText(curpath), false);
                this.parent.openScene(new GameScene(curlevel));
            }
            this.levelButtons[i] = new();
            this.levelButtons[i].Width = 100;
            this.levelButtons[i].Height = 50;
            this.levelButtons[i].Location = new(10, 50 * i);
            this.levelButtons[i].Click += openThisLevel;
            this.levelButtons[i].Text = curlevel.levelName;
            parent.Controls.Add(this.levelButtons[i]);
        }


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
        
        if (parent.player != null) parent.player.Stop();
        parent.player = new System.Media.SoundPlayer(@"resources/menu.wav");
        parent.player.PlayLooping();

        parent.Controls.Add(gameStart);
        parent.Controls.Add(exit);
    }

    public void openTestScene(object sender, EventArgs e) {
        this.parent.openScene(new LevelSelectScene());
    }    

    public void exitAction(object sender, EventArgs e) {
        this.close();
        Application.Exit();
    }
}
