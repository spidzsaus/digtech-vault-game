namespace GameComponents;

public class Pipe
{
    public bool state = false;
    BaseGate source;
    public int source_slot;
    BaseGate dest;
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
    Pipe[] ?delivers;
    Pipe[] ?recievers;
    protected int input_slots;
    protected int output_slots;
    
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
}

public class AndGate: BaseGate
{   
    public override bool[] output(bool[] input) {
        return new bool[1] {input[0] && input[1]};
    }
}

public class CurrentGate: BaseGate
{
    public override void configure(){
        this.input_slots = 0;
        this.output_slots = 2;
    }
    public override bool[] output(bool[] input) {
        return new bool[2] {true, true};
    }
}

public class NotGate: BaseGate
{
    public override void configure(){
        this.input_slots = 1;
        this.output_slots = 1;
    }
    public override bool[] output(bool[] input) {
        return new bool[1] {!input[0]};
    }
}

public class LogGate: BaseGate
{
    public override void configure(){
        this.input_slots = 1;
        this.output_slots = 0;
    }
    public override bool[] output(bool[] input) {
        Console.WriteLine(input[0].ToString());
        return new bool[1] {input[0]};
    }
}