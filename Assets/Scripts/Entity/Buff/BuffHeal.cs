public class BuffHeal : IBuff
{
	public float Duration { get; set; }

	public int Intensity { get; private set; }

	public BuffVariant Variant => BuffVariant.Dot;

	public BuffHeal(float duration, int intensity)
	{
		Duration = duration;
		Intensity = intensity;
	}

	public bool IsReady()
	{
		var time = 120 >> Intensity;
		return time > 0 ? Duration % time == 0 : true;
	}

	public void Merge(IBuff other)
	{
		if (other.Intensity > Intensity)
		{
			Intensity = other.Intensity;
		}
		Duration += other.Duration;
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
		e.Heal(1 << Intensity);
	}
}
