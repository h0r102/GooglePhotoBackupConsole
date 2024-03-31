using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.ResponseData
{
	public class AlbumListData
	{
		public List<Album> albums { get; set; }
		public string nextPageToken { get; set; }
	}
}
