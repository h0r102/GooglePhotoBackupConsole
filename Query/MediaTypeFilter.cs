using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.Query
{
	enum MediaType
	{
		ALL_MEDIA,
		VIDEO,
		PHOTO,
	}
	class MediaTypeFilter : AbstractQuery<MediaTypeFilter>
	{
		public List<MediaType> mediaTypes { get; set; }
	}
}
