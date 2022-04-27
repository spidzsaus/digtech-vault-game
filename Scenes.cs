namespace Scenes;

using LogiWidgets;
using Textures;

public class SceneManager : Panel {
    public SceneManager() : base() {
        //this.BackgroundImage = Textures.bath_tile;
        this.BackColor = Color.FromArgb(172, 213, 207);
    }
    Scene? scene;
    public System.Media.SoundPlayer player;
    public void openScene(Scene scene) {
        this.Parent.SuspendLayout();
        if (this.scene is not null) {
            this.scene.close();
        }
        this.scene = scene;
        this.scene.init(this);
        this.Parent.ResumeLayout();

    }
    override protected void OnResize(EventArgs e) {
        base.OnResize(e);
        this.SuspendLayout();
        if (this.scene != null) this.scene.resize();
        this.ResumeLayout();
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

public enum LevelEditorMode {DoNothing, SetHidden, RemoveGate, PasteGate, ConnectGates, MoveAround};
public class LevelEditor : Scene {
    Levels.Level level;
    public string selectedType; 
    TextBox levelName;
    public LevelEditorMode mode;
    public GameComponents.BaseGate? connectionTo;
    public GameComponents.BaseGate? connectionFrom;
    public int connectionToSlot;
    public int connectionFromSlot;

    ComboBox gateType;
    Button backToTheMenuButton;
    Button pasteGateButton;
    Button saveAsButton;
    Button openButton;
    Button connectButton;
    Button deleteButton;
    Button setHiddenButton;
    Button navButton;
    LevelEditorViewer viewer;
    public dynamic getSelectedGateType(){
        return this.gateType.SelectedItem;
    }
    public int optimalScale() {
        int HoverA = (parent.Height - 100) / (this.level.scheme.getRangeY() + 1);
        int WoverB = parent.Width / (this.level.scheme.getRangeX() + 1);
        return (int)((HoverA < WoverB ? HoverA : WoverB) / 1.5);
    }
    public void switchToConnect(object sender, EventArgs e) {
        this.mode = LevelEditorMode.ConnectGates;
        this.connectButton.BackgroundImage = Textures.button_golden;
        this.setHiddenButton.BackgroundImage = Textures.bath_tile;
        this.deleteButton.BackgroundImage = Textures.bath_tile;
        this.navButton.BackgroundImage = Textures.bath_tile;
        this.pasteGateButton.BackgroundImage = Textures.bath_tile;
    }
    public void switchToSetHidden(object sender, EventArgs e) {
        this.mode = LevelEditorMode.SetHidden;
        this.setHiddenButton.BackgroundImage = Textures.button_golden;
        this.connectButton.BackgroundImage = Textures.bath_tile;
        this.deleteButton.BackgroundImage = Textures.bath_tile;
        this.navButton.BackgroundImage = Textures.bath_tile;
        this.pasteGateButton.BackgroundImage = Textures.bath_tile;
    }
    public void switchToDelete(object sender, EventArgs e) {
        this.mode = LevelEditorMode.RemoveGate;
        this.deleteButton.BackgroundImage = Textures.button_golden;
        this.setHiddenButton.BackgroundImage = Textures.bath_tile;
        this.connectButton.BackgroundImage = Textures.bath_tile;
        this.navButton.BackgroundImage = Textures.bath_tile;
        this.pasteGateButton.BackgroundImage = Textures.bath_tile;
    }
    public void switchToPaste(object sender, EventArgs e) {
        this.mode = LevelEditorMode.PasteGate;
        this.deleteButton.BackgroundImage = Textures.bath_tile;
        this.setHiddenButton.BackgroundImage = Textures.bath_tile;
        this.connectButton.BackgroundImage = Textures.bath_tile;
        this.navButton.BackgroundImage = Textures.bath_tile;
        this.pasteGateButton.BackgroundImage = Textures.button_golden;
    }

    public void switchToNav(object sender, EventArgs e) {
        this.mode = LevelEditorMode.MoveAround;
        this.deleteButton.BackgroundImage = Textures.bath_tile;
        this.setHiddenButton.BackgroundImage = Textures.bath_tile;
        this.connectButton.BackgroundImage = Textures.bath_tile;
        this.navButton.BackgroundImage = Textures.button_golden;
        this.pasteGateButton.BackgroundImage = Textures.bath_tile;
    }
    public void saveLevelAs(object sender, EventArgs e) {
        string path ;
        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
        {
            saveFileDialog.InitialDirectory = @"/gamedata/levels";
            saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog.FileName;
                this.level.levelName = this.levelName.Text;
                string json = this.level.toJson();
                System.IO.File.WriteAllText(path, json);
            }
        }
    }

    public void openLevelFrom(object sender, EventArgs e) {
        string path ;
        using (OpenFileDialog saveFileDialog = new OpenFileDialog())
        {
            saveFileDialog.InitialDirectory = @"/gamedata/levels";
            saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog.FileName;
                string json = System.IO.File.ReadAllText(path);
                this.level.fromJson(json, false);
                this.viewer.Refresh();
                this.levelName.Text = this.level.levelName;
            }
        }
    }
    public override void init(SceneManager parent)
    {
        base.init(parent);

        this.levelName = new();
        this.levelName.Location = new(0, 0);
        this.levelName.Width = 300;
        this.levelName.Height = 40;
        this.levelName.Text = "";
        this.levelName.Font = Textures.small_font; 
        parent.Controls.Add(this.levelName);

        this.saveAsButton = new();
        this.saveAsButton.Location = new(300, 0);
        this.saveAsButton.Width = 150;
        this.saveAsButton.Height = 40;
        this.saveAsButton.Text = "Save As";
        this.saveAsButton.BackgroundImage = Textures.button_green;
        this.saveAsButton.Font = Textures.big_font;
        this.saveAsButton.Click += this.saveLevelAs;
        parent.Controls.Add(this.saveAsButton);
        this.openButton = new();
        this.openButton.Location = new(450, 0);
        this.openButton.Width = 150;
        this.openButton.Height = 40;
        this.openButton.Text = "Open";
        this.openButton.BackgroundImage = Textures.button_green;
        this.openButton.Font = Textures.big_font;
        this.openButton.Click += this.openLevelFrom;
        parent.Controls.Add(this.openButton);

        this.gateType = new();
        this.gateType.Items.AddRange(GameComponents.Alliases.fullnameMeanings.Keys.ToArray());
        this.gateType.DropDownStyle = ComboBoxStyle.DropDownList;
        this.gateType.Location = new(0, 140);
        this.gateType.Width = 150;
        this.gateType.Height = 50;
        this.gateType.SelectedIndexChanged += this.switchToPaste;
        parent.Controls.Add(this.gateType);

        this.level = new();
        this.level.init();
        this.mode = LevelEditorMode.DoNothing;

        this.pasteGateButton = new();
        this.pasteGateButton.Location = new(0, 40);
        this.pasteGateButton.Width = 150;
        this.pasteGateButton.Height = 100;
        this.pasteGateButton.Text = "+\nPlace gate";
        this.pasteGateButton.BackgroundImage = Textures.bath_tile;
        this.pasteGateButton.Font = Textures.big_font;
        this.pasteGateButton.Click += this.switchToPaste;
        parent.Controls.Add(this.pasteGateButton);

        this.viewer = new(this);
        this.viewer.Location = new(0, 190);
        this.viewer.Width = parent.Width;
        this.viewer.Height = parent.Height - 190;
        this.viewer.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
        this.viewer.configure_raw(10, 10, 100);
        this.viewer.openLevelRaw(this.level);
        this.viewer.BackColor = Color.Linen;

        this.navButton = new();
        this.navButton.Location = new(450, 40);
        this.navButton.Width = 150;
        this.navButton.Height = 150;
        this.navButton.Text = "⬦\nNavigate";
        this.navButton.BackgroundImage = Textures.bath_tile;
        this.navButton.Font = Textures.big_font;
        this.navButton.Click += this.switchToNav;
        parent.Controls.Add(this.navButton);

        this.connectButton = new();
        this.connectButton.Location = new(600, 40);
        this.connectButton.Width = 150;
        this.connectButton.Height = 150;
        this.connectButton.Text = "⟷\nConnect gates";
        this.connectButton.BackgroundImage = Textures.bath_tile;
        this.connectButton.Font = Textures.big_font;
        this.connectButton.Click += this.switchToConnect;
        parent.Controls.Add(this.connectButton);

        this.setHiddenButton = new();
        this.setHiddenButton.Location = new(150, 40);
        this.setHiddenButton.Width = 150;
        this.setHiddenButton.Height = 150;
        this.setHiddenButton.Text = "?\nSet visibility";
        this.setHiddenButton.BackgroundImage = Textures.bath_tile;
        this.setHiddenButton.Font = Textures.big_font;
        this.setHiddenButton.Click += this.switchToSetHidden;
        parent.Controls.Add(this.setHiddenButton);

        this.deleteButton = new();
        this.deleteButton.Location = new(300, 40);
        this.deleteButton.Width = 150;
        this.deleteButton.Height = 150;
        this.deleteButton.Text = "╳\nDelete gate";
        this.deleteButton.BackgroundImage = Textures.bath_tile;
        this.deleteButton.Font = Textures.big_font;
        this.deleteButton.Click += this.switchToDelete;
        parent.Controls.Add(this.deleteButton);

        parent.Controls.Add(this.viewer);
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

    public void openCertificate(object sender, EventArgs e) {
        string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        int i = userName.LastIndexOf('\\');
        userName = userName.Substring(i + 1, userName.Length - i - 1);
        string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + this.level.certificatePath(userName).Replace('/', '\\');
        System.Diagnostics.Process.Start("explorer.exe" , @"/select," + path);
    }

    public void win() {
        System.Media.SoundPlayer temp = new System.Media.SoundPlayer(@"resources/win.wav");
        temp.Play();
        this.Active = false;
        this.levelViewer.Enabled = false;
        string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        int i = userName.LastIndexOf('\\');
        userName = userName.Substring(i + 1, userName.Length - i - 1);
        this.level.createCertificateFile(userName);

        this.backToTheMenuButton = new();
        this.backToTheMenuButton.Location = new(410, 0);
        this.backToTheMenuButton.Width = 200;
        this.backToTheMenuButton.Height = 100;
        this.backToTheMenuButton.Text = "Back to the menu";
        this.backToTheMenuButton.Click += this.backToTheMenu;
        this.backToTheMenuButton.BackgroundImage = Textures.button_reddish;
        this.backToTheMenuButton.Font = Textures.big_font;

        this.firstButton = new();
        this.firstButton.Location = new(210, 0);
        this.firstButton.Width = 200;
        this.firstButton.Height = 100;
        this.firstButton.Text = "View certificate";
        this.firstButton.BackgroundImage = Textures.button_golden;
        this.firstButton.Font = Textures.big_font;
        this.firstButton.Click += this.openCertificate;
        parent.Controls.Add(this.backToTheMenuButton);
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
        this.backToTheMenuButton.Font = Textures.big_font;

        this.firstButton = new();
        this.firstButton.Location = new(210, 0);
        this.firstButton.Width = 200;
        this.firstButton.Height = 100;
        this.firstButton.Text = "Try again";
        this.firstButton.Click += this.retry;
        this.firstButton.BackgroundImage = Textures.button_golden;
        this.firstButton.Font = Textures.big_font;
        parent.Controls.Add(this.backToTheMenuButton);
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
                this.commitButton.Text = "Run (" + this.turnsLeft.ToString() + ")";
            } else {
                this.fail();
            }
        }

    }
    public int optimalScale() {
        int HoverA = (parent.Height - 100) / (this.level.scheme.getRangeY() + 1);
        int WoverB = parent.Width / (this.level.scheme.getRangeX() + 1);
        return (int)((HoverA < WoverB ? HoverA : WoverB) / 1.5);
    }

    public override void resize()
    {
        this.levelViewer.configure(10, 10, this.optimalScale());

    }
    public override void init(SceneManager parent)
    {
        base.init(parent);
        this.level.init();
        this.turnsLeft = this.level.turns;
        this.levelViewer = new(this);
        this.levelViewer.Location = new(0, 100);
        this.levelViewer.configure(10, 10, this.optimalScale());
        this.levelViewer.openLevel(this.level);
        this.levelViewer.BackColor = Color.Linen;
        //this.levelViewer.Anchor = (AnchorStyles.Bottom);

        this.commitButton = new();
        this.commitButton.Width = 200;
        this.commitButton.Height = 100;
        this.commitButton.Text = "Run (" + this.turnsLeft.ToString() + ")";
        this.commitButton.Click += this.commit;
        this.commitButton.Enabled = false;
        this.commitButton.BackgroundImage = Textures.button_green;
        this.commitButton.Font = Textures.big_font;
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
    static int maxLevelsPerPage = 6;
    public void backToTheMenu(object sender, EventArgs e) {
        this.parent.openScene(new MenuScene());
    }
    public void openPage(int page) {
        int start = maxLevelsPerPage * page;
        int finish = (int)(start + maxLevelsPerPage);
        if (finish > levels.Length) finish = levels.Length;

        this.parent.SuspendLayout();
        if (this.levelButtons != null) {
            foreach (Button item in levelButtons)
            {
                this.parent.Controls.Remove(item);
                item.Dispose();
            }
        }
        levelButtons = new Button[finish - start];

        for (int i = finish - 1; i >= start; i--)
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
            this.levelButtons[j].Click += openThisLevel;
            this.levelButtons[j].Text = curlevel.levelName;
            this.levelButtons[j].BackgroundImage = Textures.button_gray;
            this.levelButtons[j].Dock = DockStyle.Top;
            this.levelButtons[j].Font = Textures.big_font;
            parent.Controls.Add(this.levelButtons[j]);
        }
        this.parent.ResumeLayout();
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
        string?[] levelPaths = System.IO.Directory.GetFiles(@"gamedata/levels/", "*.json",
                                            System.IO.SearchOption.TopDirectoryOnly);
        System.Array.Sort(levelPaths);
        Levels.Level[] prelevels = new Levels.Level[levelPaths.Length];
        for (int i = 0; i < levelPaths.Length; i++)
        {
            prelevels[i] = new();
            bool result = prelevels[i].fromJson(System.IO.File.ReadAllText(levelPaths[i]), true);
            if (result) {
                Levels.Level curlevel = prelevels[i];
                string curpath = levelPaths[i];
                curlevel.levelPath = curpath;
            } else {
                levelPaths[i] = null;
                prelevels[i] = null;
            }
        }
        this.levels = prelevels.Where(c => c != null).ToArray();
        levelPaths = levelPaths.Where(c => c != null).ToArray();
        this.maxPage = (int)System.Math.Ceiling((double)(this.levels.Length / maxLevelsPerPage));

        this.nextPageButton = new();
        this.nextPageButton.Click += this.nextPage;
        this.nextPageButton.Text = ">";
        this.nextPageButton.BackgroundImage = Textures.button_green;
        this.nextPageButton.Width = 200;
        this.nextPageButton.Height = 180;
        this.nextPageButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
        this.nextPageButton.Location = new(parent.Width - 200, parent.Height - 180);
        this.nextPageButton.Font = Textures.big_font;

        this.prevPageButton = new();
        this.prevPageButton.Click += this.prevPage;
        this.prevPageButton.Text = "<";
        this.prevPageButton.BackgroundImage = Textures.button_green;
        this.prevPageButton.Width = 200;
        this.prevPageButton.Height = 180;
        this.prevPageButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
        this.prevPageButton.Location = new(0, parent.Height - 180);
        this.prevPageButton.Font = Textures.big_font;

        this.backToTheMenuButton = new();
        this.backToTheMenuButton.Anchor = AnchorStyles.Bottom;
        this.backToTheMenuButton.Location = new(parent.Width / 2 - 200, parent.Height - 180);
        this.backToTheMenuButton.Width = 400;
        this.backToTheMenuButton.Height = 180;
        this.backToTheMenuButton.Text = "Back to the menu";
        this.backToTheMenuButton.Click += this.backToTheMenu;
        this.backToTheMenuButton.BackgroundImage = Textures.button_reddish;
        this.backToTheMenuButton.Font = Textures.big_font;
        parent.Controls.Add(this.nextPageButton);
        parent.Controls.Add(this.prevPageButton);
        parent.Controls.Add(this.backToTheMenuButton);
        this.nextPageButton.Enabled = (currentPage < this.maxPage);
        this.prevPageButton.Enabled = (currentPage > 0);

        openPage(0);
    }

}

