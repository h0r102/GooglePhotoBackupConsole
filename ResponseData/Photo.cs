using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.ResponseData
{
	public class Photo
	{
		public string cameraMake { get; set; }
		public string cameraModel { get; set; }
		public double focalLength { get; set; }
		public double apertureFNumber { get; set; }
		public int isoEquivalent { get; set; }
		public string exposureTime { get; set; }
	}
}
