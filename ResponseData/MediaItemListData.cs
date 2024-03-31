using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.ResponseData
{
	public class MediaItemListData
	{
		public List<MediaItem> mediaItems { get; set; }
		public string nextPageToken { get; set; }
	}
}
