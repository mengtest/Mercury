using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

internal class EntitySystemManager : Singleton<EntitySystemManager>
{
	private readonly Dictionary<Type, IEntitySystem> _systems = new Dictionary<Type, IEntitySystem>();

	public IReadOnlyDictionary<Type, IEntitySystem> Systems => _systems;

	private EntitySystemManager()
	{
		Init();
	}

	private void Init()
	{
		var asm = GetType().Assembly;
		foreach (var type in asm.GetTypes())
		{
			if (type.IsAbstract || type.IsInterface)
			{
				continue;
			}
			if (!typeof(IEntitySystem).IsAssignableFrom(type))
			{
				continue;
			}
			var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (constructors.Length != 1)
			{
				throw new ArgumentException($"系统{type.FullName}不能有多个构造方法");
			}
			var constructor = constructors.SingleOrDefault(c => !c.GetParameters().Any() && c.IsPrivate);
			if (constructor == null)
			{
				throw new ArgumentException($"系统{type.FullName}的构造方法必须是私有且无参的");
			}
			_systems.Add(type, Activator.CreateInstance(type, true) as IEntitySystem);
		}
	}
}
