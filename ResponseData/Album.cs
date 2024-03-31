using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.ResponseData
{
	public class Album
	{
		public string id { get; set; }
		public string title { get; set; }
		public string productUrl { get; set; }
		public bool isWriteable { get; set; }
		public ShareInfo shareInfo { get; set; }
		public string mediaItemsCount { get; set; }
		public string coverPhotoBaseUrl { get; set; }
		public string coverPhotoMediaItemId { get; set; }
	}
}
