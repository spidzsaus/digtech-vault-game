namespace Scenes;

using LogiWidgets;
using Textures;

public class SceneManager : Panel {
    public SceneManager() : base() {
        this.BackgroundImage = Textures.bath_tile;
    }
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
    Button commitButton;
    
    Button backToTheMenuButton;
    Button firstButton;
    bool Active = true;
    int turnsLeft;
    LevelViewer levelViewer;
    public void unlockCommitButton() {
        this.commitButton.Enabled = true;
    }
    public void backToTheMenu(object sender, EventArgs e) {
        this.parent.openScene(new LevelSelectScene());
        parent.player = new System.Media.SoundPlayer(@"resources/menu.wav");
        parent.player.PlayLooping();
    }

    public void retry(object sender, EventArgs e) {
        this.level.fromJson(System.IO.File.ReadAllText(this.level.levelPath!), false);
        this.parent.openScene(new GameScene(this.level));
    }

    public void win() {
        System.Media.SoundPlayer temp = new System.Media.SoundPlayer(@"resources/win.wav");
        temp.Play();
        this.Active = false;
        this.levelViewer.Enabled = false;

        this.backToTheMenuButton = new();
        this.backToTheMenuButton.Location = new(410, 0);
        this.backToTheMenuButton.Width = 200;
        this.backToTheMenuButton.Height = 100;
        this.backToTheMenuButton.Text = "Back to the menu";
        this.backToTheMenuButton.Click += this.backToTheMenu;
        this.backToTheMenuButton.BackgroundImage = Textures.button_reddish;
        parent.Controls.Add(this.backToTheMenuButton);

        this.firstButton = new();
        this.firstButton.Location = new(210, 0);
        this.firstButton.Width = 200;
        this.firstButton.Height = 100;
        this.firstButton.Text = "View certificate";
        this.firstButton.Click += this.retry;
        this.firstButton.BackgroundImage = Textures.button_golden;
        parent.Controls.Add(this.firstButton);
    }
    public void fail() {
        System.Media.SoundPlayer temp = new System.Media.SoundPlayer(@"resources/fail.wav");
        temp.Play();
        this.Active = false;
        this.levelViewer.Enabled = false;

        this.backToTheMenuButton = new();
        this.backToTheMenuButton.Location = new(410, 0);
        this.backToTheMenuButton.Width = 200;
        this.backToTheMenuButton.Height = 100;
        this.backToTheMenuButton.Text = "Back to the menu";
        this.backToTheMenuButton.Click += this.backToTheMenu;
        this.backToTheMenuButton.BackgroundImage = Textures.button_reddish;
        parent.Controls.Add(this.backToTheMenuButton);

        this.firstButton = new();
        this.firstButton.Location = new(210, 0);
        this.firstButton.Width = 200;
        this.firstButton.Height = 100;
        this.firstButton.Text = "Try again";
        this.firstButton.Click += this.retry;
        this.firstButton.BackgroundImage = Textures.button_golden;
        parent.Controls.Add(this.firstButton);
    }
    public void commit(object sender, EventArgs e) {
        bool[] result = this.level.scheme.run();
        this.levelViewer.Refresh();
        this.commitButton.Enabled  = false;
        this.turnsLeft -= 1;
        if (!result.Contains(false)) {
            this.win();
        } else {
            if (this.turnsLeft >= 0) {
                this.commitButton.Text = "Commit (" + this.turnsLeft.ToString() + ")";
            } else {
                this.fail();
            }
        }

    }
    public override void init(SceneManager parent)
    {
        base.init(parent);
        this.level.init();
        this.turnsLeft = this.level.turns;
        this.levelViewer = new(this);
        this.levelViewer.Location = new(0, 100);
        this.levelViewer.configure(10, 10, 100);
        this.levelViewer.openLevel(this.level);

        this.commitButton = new();
        this.commitButton.Width = 200;
        this.commitButton.Height = 100;
        this.commitButton.Text = "Commit (" + this.turnsLeft.ToString() + ")";
        this.commitButton.Click += this.commit;
        this.commitButton.Enabled = false;
        this.commitButton.BackgroundImage = Textures.button_green;
        parent.Controls.Add(this.commitButton);


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
                curlevel.levelPath = curpath;
                this.parent.openScene(new GameScene(curlevel));  
            }
            this.levelButtons[i] = new();
            this.levelButtons[i].Width = 100;
            this.levelButtons[i].Height = 50;
            this.levelButtons[i].Location = new(10, 50 * i);
            this.levelButtons[i].Click += openThisLevel;
            this.levelButtons[i].Text = curlevel.levelName;
            this.levelButtons[i].BackgroundImage = Textures.button_gray;
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
        this.gameStart.BackgroundImage = Textures.button_gray;
        this.exit = new();
        this.exit.Location = new(100, 200);
        this.exit.Width = 300;
        this.exit.Height = 100;
        this.exit.Click += this.exitAction;
        this.exit.BackgroundImage = Textures.button_reddish;
        
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
