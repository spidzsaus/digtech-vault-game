using GameComponents;
using Levels;

namespace LogiWidgets;

public class RawLevelViewer: Panel {
    protected Scenes.Scene scene;
    protected Level level;
    protected int XShift;
    protected int YShift;
    protected int scale;
    protected int bufferSpace = 10;
    public bool Enabled = true;
    protected DrawStandart standart;
    public RawLevelViewer(Scenes.Scene scene) : base() {
        this.standart = DrawStandart.IEC;
        this.MouseClick += panelClick;
        this.MouseMove += panelMouseMove;
        this.scene = scene;
        this.DoubleBuffered = true;
    }
    protected override void OnResize(EventArgs e) {
        base.OnResize(e);
        this.Refresh();
    }
    protected virtual void panelClick(object sender, MouseEventArgs e)
    {}
    protected virtual void panelMouseMove(object sender, MouseEventArgs e)
    {}
    public void openLevel(Level level) {
        this.level = level;
        this.Width = this.XShift + this.scale + (int)(this.scale * 1.5f * level.scheme.getRangeX()) + bufferSpace;
        this.Height = this.YShift + this.scale + (int)(this.scale * 1.5f * level.scheme.getRangeY()) + bufferSpace;
        this.level.init();
    }
    public void openLevelRaw(Level level) {
        this.level = level;
        this.level.init();
    }
    public void setStandart(DrawStandart standart) {
        this.standart = standart;
    }

    public void configure(int XShift, int YShift, int scale) {
        this.XShift = XShift;
        this.YShift = YShift;
        this.scale = scale;
        if (level != null) {
        this.Width = this.XShift + this.scale + (int)(this.scale * 1.5f * level.scheme.getRangeX()) + bufferSpace;
        this.Height = this.YShift + this.scale + (int)(this.scale * 1.5f * level.scheme.getRangeY()) + bufferSpace;
        }
    }
    public void configure_raw(int XShift, int YShift, int scale) {
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

public class LevelViewer : RawLevelViewer {
    protected Scenes.GameScene scene;
    public LevelViewer(Scenes.GameScene scene) : base(scene) {
        this.standart = DrawStandart.IEC;
        this.MouseClick += panelClick;
        this.scene = scene;
        this.DoubleBuffered = true;
    }
    protected virtual void panelClick(object sender, MouseEventArgs e)
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
}



public class LevelEditorViewer : RawLevelViewer {
    protected Scenes.LevelEditor scene;
    bool isMoving;
    int mouseRelX;
    int mouseRelY;
    public LevelEditorViewer(Scenes.LevelEditor scene) : base(scene) {
        this.standart = DrawStandart.IEC;
        this.scene = scene;
        this.DoubleBuffered = true;
    }
    protected override void OnPaint(PaintEventArgs e) {
        if (this.level != null){
            foreach (BaseGate component in this.level.scheme.getGates())
            {
                if (component == this.scene.connectionFrom) {
                    component.draw(e, XShift, YShift, scale, standart, null, this.scene.connectionFromSlot);
                } else if (component == this.scene.connectionTo) {
                    component.draw(e, XShift, YShift, scale, standart, this.scene.connectionToSlot, null);
                } else {
                    component.draw(e, XShift, YShift, scale, standart, null, null);
                }
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
    protected override void panelMouseMove(object sender, MouseEventArgs e)
    {
        base.panelMouseMove(sender, e);
        if (this.isMoving) {
            int mouseX = e.X;
            int mouseY = e.Y;
            this.XShift += (mouseX - mouseRelX);
            this.YShift += (mouseY - mouseRelY);
            if (this.XShift > 0) this.XShift = 0;
            if (this.YShift > 0) this.YShift = 0;
            this.Refresh();
            mouseRelX = mouseX;
            mouseRelY = mouseY;
        }
    }
    protected override void panelClick(object sender, MouseEventArgs e)
    {
        if (this.Enabled) {
            int mouseX = e.X;
            int mouseY = e.Y;
            float trueX = (mouseX - this.XShift) / (this.scale * 1.5f);
            float trueY = (mouseY - this.YShift) / (this.scale * 1.5f);
            int componentX = (int)System.Math.Floor(trueX);
            int componentY = (int)System.Math.Floor(trueY);
            float relX = trueX - componentX;
            float relY = trueY - componentY;
            BaseGate? component = this.level.scheme.getComponent(componentX, componentY);
            switch (this.scene.mode)
            {
                case Scenes.LevelEditorMode.MoveAround:
                mouseRelX = mouseX;
                mouseRelY = mouseY;
                isMoving = !isMoving;
                break;
                case Scenes.LevelEditorMode.PasteGate:
                if (component == null && this.scene.getSelectedGateType() != null) {
                    BaseGate newgate = Alliases.createGateFromTypeFullName(this.scene.getSelectedGateType());
                    newgate.setPosition(componentX, componentY);
                    this.level.scheme.addGate(newgate);
                    this.Refresh();
                }
                break;
                case Scenes.LevelEditorMode.RemoveGate:
                if (component != null) { 
                    this.level.scheme.removeGate(component);
                    this.Refresh(); }
                break;
                case Scenes.LevelEditorMode.SetHidden:
                if (component != null) {
                    component.is_hidden = !component.is_hidden;
                    this.Refresh();
                }
                break;
                case Scenes.LevelEditorMode.ConnectGates:
                if (component != null) {
                    if (relX >= 0.5 && component.output_slots > 0) {
                        if (this.scene.connectionFrom == component) {
                            this.scene.connectionFrom = null;
                            this.scene.connectionFromSlot = -1;
                            this.Refresh();
                        } else {
                            this.scene.connectionFrom = component;
                            this.scene.connectionFromSlot = (int)(relY * component.output_slots + 0.5f);
                            this.Refresh();
                        }
                    } else if (relX < 0.5  && component.input_slots > 0) {
                        if (this.scene.connectionTo == component) {
                            this.scene.connectionTo = null;
                            this.scene.connectionToSlot = -1;
                            this.Refresh();
                        } else {
                            this.scene.connectionTo = component;
                            this.scene.connectionToSlot = (int)(relY * component.input_slots + 0.5f);
                            this.Refresh();
                        }
                    }
                    
                    if (this.scene.connectionFrom != null && this.scene.connectionTo != null) {
                        this.scene.connectionFrom.connect(this.scene.connectionTo,
                                                          this.scene.connectionFromSlot,
                                                          this.scene.connectionToSlot);

                        this.scene.connectionTo = null;
                        this.scene.connectionFrom = null;
                        this.scene.connectionToSlot = -1;
                        this.scene.connectionFromSlot = -1;
                        this.Refresh();

                    }
                }
                break; 
                
            }
            //if (component != null) { 
            //    bool result = component.onClick();
            //    if (result) { 
            //        this.Refresh();
            //        this.scene.unlockCommitButton();
            //    }
            //}
        }
    }
}