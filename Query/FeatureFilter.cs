using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.Query
{
	enum Feature
	{
		NONE,
		FAVORITES
	}
	class FeatureFilter : AbstractQuery<FeatureFilter>
	{
		public List<Feature> includedFeatures { get; set; }
	}
}