public class CertificateValidationScene : Scene {
    Button backToTheMenuButton;
    Button openCertificateButton;
    TextBox certificatePathView;
    Button compareButton;
    Label resultMessage;
    override public void init(SceneManager parent) {
        base.init(parent);

        this.openCertificateButton = new();
        this.openCertificateButton.Anchor = (AnchorStyles.Top);
        this.openCertificateButton.Location = new(parent.Width / 2 - 300, 350);
        this.openCertificateButton.Width = 600;
        this.openCertificateButton.Height = 100;
        this.openCertificateButton.Click += this.openCertificate;
        this.openCertificateButton.BackgroundImage = Textures.button_gray;
        this.openCertificateButton.Text = "Select the certificate";
        this.openCertificateButton.Font = Textures.big_font;

        this.certificatePathView = new();
        this.certificatePathView.Anchor = (AnchorStyles.Top);
        this.certificatePathView.Location = new(parent.Width / 2 - 300, 450);
        this.certificatePathView.Width = 600;
        this.certificatePathView.Height = 30;
        this.certificatePathView.Text = "";
        this.certificatePathView.Font = Textures.small_font; 

        this.compareButton = new();
        this.compareButton.Anchor = (AnchorStyles.Top);
        this.compareButton.Location = new(parent.Width / 2 - 300, 600);
        this.compareButton.Width = 600;
        this.compareButton.Height = 100;
        this.compareButton.Click += this.compare;
        this.compareButton.BackgroundImage = Textures.button_green;
        this.compareButton.Text = "Validate";
        this.compareButton.Font = Textures.big_font;

        this.resultMessage = new();
        this.resultMessage.Anchor = (AnchorStyles.Top);
        this.resultMessage.Location = new(parent.Width / 2 - 300, 700);
        this.resultMessage.Width = 600;
        this.resultMessage.Height = 50;
        this.resultMessage.Click += this.compare;
        this.resultMessage.BackgroundImage = Textures.button_gray;
        this.resultMessage.Text = "Validation result";
        this.resultMessage.Font = Textures.big_font;

        this.backToTheMenuButton = new();
        this.backToTheMenuButton.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
        this.backToTheMenuButton.Location = new(parent.Width - 150, 0);
        this.backToTheMenuButton.Width = 150;
        this.backToTheMenuButton.Height = 100;
        this.backToTheMenuButton.Text = "Back";
        this.backToTheMenuButton.Click += this.backToTheMenu;
        this.backToTheMenuButton.BackgroundImage = Textures.button_reddish;
        this.backToTheMenuButton.Font = Textures.big_font;
        parent.Controls.Add(this.backToTheMenuButton);

        parent.Controls.Add(openCertificateButton);
        parent.Controls.Add(certificatePathView);
        parent.Controls.Add(compareButton);
        parent.Controls.Add(resultMessage);
    }
    public void backToTheMenu(object sender, EventArgs e) {
        this.parent.openScene(new MenuScene());
    }

