using Shaolinq;

namespace Elmah.Shaolinq.DataModel
{
	public interface IElmahDataAccessModel
	{
		DataAccessObjects<DbElmahError> ElmahErrors { get; }
	}
}