namespace GameComponents;
public class Pipe
{
    public bool state = false;
    public BaseGate source;
    public int source_slot;
    public BaseGate dest;
    public int dest_slot;

    public Pipe(BaseGate source, int source_slot,
                BaseGate dest,   int dest_slot){
        this.source = source;
        this.source_slot = source_slot;
        this.dest = dest;
        this.dest_slot = dest_slot;
    }

    public void doChainTick() {
        this.dest.doChainTick();
    }

    public void setState(bool state) {
        this.state = state;
    }
}

public enum DrawStandart {IEC, ANSI};


abstract public class BaseGate
{
    public Pipe[] ?delivers;
    public Pipe[] ?recievers;
    virtual public int input_slots {get;}
    virtual public int output_slots {get;}
    virtual public bool is_input {get => false;}
    virtual public bool is_output {get => false;}
    abstract public bool[] output(bool[] input);
    abstract public void IECdraw(PaintEventArgs e, int x, int y, float scale);
    abstract public void ANSIdraw(PaintEventArgs e, int x, int y, float scale);
    public int parentsCount {get { return this.countParents(); } }

    public Scheme Parent;
    public bool suggestion;
    public int gamefieldX;
    public int gamefieldY;
    public bool is_hidden;
    protected static Font _drawFont = new Font("Arial", 20);
    protected static StringFormat _drawFormat = new StringFormat();

    protected void IECDrawTemplate(PaintEventArgs e, int x, int y, float scale, string symbol, bool hasCircle) {
        float shift = scale * 1.5f;
        int componentX = (int)(gamefieldX * shift + x);
        int componentY = (int)(gamefieldY * shift + y);
        Pen pen = new Pen(Color.Black);
        pen.Width = 3;
        SolidBrush brush = new SolidBrush(Color.Black);
        e.Graphics.DrawRectangle(pen, new Rectangle(componentX, 
                                                    componentY,
                                                    (int)(scale * 0.75f),
                                                    (int)(scale)));
        e.Graphics.DrawString(symbol,
                              BaseGate._drawFont,
                              brush,
                              (int)((componentX) + 0.15f * scale),
                              componentY,
                              BaseGate._drawFormat
                              );
        
        if (hasCircle) {
            int lineStartX = (int)(componentX + scale * 0.75f); 
            float lineStartReferenceY = componentY;
            foreach (Pipe pipe in this.recievers!)
            {
                if (!(pipe is null)) {
                pen.Color = pipe.state ? Color.Red : Color.Black;
                BaseGate pipe_dest = pipe.dest;
                int lineStartY = (int)(lineStartReferenceY + ((pipe.source_slot + 0.5f) * (scale / this.output_slots)));
                int lineEndX = x + (int)(pipe_dest.gamefieldX * shift);
                int lineEndY = y + (int)(pipe_dest.gamefieldY * shift + (pipe.dest_slot + 0.5f) * (scale / pipe_dest.input_slots));
                int midX = lineStartX + (lineEndX - lineStartX) / 2;
                e.Graphics.DrawEllipse(pen,
                                       (int)lineStartX,
                                       (int)(lineStartY - scale * 0.1f),
                                       (int)((0.2f) * scale),
                                       (int)((0.2f) * scale));
                e.Graphics.DrawLine(pen,
                                    lineStartX + scale * 0.2f,
                                    lineStartY,
                                    midX,
                                    lineStartY);
                e.Graphics.DrawLine(pen,
                                    midX,
                                    lineStartY,
                                    midX,
                                    lineEndY);
                e.Graphics.DrawLine(pen,
                                    midX,
                                    lineEndY,
                                    lineEndX,
                                    lineEndY);
                }
            }
        } else {
            int lineStartX = (int)(componentX + scale * 0.75f); 
            float lineStartReferenceY = componentY;
            foreach (Pipe pipe in this.recievers!)
            {
                if (!(pipe is null)) {
                pen.Color = pipe.state ? Color.Red : Color.Black;
                BaseGate pipe_dest = pipe.dest;
                int lineStartY = (int)(lineStartReferenceY + ((pipe.source_slot + 0.5f) * (scale / this.output_slots)));
                int lineEndX = x + (int)(pipe_dest.gamefieldX * shift);
                int lineEndY = y + (int)(pipe_dest.gamefieldY * shift + (pipe.dest_slot + 0.5f) * (scale / pipe_dest.input_slots));
                int midX = lineStartX + (lineEndX - lineStartX) / 2;
                e.Graphics.DrawLine(pen,
                                    lineStartX,
                                    lineStartY,
                                    midX,
                                    lineStartY);
                e.Graphics.DrawLine(pen,
                                    midX,
                                    lineStartY,
                                    midX,
                                    lineEndY);
                e.Graphics.DrawLine(pen,
                                    midX,
                                    lineEndY,
                                    lineEndX,
                                    lineEndY);
                }
            }
        }


        pen.Dispose();
        brush.Dispose();

    }

