using Shaolinq;

namespace Elmah.Shaolinq.DataModel
{
	[DataAccessModel]
	public abstract class ElmahDataAccessModel
		: DataAccessModel, IElmahDataAccessModel
	{
		[DataAccessObjects]
		public abstract DataAccessObjects<DbElmahError> ElmahErrors { get; }
	}
}