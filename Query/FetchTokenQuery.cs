﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.Query
{
	class FetchTokenQuery : AbstractQuery<FetchTokenQuery>
	{
		public string client_id { get; set; }

		public string client_secret { get; set; }
		public string redirect_uri { get; set; }
		public string grant_type = "authorization_code";
		public string code { get; set; }
	}
}
