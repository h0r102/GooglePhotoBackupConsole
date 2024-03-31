using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.Query
{
	class MediaItemListQuery : AbstractQuery<MediaItemListQuery>
	{
		public string albumId { get; set; }
		// レスポンスで返されるアルバムの最大数。返されるアルバムの数が、指定した数より少ない可能性があります。デフォルトの pageSize は 20、最大値は 50 です。
		public int pageSize { get; set; }
		// 結果の次のページを取得するための連続トークン。これをリクエストに追加すると、pageToken の後の行が返されます。pageToken は、listAlbums リクエストに対するレスポンスの nextPageToken パラメータで返される値である必要があります。
		public string pageToken { get; set; }
		//public Filters filters { get; set; }
		public string orderBy { get; set; }
	}
}
