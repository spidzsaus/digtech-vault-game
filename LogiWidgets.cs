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
