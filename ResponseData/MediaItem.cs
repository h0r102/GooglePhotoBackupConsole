using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.ResponseData
{
	public class MediaItem
	{
		public string id { get; set; }
		public string description { get; set; }
		public string productUrl { get; set; }
		public string baseUrl { get; set; }
		public string mimeType { get; set; }
		public MediaMetadata mediaMetadata { get; set; }
		public ContributorInfo contributorInfo { get; set; }
		public string filename { get; set; }
	}
}
