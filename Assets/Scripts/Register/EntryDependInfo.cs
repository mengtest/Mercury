using System.Text;

namespace Mercury
{
    /// <summary>
    /// 注册项之间依赖关系信息
    /// </summary>
    public class EntryDependInfo : RegistryEntryImpl<EntryDependInfo>
    {
        /// <summary>
        /// 注册项
        /// </summary>
        public EntryLocation Key { get; }

        /// <summary>
        /// 依赖项
        /// </summary>
        public EntryLocation[] Values { get; }
        
        /// <param name="id">依赖关系信息的id</param>
        /// <param name="key">注册项</param>
        /// <param name="values">依赖项</param>
        public EntryDependInfo(AssetLocation id, EntryLocation key, params EntryLocation[] values)
        {
            SetRegisterName(id);
            Key = key;
            Values = values;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Key).Append('#');
            foreach (var v in Values)
            {
                builder.Append(v).Append('|');
            }

            return builder.ToString();
        }
    }
}