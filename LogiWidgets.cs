using GameComponents;

namespace LogiWidgets;

public static class DrawConstants {
public const float GATE_UNIFORM_WIDTH = 0.75f;
public const float GATE_UNIFORM_HEIGHT = 1.0f;

public static Font drawFont = new Font("Arial", 20);
public static StringFormat drawFormat = new StringFormat();
}

public class LogiComponent {
    BaseGate reference;
    int x, y;

    public LogiComponent(BaseGate reference){
        this.reference = reference;
        this.x = reference.gamefieldX;
        this.y = reference.gamefieldY;
    }

    public void draw(PaintEventArgs e, float scale){
        Pen pen = new Pen(Color.Black);
        SolidBrush brush = new SolidBrush(Color.Black);
        e.Graphics.DrawRectangle(pen, new Rectangle((int)(this.x * scale), 
                                                    (int)(this.y * scale),
                                                    (int)(scale * DrawConstants.GATE_UNIFORM_WIDTH),
                                                    (int)(scale * DrawConstants.GATE_UNIFORM_HEIGHT)));
        e.Graphics.DrawString(this.reference.getSymbol().ToString(),
                              DrawConstants.drawFont,
                              brush,
                              (int)((this.x + DrawConstants.GATE_UNIFORM_WIDTH * 0.5f) * scale),
                              (int)(this.y * scale),
                              DrawConstants.drawFormat
                              );

        foreach (Pipe pipe in this.reference.recievers)
        {
            if (!(pipe is null)) {
            BaseGate pipe_source = pipe.source;
            BaseGate pipe_dest = pipe.dest;
            e.Graphics.DrawLine(pen,
                                (int)((pipe_source.gamefieldX + DrawConstants.GATE_UNIFORM_WIDTH) * scale),
                                (int)((pipe_source.gamefieldY + ((pipe.source_slot + 0.5f) * (DrawConstants.GATE_UNIFORM_HEIGHT / pipe_source.output_slots))) * scale),
                                (int)((pipe_dest.gamefieldX) * scale),
                                (int)((pipe_dest.gamefieldY + ((pipe.dest_slot + 0.5f) * (DrawConstants.GATE_UNIFORM_HEIGHT / pipe_dest.input_slots))) * scale)
                                );
            }
        }


        pen.Dispose();
        brush.Dispose();
    }

}
