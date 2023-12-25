public class Ice : NodeAction
{
    public Ice(int x, int y) : base(x, y) 
    {
        MovementBlock = true;
        CascadeBlock = true;
    }
}
