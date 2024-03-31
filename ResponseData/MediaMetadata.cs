using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.ResponseData
{
	public class MediaMetadata
	{
		public string creationTime { get; set; }
		public string width { get; set; }
		public string height { get; set; }
		public Photo photo { get; set; }
		public Video video { get; set; }
	}
}