    virtual public bool onClick () {return false;}

    public void draw(PaintEventArgs e, int x, int y, float scale, DrawStandart drawStandart) {
        if (drawStandart == DrawStandart.IEC) {
            if (this.is_hidden) {
                this.IECDrawHidden(e, x, y, scale);
            } else { 
                this.IECdraw(e, x, y, scale);
            }
        } else {
            if (this.is_hidden) {
                this.ANSIDrawHidden(e, x, y, scale);
            } else { 
                this.ANSIdraw(e, x, y, scale);
            }
        }
    }
    
    public void IECDrawHidden(PaintEventArgs e, int x, int y, float scale) {
        this.IECDrawTemplate(e, x, y, scale, " ?", false);
    }
    public void ANSIDrawHidden(PaintEventArgs e, int x, int y, float scale) {
        
    }
    public void setPosition(int x, int y) {
        this.gamefieldX = x;
        this.gamefieldY = y;
    }
    
    public void doChainTick() {
        this.update();
        foreach (Pipe pipe in this.recievers!) {
            pipe.doChainTick();
        }
    }

    public void passForced(bool[] output) {
        foreach (Pipe pipe in this.recievers!) {
            pipe.setState(output[pipe.source_slot]);
        }
    }

    public bool[] update() {
        bool[] input = new bool[input_slots];
        foreach (Pipe pipe in this.delivers!) {
            input.SetValue(pipe.state, pipe.dest_slot);
        }
        bool[] output = this.output(input);
        this.passForced(output);
        return output;
    }

    public Pipe connect(BaseGate other, int from_slot, int to_slot) {
        Pipe bond = new Pipe(this, from_slot,
                             other, to_slot);
        this.recievers!.SetValue(bond, from_slot);
        other.delivers!.SetValue(bond, to_slot);
        return bond;
    }

    public int countParents(){
        if (this.input_slots == 0) {
            return 0;
        } else {
            int maxParentParents = 0;
            foreach (Pipe pipe in this.delivers!) {
                if (pipe.source.countParents() > maxParentParents) {
                    maxParentParents = pipe.source.countParents();
                }
            }
            return maxParentParents + 1;
        }
    }

    public BaseGate(){
        this.delivers = new Pipe[input_slots];
        this.recievers = new Pipe[output_slots];
    }
}

public class AndGate: BaseGate
{   
    override public int input_slots {get => 2;}
    override public int output_slots {get => 1;}
    public override bool[] output(bool[] input) {
        return new bool[1] {input[0] && input[1]};
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, " &", false);
    }
    
}

public class NotGate: BaseGate
{   
    override public int input_slots {get => 1;}
    override public int output_slots {get => 1;}
    public override bool[] output(bool[] input) {
        return new bool[1] {!input[0]};
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, " 1", true);
    }
    
}

public class BufGate: BaseGate
{   
    override public int input_slots {get => 1;}
    override public int output_slots {get => 1;}
    public override bool[] output(bool[] input) {
        return new bool[1] {input[0]};
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, " 1", false);
    }
    
}

public class NandGate: BaseGate
{   
    override public int input_slots {get => 2;}
    override public int output_slots {get => 1;}
    public override bool[] output(bool[] input) {
        return new bool[1] {!(input[0] && input[1])};
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, " &", true);
    }
    
}

public class OrGate: BaseGate
{   
    override public int input_slots {get => 2;}
    override public int output_slots {get => 1;}
    public override bool[] output(bool[] input) {
        return new bool[1] {input[0] || input[1]};
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, "⩾1", false);
    }
    
}

public class NorGate: BaseGate
{   
    override public int input_slots {get => 2;}
    override public int output_slots {get => 1;}
    public override bool[] output(bool[] input) {
        return new bool[1] {!(input[0] || input[1])};
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, "⩾1", true);
    }
    
}

public class XorGate: BaseGate
{   
    override public int input_slots {get => 2;}
    override public int output_slots {get => 1;}
    public override bool[] output(bool[] input) {
        return new bool[1] {input[0] ^ input[1]};
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, "=1", false);
    }
    
}

