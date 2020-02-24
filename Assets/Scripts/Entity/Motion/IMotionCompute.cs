namespace Mercury
{
    public interface IMotionCompute
    {
        /// <summary>
        /// 移动速度，单位（格/秒）
        /// </summary>
        DataChange<float> MoveSpeed { get; }

        /// <summary>
        /// 跳跃高度，单位（格/秒）
        /// </summary>
        DataChange<float> JumpSpeed { get; }

        /// <summary>
        /// 地面阻力
        /// </summary>
        DataChange<float> GroundDamping { get; }

        /// <summary>
        /// 空气阻力
        /// </summary>
        DataChange<float> AirDamping { get; }

        /// <summary>
        /// 重力
        /// </summary>
        DataChange<float> Gravity { get; }

        /// <summary>
        /// 设置运动数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">类型</param>
        void SetMotionData(float data, MotionDataType type);

        /// <summary>
        /// 移除运动数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">类型</param>
        /// <returns>移除失败返回false</returns>
        bool RemoveMotionData(float data, MotionDataType type);
    }

    /// <summary>
    /// 标准运动计算实现
    /// </summary>
    public class MotionComputeImpl : IMotionCompute
    {
        private readonly MotionData _motionData;
        private readonly ChainAdd[] _datas;

        public MotionComputeImpl(MotionData motionData)
        {
            _datas = new ChainAdd[5];
            _datas[0] = new ChainAdd();
            _datas[1] = new ChainAdd();
            _datas[2] = new ChainAdd();
            _datas[3] = new ChainAdd();
            _datas[4] = new ChainAdd();
            _motionData = motionData;
        }

        public DataChange<float> MoveSpeed => new DataChange<float>(_motionData.moveSpeed, _datas[0].Cache);
        public DataChange<float> JumpSpeed => new DataChange<float>(_motionData.jumpSpeed, _datas[1].Cache);
        public DataChange<float> GroundDamping => new DataChange<float>(_motionData.groundDamping, _datas[2].Cache);
        public DataChange<float> AirDamping => new DataChange<float>(_motionData.airDamping, _datas[3].Cache);
        public DataChange<float> Gravity => new DataChange<float>(_motionData.gravity, _datas[4].Cache);
        
        public void SetMotionData(float data, MotionDataType type) { _datas[(int) type].AddData(data); }

        public bool RemoveMotionData(float data, MotionDataType type) { return _datas[(int) type].RemoveData(data); }
    }
}