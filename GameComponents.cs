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

abstract public class BaseGate
{
    public Pipe[] ?delivers;
    public Pipe[] ?recievers;
    public int input_slots;
    public int output_slots;

    public int gamefieldX;
    public int gamefieldY;

    static char symbol;

    public void setPosition(int x, int y) {
        this.gamefieldX = x;
        this.gamefieldY = y;
    }
    
    abstract public bool[] output(bool[] input);
    
    public void doChainTick() {
        bool[] input = new bool[this.input_slots];
        foreach (Pipe pipe in this.delivers) {
            input.SetValue(pipe.state, pipe.dest_slot);
        }
        bool[] output = this.output(input);
        foreach (Pipe pipe in this.recievers) {
            pipe.setState(output[pipe.source_slot]);
            pipe.doChainTick();
        }
    }

    public Pipe connect(BaseGate other, int from_slot, int to_slot) {
        Pipe bond = new Pipe(this, from_slot,
                             other, to_slot);
        this.recievers.SetValue(bond, from_slot);
        other.delivers.SetValue(bond, to_slot);
        return bond;
    }

    virtual public void configure(){
        this.input_slots = 2;
        this.output_slots = 1;
    }
    public BaseGate(){
        this.configure();
        this.delivers = new Pipe[this.input_slots];
        this.recievers = new Pipe[this.output_slots];
    }

    virtual public char getSymbol(){
        return symbol;
    }
}

public class AndGate: BaseGate
{   
    static char symbol = '&';
    public override bool[] output(bool[] input) {
        return new bool[1] {input[0] && input[1]};
    }
    public override char getSymbol(){
        return symbol;
    }
}

public class CurrentGate: BaseGate
{
    static char symbol = 'C';
    public override void configure(){
        this.input_slots = 0;
        this.output_slots = 2;
    }
    public override bool[] output(bool[] input) {
        return new bool[2] {true, true};
    }
    public override char getSymbol(){
        return symbol;
    }
}

public class NotGate: BaseGate
{
    static char symbol = '!';
    public override void configure(){
        this.input_slots = 1;
        this.output_slots = 1;
    }
    public override bool[] output(bool[] input) {
        return new bool[1] {!input[0]};
    }
    public override char getSymbol(){
        return symbol;
    }
}

public class LogGate: BaseGate
{
    static char symbol = '.';
    public override void configure(){
        this.input_slots = 1;
        this.output_slots = 0;
    }
    public override bool[] output(bool[] input) {
        Console.WriteLine(input[0].ToString());
        return new bool[1] {input[0]};
    }
    public override char getSymbol(){
        return symbol;
    }
}