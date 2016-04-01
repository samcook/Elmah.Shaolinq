using System;
using System.Collections;
using System.Linq;
using Elmah.Shaolinq.DataModel;
using Platform;
using Shaolinq;

namespace Elmah.Shaolinq
{
	public class ShaolinqErrorLog : ErrorLog
	{
		private const int MaxAppNameLength = 60;

		private readonly IElmahDataAccessModel dataModel;

		public ShaolinqErrorLog(IDictionary config)
		{
			var dataAccessModelTypeName = config.Find("dataAccessModelType", string.Empty);

			if (string.IsNullOrEmpty(dataAccessModelTypeName))
			{
				throw new ApplicationException("DataAccessModelType not specified");
			}

			var modelType = Type.GetType(dataAccessModelTypeName);

			if (modelType == null)
			{
				throw new ApplicationException($"Could not find type {dataAccessModelTypeName}");
			}

			if (!modelType.GetInterfaces().Contains(typeof(IElmahDataAccessModel)))
			{
				throw new ApplicationException("DataAccessModelType must implement IElmahDataAccessModel");
			}

			var dataAccessModelConfigSection = config.Find("dataAccessModelConfigSection", modelType.Name);

			var dataAccessModelConfiguration = ConfigurationBlock<DataAccessModelConfiguration>.Load(dataAccessModelConfigSection);

			this.dataModel = (IElmahDataAccessModel)DataAccessModel.BuildDataAccessModel(
				Type.GetType(dataAccessModelTypeName),
				dataAccessModelConfiguration);

			// Set the application name as this implementation provides per-application isolation over a single store.
			// Use application name of "*" to disable per-application isolation.
			var appName = config.Find("applicationName", string.Empty);

			if (appName.Length > MaxAppNameLength)
			{
				throw new ApplicationException($"Application name is too long. Maximum length allowed is {MaxAppNameLength.ToString("N0")} characters.");
			}

			ApplicationName = appName;
		}

		public override string Name => "Shaolinq Error Log";

		public override string Log(Error error)
		{
			if (error == null)
			{
				throw new ArgumentNullException(nameof(error));
			}

			var errorXml = ErrorXml.EncodeString(error);

			using (var scope = DataAccessScope.CreateReadCommitted())
			{
				var dbElmahError = dataModel.ElmahErrors.Create();

				dbElmahError.Application = ApplicationName;
				dbElmahError.Host = error.HostName;
				dbElmahError.Type = error.Type;
				dbElmahError.Source = error.Source;
				dbElmahError.Message = error.Message;
				dbElmahError.User = error.User;
				dbElmahError.StatusCode = error.StatusCode;
				dbElmahError.TimeUtc = error.Time;
				dbElmahError.AllXml = errorXml;

				scope.Complete();

				return dbElmahError.Id.ToString();
			}
		}

		public override ErrorLogEntry GetError(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException(nameof(id));
			}

			Guid errorId;
			if (!Guid.TryParse(id, out errorId))
			{
				throw new ArgumentException("Could not parse id as guid", nameof(id));
			}

			var dbElmahErrors = dataModel.ElmahErrors.Where(x => x.Id == errorId);

			if (ApplicationName != "*")
			{
				dbElmahErrors = dbElmahErrors.Where(x => x.Application == ApplicationName);
			}

			var dbElmahError = dbElmahErrors.SingleOrDefault();

			if (dbElmahError == null)
			{
				return null;
			}

			var error = ErrorXml.DecodeString(dbElmahError.AllXml);

			return new ErrorLogEntry(this, id, error);
		}

		public override int GetErrors(int pageIndex, int pageSize, IList errorEntryList)
		{
			if (pageIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(pageIndex), pageIndex, null);
			}

			if (pageSize < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, null);
			}

			var dbElmahErrors = dataModel.ElmahErrors.Select(x => x);

			if (ApplicationName != "*")
			{
				dbElmahErrors = dbElmahErrors.Where(x => x.Application == ApplicationName);
			}

			dbElmahErrors = dbElmahErrors
				.OrderByDescending(x => x.TimeUtc)
				.ThenByDescending(x => x.Sequence)
				.Skip(pageIndex * pageSize)
				.Take(pageSize);

			foreach (var dbElmahError in dbElmahErrors.ToList())
			{
				errorEntryList.Add(new ErrorLogEntry(this, dbElmahError.Id.ToString(), ErrorXml.DecodeString(dbElmahError.AllXml)));
			}

			return dataModel.ElmahErrors.Count(x => x.Application == ApplicationName);
		}
	}
}
