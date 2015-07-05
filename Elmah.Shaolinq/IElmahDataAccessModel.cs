using Shaolinq;

namespace Elmah.Shaolinq
{
	public interface IElmahDataAccessModel
	{
		DataAccessObjects<DbElmahError> ElmahErrors { get; }
	}
}