public class XnorGate: BaseGate
{   
    override public int input_slots {get => 2;}
    override public int output_slots {get => 1;}
    public override bool[] output(bool[] input) {
        return new bool[1] {!(input[0] ^ input[1])};
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, "=1", true);
    }
    
}

public class DummyInputGate: BaseGate
{   
    override public int input_slots {get => 0;}
    override public int output_slots {get => 1;}
    override public bool is_input {get => true;}
    public override bool[] output(bool[] input) {
        return input;
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, ">>", false);
    }
}

public class DummyOutputGate: BaseGate
{   
    override public int input_slots {get => 1;}
    override public int output_slots {get => 0;}
    override public bool is_output {get => true;}
    public override bool[] output(bool[] input) {
        return input;
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, "<<", false);
    }
}

public class LogOutputGate: BaseGate
{   
    override public int input_slots {get => 1;}
    override public int output_slots {get => 0;}
    override public bool is_output {get => true;}
    public override bool[] output(bool[] input) {
        Console.WriteLine(input[0]);
        return input;
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        this.IECDrawTemplate(e, x, y, scale, "<<", false);
    }
}

public class LampOutputGate: BaseGate
{   
    override public int input_slots {get => 1;}
    override public int output_slots {get => 0;}
    override public bool is_output {get => true;}
    public override bool[] output(bool[] input) {
        this.suggestion = input[0];
        return input;
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        float shift = scale * 1.5f;
        int componentX = (int)(gamefieldX * shift + x);
        int componentY = (int)(gamefieldY * shift + y);
        Pen pen = new Pen(Color.Black);
        pen.Width = 10;
        SolidBrush brush;
        if (this.suggestion) {
            brush = new SolidBrush(Color.Yellow);
        } else {
            brush = new SolidBrush(Color.Gray);
        }

        e.Graphics.DrawEllipse(pen,
                                (int)componentX,
                                (int)componentY + (int)scale * 0.3f,
                                (int)scale - (int)scale * 0.6f,
                                (int)scale - (int)scale * 0.6f);
        e.Graphics.FillEllipse(brush,
                                (int)componentX,
                                (int)componentY + (int)scale * 0.3f,
                                (int)scale - (int)scale * 0.6f,
                                (int)scale - (int)scale * 0.6f);

        pen.Dispose();
        brush.Dispose();
    }
}

public class ButtonInputGate: BaseGate
{ 
    override public bool onClick() {
        this.suggestion = !this.suggestion;
        this.Parent.run();
        return true;
    }
    override public int input_slots {get => 0;}
    override public int output_slots {get => 1;}
    override public bool is_input {get => true;}
    public override bool[] output(bool[] input) {
        return input;
    }
    public override void ANSIdraw(PaintEventArgs e, int x, int y, float scale)
    {
        throw new NotImplementedException();
    }
    public override void IECdraw(PaintEventArgs e, int x, int y, float scale)
    {
        float shift = scale * 1.5f;
        int componentX = (int)(gamefieldX * shift + x);
        int componentY = (int)(gamefieldY * shift + y);
        Pen pen = new Pen(Color.Black);
        pen.Width = 10;
        SolidBrush brush;
        if (this.suggestion) {
            brush = new SolidBrush(Color.Lime);
        } else {
            brush = new SolidBrush(Color.Gray);
        }

        e.Graphics.DrawEllipse(pen,
                                (int)componentX,
                                (int)componentY,
                                (int)scale,
                                (int)scale);
        e.Graphics.FillEllipse(brush,
                                (int)componentX,
                                (int)componentY ,
                                (int)scale,
                                (int)scale);
        pen.Width = 3;
        int lineStartX = (int)(componentX + scale);
            float lineStartReferenceY = componentY;
            foreach (Pipe pipe in this.recievers!)
            {
                if (!(pipe is null)) {
                pen.Color = pipe.state ? Color.Red : Color.Black;
                BaseGate pipe_dest = pipe.dest;
                int lineStartY = (int)(lineStartReferenceY + ((pipe.source_slot + 0.5f) * (scale / this.output_slots)));
                int lineEndX = x + (int)(pipe_dest.gamefieldX * shift);
                int lineEndY = y + (int)(pipe_dest.gamefieldY * shift + (pipe.dest_slot + 0.5f) * (scale / pipe_dest.input_slots));
                int midX = lineStartX + (lineEndX - lineStartX) / 2;
                e.Graphics.DrawLine(pen,
                                    lineStartX,
                                    lineStartY,
                                    midX,
                                    lineStartY);
                e.Graphics.DrawLine(pen,
                                    midX,
                                    lineStartY,
                                    midX,
                                    lineEndY);
                e.Graphics.DrawLine(pen,
                                    midX,
                                    lineEndY,
                                    lineEndX,
                                    lineEndY);
                }
            }

        pen.Dispose();
        brush.Dispose();
    }
}

