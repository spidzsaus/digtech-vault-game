using GameComponents;

namespace LogiWidgets;

public class LogiComponent {
    BaseGate reference;

    public LogiComponent(BaseGate reference){
        this.reference = reference;
    }

    public void draw(PaintEventArgs e, int x, int y, float scale){
        this.reference.draw(e, x, y, scale, DrawStandart.IEC);
    }

}

public class LevelViewer : Panel {
    Level level;
    int XShift;
    int YShift;
    int scale;
    DrawStandart standart;

    public LevelViewer() : base() {
        this.standart = DrawStandart.IEC;
    }
    public void openLevel(Level level) {
        this.level = level;
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
        base.OnPaint(e);
        if (this.level != null){
            foreach (BaseGate component in this.level.scheme.getGates())
            {
                component.draw(e, XShift, YShift, scale, standart);
            }
        }
    }
}