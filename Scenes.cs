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
    override protected void OnResize(EventArgs e) {
        base.OnResize(e);
        if (this.scene != null) this.scene.resize();
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
    public virtual void resize() {}
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
    public override void resize()
    {
        this.levelViewer.configure(10, 10, (parent.Width < parent.Height ? parent.Width : parent.Height) / 8);

    }
    public override void init(SceneManager parent)
    {
        base.init(parent);
        this.level.init();
        this.turnsLeft = this.level.turns;
        this.levelViewer = new(this);
        this.levelViewer.Location = new(0, 100);
        this.levelViewer.configure(10, 10, (parent.Width < parent.Height ? parent.Width : parent.Height) / 8);
        this.levelViewer.openLevel(this.level);
        //this.levelViewer.Anchor = (AnchorStyles.Bottom);

        this.commitButton = new();
        this.commitButton.Width = 200;
        this.commitButton.Height = 100;
        this.commitButton.Text = "Commit (" + this.turnsLeft.ToString() + ")";
        this.commitButton.Click += this.commit;
        this.commitButton.Enabled = false;
        this.commitButton.BackgroundImage = Textures.button_green;
        //this.commitButton.Anchor = (AnchorStyles.Left | AnchorStyles.Top);
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
    Button nextPageButton;
    Button prevPageButton;
    Button backToTheMenuButton;
    int maxPage;
    int currentPage;
    static int maxLevelsPerPage = 4;
    public void backToTheMenu(object sender, EventArgs e) {
        this.parent.openScene(new MenuScene());
    }
    public void openPage(int page) {
        int start = maxLevelsPerPage * page;
        int finish = (int)(start + maxLevelsPerPage);
        if (finish > levels.Length) finish = levels.Length;

        if (this.levelButtons != null) {
            foreach (Button item in levelButtons)
            {
                this.parent.Controls.Remove(item);
                item.Dispose();
            }
        }
        levelButtons = new Button[finish - start];

        for (int i = start; i < finish; i++)
        {
            Levels.Level curlevel = this.levels[i];
            void openThisLevel(object sender, EventArgs e){
                curlevel.fromJson(System.IO.File.ReadAllText(curlevel.levelPath), false);
                this.parent.openScene(new GameScene(curlevel));  
            }
            int j = i - start;
            this.levelButtons[j] = new();
            this.levelButtons[j].Width = 100;
            this.levelButtons[j].Height = 50;
            this.levelButtons[j].Location = new(10, 50 * j);
            this.levelButtons[j].Click += openThisLevel;
            this.levelButtons[j].Text = curlevel.levelName;
            this.levelButtons[j].BackgroundImage = Textures.button_gray;
            this.levelButtons[j].Dock = DockStyle.Top;
            parent.Controls.Add(this.levelButtons[j]);
        }
    }

    public void nextPage(object sender, EventArgs e) {
        this.currentPage += 1;
        openPage(this.currentPage);
        this.nextPageButton.Enabled = (currentPage < this.maxPage);
        this.prevPageButton.Enabled = (currentPage > 0);
    }
    public void prevPage(object sender, EventArgs e) {
        this.currentPage -= 1;
        openPage(this.currentPage);
        this.nextPageButton.Enabled = (currentPage < this.maxPage);
        this.prevPageButton.Enabled = (currentPage > 0);
    }
    
    public override void init(SceneManager parent)
    {
        base.init(parent);
        string[] levelPaths = System.IO.Directory.GetFiles(@"gamedata/levels/", "*.json",
                                            System.IO.SearchOption.TopDirectoryOnly);
        System.Array.Sort(levelPaths);
        this.levels = new Levels.Level[levelPaths.Length];
        for (int i = 0; i < levelPaths.Length; i++)
        {
            this.levels[i] = new();
            this.levels[i].fromJson(System.IO.File.ReadAllText(levelPaths[i]), true);
            Levels.Level curlevel = this.levels[i];
            string curpath = levelPaths[i];
            curlevel.levelPath = curpath;
        }
        this.maxPage = (int)System.Math.Ceiling((double)(this.levels.Length / maxLevelsPerPage));

        this.nextPageButton = new();
        this.nextPageButton.Click += this.nextPage;
        this.nextPageButton.Text = ">";
        this.nextPageButton.BackgroundImage = Textures.button_green;
        this.nextPageButton.Width = 200;
        this.nextPageButton.Height = 180;
        this.nextPageButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
        this.nextPageButton.Location = new(parent.Width - 200, parent.Height - 180);
        parent.Controls.Add(this.nextPageButton);

        this.prevPageButton = new();
        this.prevPageButton.Click += this.prevPage;
        this.prevPageButton.Text = "<";
        this.prevPageButton.BackgroundImage = Textures.button_green;
        this.prevPageButton.Width = 200;
        this.prevPageButton.Height = 180;
        this.prevPageButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
        this.prevPageButton.Location = new(0, parent.Height - 180);
        this.prevPageButton.Enabled = false;
        parent.Controls.Add(this.prevPageButton);

        this.backToTheMenuButton = new();
        this.backToTheMenuButton.Anchor = AnchorStyles.Bottom;
        this.backToTheMenuButton.Location = new(parent.Width / 2 - 200, parent.Height - 180);
        this.backToTheMenuButton.Width = 400;
        this.backToTheMenuButton.Height = 180;
        this.backToTheMenuButton.Text = "Back to the menu";
        this.backToTheMenuButton.Click += this.backToTheMenu;
        this.backToTheMenuButton.BackgroundImage = Textures.button_reddish;
        parent.Controls.Add(this.backToTheMenuButton);

        openPage(0);
    }

}

public class MenuScene : Scene {
    Button gameStart;
    Button exit;
    PictureBox logo;
    override public void init(SceneManager parent) {
        base.init(parent);
        this.logo = new PictureBox();            
        this.logo.ImageLocation = @"resources/logo.png";
        this.logo.SizeMode = PictureBoxSizeMode.AutoSize;
        this.logo.BackgroundImage = Textures.bath_tile;
        this.logo.Anchor = AnchorStyles.Top;
        this.logo.Location = new(parent.Width / 2 - 350, 0);
        parent.Controls.Add(this.logo);

        this.gameStart = new();
        this.gameStart.Anchor = (AnchorStyles.Top);
        this.gameStart.Location = new(parent.Width / 2 - 300, 300);
        this.gameStart.Width = 600;
        this.gameStart.Height = 100;
        this.gameStart.Click += this.openTestScene;
        this.gameStart.BackgroundImage = Textures.button_gray;

        this.exit = new();
        this.exit.Anchor = (AnchorStyles.Top);
        this.exit.Location = new(parent.Width / 2 - 300, 400);
        this.exit.Width = 600;
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
