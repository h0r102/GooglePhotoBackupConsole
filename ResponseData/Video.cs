using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.ResponseData
{
	public enum VideoProcessingStatus
	{
		UNSPECIFIED,
		PROCESSING,
		READY,
		FAILED,
	}
	public class Video
	{
		public string cameraMake { get; set; }
		public string cameraModel { get; set; }
		public double fps { get; set; }
		public VideoProcessingStatus status { get; set; }
	}
}
