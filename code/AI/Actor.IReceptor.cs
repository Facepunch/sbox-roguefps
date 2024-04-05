
namespace RogueFPS;

partial class Actor
{
	public interface IReceptor
	{
		public void OnStimulusReceived( Stimulus stimulusInfo );
	}
}
