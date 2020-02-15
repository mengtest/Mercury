public class MotionCalculator
{
    private readonly IMovable _movable;
    private readonly CalculationChain[] _motion = new CalculationChain[5];

    public DataChange MoveSpeed => new DataChange(_movable.MoveSpeed, _movable.MoveSpeed * _motion[0].Result);
    public DataChange JumpSpeed => new DataChange(_movable.JumpSpeed, _movable.MoveSpeed * _motion[1].Result);
    public DataChange GroundDamping => new DataChange(_movable.GroundDamping, _motion[2].Result);
    public DataChange AirDamping => new DataChange(_movable.AirDamping, _motion[3].Result);
    public DataChange Gravity => new DataChange(_movable.Gravity, _motion[4].Result);

    public MotionCalculator(IMovable movable)
    {
        _movable = movable;
        _motion[0] = new CalculationChain(CalculateUtility.FormulaSum);
        _motion[1] = new CalculationChain(CalculateUtility.FormulaSum);
        _motion[2] = new CalculationChain(CalculateUtility.FormulaSum);
        _motion[3] = new CalculationChain(CalculateUtility.FormulaSum);
        _motion[4] = new CalculationChain(CalculateUtility.FormulaSum);
    }

    public void Add(float value, MotionDataType type) { CalculateUtility.AddChain(_motion[(int) type], value); }

    public bool Remove(float value, MotionDataType type) { return CalculateUtility.RemoveChain(_motion[(int) type], value); }
}