    void compare(object sender, EventArgs e) {
        bool result = File.Exists(this.certificatePathView.Text);
        if (result) {
            result = Levels.CertificateManager.validateCertificate(this.certificatePathView.Text);
        }
        if (result) {
            this.resultMessage.BackgroundImage = Textures.button_green;
            this.resultMessage.Text = "Certificate is valid!";
        } else {
            this.resultMessage.BackgroundImage = Textures.button_reddish;
            this.resultMessage.Text = "Certificate is invalid!";
        }
    }

    void openCertificate(object sender, EventArgs e) {

        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.InitialDirectory = @"/gamedata/certificates";
            openFileDialog.Filter = "Certificate files (*.certificate)|*.certificate|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.certificatePathView.Text = openFileDialog.FileName;
            }
        }
    }
}

public class MenuScene : Scene {
    Button gameStart;
    Button exit;
    Button certificateValidator;
    PictureBox logo;
    override public void init(SceneManager parent) {
        base.init(parent);
        this.logo = new PictureBox();            
        this.logo.ImageLocation = @"resources/logo.png";
        this.logo.SizeMode = PictureBoxSizeMode.AutoSize;
        //this.logo.BackgroundImage = Textures.bath_tile;
        this.logo.Anchor = AnchorStyles.Top;
        this.logo.Location = new(parent.Width / 2 - 350, 0);
        parent.Controls.Add(this.logo);

        this.gameStart = new();
        this.gameStart.Anchor = (AnchorStyles.Top);
        this.gameStart.Location = new(parent.Width / 2 - 300, 300);
        this.gameStart.Width = 600;
        this.gameStart.Height = 100;
        this.gameStart.Click += this.startGame;
        this.gameStart.BackgroundImage = Textures.button_gray;
        this.gameStart.Text = "Play";
        this.gameStart.Font = Textures.big_font;

        this.certificateValidator = new();
        this.certificateValidator.Anchor = (AnchorStyles.Top);
        this.certificateValidator.Location = new(parent.Width / 2 - 300, 400);
        this.certificateValidator.Width = 600;
        this.certificateValidator.Height = 100;
        this.certificateValidator.Click += this.certificates;
        this.certificateValidator.BackgroundImage = Textures.button_gray;
        this.certificateValidator.Text = "Level Editor";
        this.certificateValidator.Font = Textures.big_font;

        this.exit = new();
        this.exit.Anchor = (AnchorStyles.Top);
        this.exit.Location = new(parent.Width / 2 - 300, 500);
        this.exit.Width = 600;
        this.exit.Height = 100;
        this.exit.Click += this.exitAction;
        this.exit.BackgroundImage = Textures.button_reddish;
        this.exit.Text = "Exit";
        this.exit.Font = Textures.big_font;
        
        if (parent.player != null) parent.player.Stop();
        parent.player = new System.Media.SoundPlayer(@"resources/menu.wav");
        parent.player.PlayLooping();

        parent.Controls.Add(gameStart);
        parent.Controls.Add(exit);
        parent.Controls.Add(certificateValidator);
    }

    public void startGame(object sender, EventArgs e) {
        this.parent.openScene(new LevelSelectScene());
    }    


    public void certificates(object sender, EventArgs e) {
        this.parent.openScene(new LevelEditor());
    }   
    public void exitAction(object sender, EventArgs e) {
        this.close();
        Application.Exit();
    }
}
