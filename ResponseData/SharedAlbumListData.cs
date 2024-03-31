using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.ResponseData
{
	public class SharedAlbumListData
	{
		public List<Album> sharedAlbums { get; set; }
		public string nextPageToken { get; set; }
	}
}
