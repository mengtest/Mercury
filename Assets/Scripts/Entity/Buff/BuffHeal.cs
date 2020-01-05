public class BuffHeal : IBuff
{
	private int _triggerCount;
	private static readonly int _triggerFreq = 500;

	public float Duration { get; set; }
	public int Intensity { get; private set; }

	public BuffVariant Variant => BuffVariant.Dot;

	public BuffHeal(float duration, int intensity)
	{
		Duration = duration;
		Intensity = intensity;
		_triggerCount = GetTAllriggerCount(Duration);
	}

	private static int GetTAllriggerCount(float duration)
	{
		return (int)duration / _triggerFreq;
	}

	public bool IsReady()
	{
		var leaveCount = GetTAllriggerCount(Duration);
		if (leaveCount < _triggerCount)
		{
			_triggerCount -= 1;
			return true;
		}
		else
		{
			return false;
		}
	}

	public void Merge(IBuff other)
	{
		if (other.Intensity > Intensity)
		{
			Intensity = other.Intensity;
		}
		Duration += other.Duration;
		_triggerCount = GetTAllriggerCount(Duration);
	}

	public void OnFirstAdd(IBuffable buffable)
	{
	}

	public void OnRemove(IBuffable buffable)
	{
	}

	public void OnTrigger(IBuffable buffable)
	{
		var e = buffable as Entity;
		e.Heal(Intensity * 0.5f);
	}
}
