using GameComponents;
using Levels;

namespace LogiWidgets;

public class LevelViewer : Panel {
    Scenes.GameScene scene;
    Level level;
    int XShift;
    int YShift;
    int scale;
    int bufferSpace = 10;
    public bool Enabled = true;
    DrawStandart standart;
    public LevelViewer(Scenes.GameScene scene) : base() {
        this.standart = DrawStandart.IEC;
        this.MouseClick += panelClick;
        this.scene = scene;
    }
    protected void panelClick(object sender, MouseEventArgs e)
    {
        if (this.Enabled) {
            int mouseX = e.X;
            int mouseY = e.Y;
            int componentX = (int)System.Math.Floor((mouseX - this.XShift) / (this.scale * 1.5f));
            int componentY = (int)System.Math.Floor((mouseY - this.YShift) / (this.scale * 1.5f));
            BaseGate? component = this.level.scheme.getComponent(componentX, componentY);
            if (component != null) { 
                bool result = component.onClick();
                if (result) { 
                    this.Refresh();
                    this.scene.unlockCommitButton();
                }
            }
        }
    }
    public void openLevel(Level level) {
        this.level = level;
        this.Width = this.XShift + this.scale + (int)(this.scale * 1.5f * level.scheme.getRangeX()) + bufferSpace;
        this.Height = this.YShift + this.scale + (int)(this.scale * 1.5f * level.scheme.getRangeY()) + bufferSpace;
        this.level.init();
    }
    public void setStandart(DrawStandart standart) {
        this.standart = standart;
    }

    public void configure(int XShift, int YShift, int scale) {
        this.XShift = XShift;
        this.YShift = YShift;
        this.scale = scale;
    }

    protected override void OnPaint(PaintEventArgs e) {
        if (this.level != null){
            foreach (BaseGate component in this.level.scheme.getGates())
            {
                component.draw(e, XShift, YShift, scale, standart);
            }
        }
        Pen pen = new Pen(Color.Black);
        pen.Width = 5;
        e.Graphics.DrawLine(pen,
                            0,
                            this.Height,
                            this.Width,
                            this.Height);
        e.Graphics.DrawLine(pen,
                            this.Width,
                            0,
                            this.Width,
                            this.Height);
        base.OnPaint(e);
    }
}