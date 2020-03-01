using System;
using System.Collections.Generic;
using System.Linq;

namespace Mercury
{
    /// <summary>
    /// 存放所有注册项依赖关系信息
    /// </summary>
    public static class EntryDependInfos
    {
        /// <summary>
        /// 注册项依赖关系信息的注册表
        /// </summary>
        public static readonly EntryDependRegistry Registry = new EntryDependRegistry();

        public static readonly EntryDependInfo Raceter = new EntryDependInfo(Const.Raceter,
            new EntryLocation("entity", Const.Raceter),
            new EntryLocation("asset", Const.Raceter),
            new EntryLocation("asset", Const.RaceterMoonAtkAtked),
            new EntryLocation("asset", Const.RaceterMoonAtk1),
            new EntryLocation("asset", Const.RaceterMoonAtk2),
            new EntryLocation("asset", Const.RaceterMoonAtk3));

        public static readonly EntryDependInfo Scarecrow = new EntryDependInfo(Const.Scarecrow,
            new EntryLocation("entity", Const.Scarecrow),
            new EntryLocation("asset", Const.Scarecrow));

        public static void Init(RegisterManager manager)
        {
            manager.AddRegistry(Registry);
            Registry.Register(Raceter);
            Registry.Register(Scarecrow);
        }
    }

    /// <summary>
    /// 注册项依赖关系信息的注册表
    /// </summary>
    public class EntryDependRegistry : RegistryImpl<EntryDependInfo>
    {
        /// <summary>
        /// key:注册项
        /// value:注册项依赖关系信息
        /// </summary>
        private readonly Dictionary<EntryLocation, EntryDependInfo> _dependencies;

        public EntryDependRegistry() : base("depend") { _dependencies = new Dictionary<EntryLocation, EntryDependInfo>(); }

        /// <summary>
        /// 获取注册项依赖关系信息
        /// </summary>
        /// <param name="id">注册项id</param>
        /// <returns>如果不存在注册项则返回null</returns>
        public EntryDependInfo GetEntryDependInfo(EntryLocation id)
        {
            _dependencies.TryGetValue(id, out var result);
            return result;
        }

        /// <summary>
        /// 筛选注册项依赖
        /// </summary>
        /// <param name="id">注册项id</param>
        /// <param name="selectKey">所需的依赖性类型</param>
        /// <returns>所需的依赖性类型迭代器</returns>
        /// <exception cref="ArgumentException">注册项id不存在</exception>
        public IEnumerable<EntryLocation> SelectEntryDependInfo(EntryLocation id, string selectKey)
        {
            var dependInfo = GetEntryDependInfo(id);
            if (dependInfo == null)
            {
                throw new ArgumentException($"无法找到注册项:{id}");
            }

            return dependInfo.Values.Where(depended => depended.entryType == selectKey);
        }

        public override void Register(EntryDependInfo entry)
        {
            base.Register(entry);

            if (_dependencies.ContainsKey(entry.Key))
            {
                throw new InvalidOperationException($"已注册依赖关系{entry.Key}");
            }

            _dependencies.Add(entry.Key, entry);
        }
    }
}