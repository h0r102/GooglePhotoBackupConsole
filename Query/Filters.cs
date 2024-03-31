using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.Query
{
	class Filters: AbstractQuery<Filters>
	{
		public DateFilter dateFilter { get; set; }
		public ContentFilter contentFilter { get; set; }
		public MediaTypeFilter mediaTypeFilter { get; set; }
		public FeatureFilter featureFilter { get; set; }
		public bool includeArchivedMedia { get; set; }
		public bool excludeNonAppCreatedData { get; set; }
	}
}
