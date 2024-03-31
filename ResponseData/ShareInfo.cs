using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.ResponseData
{
	public class ShareInfo
	{
		public SharedAlbumOptions sharedAlbumOptions { get; set; }
		public string shareableUrl { get; set; }
		public string shareToken { get; set; }
		public bool isJoined { get; set; }
		public bool isOwned { get; set; }
		public bool isJoinable { get; set; }}
}
