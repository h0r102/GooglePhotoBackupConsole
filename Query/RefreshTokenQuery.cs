using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.Query
{
	class RefreshTokenQuery : AbstractQuery<RefreshTokenQuery>
	{
		public string client_id { get; set; }

		public string client_secret { get; set; }
		public string refresh_token { get; set; }
		public string grant_type = "refresh_token";
	}
}
