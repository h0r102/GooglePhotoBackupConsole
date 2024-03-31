using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.ResponseData
{
	class FetchTokenData
	{
		public string access_token { get; set; } // 必須
		public string token_type { get; set; } // 必須

		//"expires_in":{有効秒数}, // 任意
		public string refresh_token { get; set; } // 任意
		//"scope":"{スコープ群}"                    // 要求したスコープ群と差異があれば必須
	}
}