public class Scheme {
    List<BaseGate> schemeBody = new List<BaseGate>();
    List<BaseGate> inputLayer = new List<BaseGate>();
    List<BaseGate> outputLayer = new List<BaseGate>();
    public bool isCompiled = false;
    public bool isReady = false;
    public Dictionary<bool[], bool[]>? truthTable;
    private Dictionary<int, Dictionary<int, BaseGate>>? locator;
    public List<BaseGate> getGates() {
        return this.schemeBody;
    }
    public BaseGate? getComponent(int X, int Y) {
        if (!this.isReady) {
            this.prepare();
        }
        try
        {
            return this.locator[X][Y];
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            return null;
        }
    }
    public int getRangeX() {
        int maxX = 0;
        foreach (BaseGate item in this.schemeBody)
        {
            if (item.gamefieldX > maxX) {
                maxX = item.gamefieldX;
            }
        }
        return maxX;
    }
    public int getRangeY() {
        int maxY = 0;
        foreach (BaseGate item in this.schemeBody)
        {
            if (item.gamefieldY > maxY) {
                maxY = item.gamefieldY;
            }
        }
        return maxY;
    }
    public bool[] output(bool[] input) {
        if (this.isCompiled) {
            return this.truthTable![input];
        } else {
            return this.run(input);
        }
    }
    public void compile() {
        if (!this.isReady) {
            this.prepare();
        }
        this.truthTable = this.genTruthTable();
        this.isCompiled = true;
    }
    public void prepare() {
        this.locator = new();
        foreach (BaseGate gate in this.schemeBody) {
            if (!this.locator.ContainsKey(gate.gamefieldX)) {
                this.locator[gate.gamefieldX] = new();
            }
            this.locator[gate.gamefieldX][gate.gamefieldY] = gate;
            if (gate.is_input) {
                inputLayer.Add(gate);
            }
            if (gate.is_output) {
                outputLayer.Add(gate);
            }
        }
        this.schemeBody = this.schemeBody.OrderBy(bg => bg.parentsCount).ToList();
        this.isReady = true;
    }

    public bool[] run(bool[] input) {
        for (int i = 0; i < input.Length; i++)
        {
            inputLayer[i].passForced( new bool[1] {input[i]} );
        }
        foreach (BaseGate gate in this.schemeBody)
        {
            if (!gate.is_input) {
                gate.update();
            }
            
        }
        bool[] output = new bool[this.outputLayer.Count];
        for (int i = 0; i < this.outputLayer.Count; i++)
        {
            output[i] = this.outputLayer[i].update()[0];   
        }
        return output;
    }
    public bool[] run() {
        for (int i = 0; i < this.inputLayer.Count; i++)
        {
            inputLayer[i].passForced(new bool[1] {inputLayer[i].suggestion});
        }
        foreach (BaseGate gate in this.schemeBody)
        {
            if (!gate.is_input) {
                gate.update();
            }
            
        }
        bool[] output = new bool[this.outputLayer.Count];
        for (int i = 0; i < this.outputLayer.Count; i++)
        {
            output[i] = this.outputLayer[i].update()[0];   
        }
        return output;
    }

    public void addGate(BaseGate gate) {
        this.schemeBody.Add(gate);
        this.isReady = false;
        gate.Parent = this;
    }

    public Dictionary<bool[], bool[]> genTruthTable(){
        int inputSize = this.inputLayer.Count;
        int outputSize = this.outputLayer.Count;
        int tableSize = (int)Math.Pow(2, inputSize);
        Dictionary<bool[], bool[]> output = new();
        for (int i = 0; i < tableSize; i++)
        {
            bool[] key = new bool[inputSize];
            List<bool> input = new List<bool>();
            for (int j = inputSize - 1; j >= 0; j--)
            {
                int buff = (int)(i / Math.Pow(2, j));  
                key[j] = (buff % 2) == 1;
            }
            output[key] = this.run(input.ToArray());
        }
        return output;
    }


}

public class Level {
    public Scheme scheme; 
    public string levelName;

}