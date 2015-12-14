using System;
using Platform.Validation;
using Shaolinq;

namespace Elmah.Shaolinq.DataModel
{
	/*
	CREATE TABLE "ElmahError"
	(
	  "ElmahErrorId" UUID NOT NULL PRIMARY KEY,
	  "Application" VARCHAR(60) NOT NULL,
	  "Host" VARCHAR(50) NOT NULL,
	  "Type" VARCHAR(100) NOT NULL,
	  "Source" VARCHAR(60) NOT NULL,
	  "Message" VARCHAR(500) NOT NULL,
	  "User" VARCHAR(50) NOT NULL,
	  "StatusCode" INTEGER NOT NULL,
	  "TimeUtc" TIMESTAMP NOT NULL,
	  "Sequence" SERIAL NOT NULL,
	  "AllXml" TEXT NOT NULL
	);

	CREATE INDEX "ElmahError_Application_TimeUtc_Sequence" ON "ElmahError"("Application", "TimeUtc", "Sequence");
	*/
	[DataAccessObject(Name = "ElmahError")]
	public abstract class DbElmahError
		: DataAccessObject<Guid>
	{
		[ValueRequired]
		[PersistedMember]
		[SizeConstraint(MaximumLength = 60)]
		[Index(IndexName = "ElmahError_Application_TimeUtc_Sequence", CompositeOrder = 1)]
		public abstract string Application { get; set; }

		[ValueRequired]
		[PersistedMember]
		[SizeConstraint(MaximumLength = 50)]
		public abstract string Host { get; set; }

		[ValueRequired]
		[PersistedMember]
		[SizeConstraint(MaximumLength = 100)]
		public abstract string Type { get; set; }

		[ValueRequired]
		[PersistedMember]
		[SizeConstraint(MaximumLength = 60)]
		public abstract string Source { get; set; }

		[ValueRequired]
		[PersistedMember]
		[SizeConstraint(MaximumLength = 500)]
		public abstract string Message { get; set; }
		
		[ValueRequired]
		[PersistedMember]
		[SizeConstraint(MaximumLength = 50)]
		public abstract string User { get; set; }
		
		[ValueRequired]
		[PersistedMember]
		public abstract int StatusCode { get; set; }
		
		[ValueRequired]
		[PersistedMember]
		[Index(IndexName = "ElmahError_Application_TimeUtc_Sequence", CompositeOrder = 2)]
		public abstract DateTime TimeUtc { get; set; }

		[ValueRequired]
		[PersistedMember]
		[Index(IndexName = "ElmahError_Application_TimeUtc_Sequence", CompositeOrder = 2)]
		[AutoIncrement]
		public abstract int Sequence { get; set; }

		[ValueRequired]
		[PersistedMember]
		[SizeConstraint(SizeFlexibility = SizeFlexibility.LargeVariable)]
		public abstract string AllXml { get; set; }
	}
}