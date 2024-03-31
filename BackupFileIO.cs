using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GooglePhotoBackupConsole.ResponseData;

namespace GooglePhotoBackupConsole
{
	class BackupFileIO
	{
		static private string albumsJsonFilename = "albums.json";
		static private string mediaItemsJsonFilename = "mediaItems.json";

		static public bool RenameAlbumDirectory(string dirpath, string oldTitle, string newTitle)
		{
			string oldDirpath = Path.Combine(dirpath, oldTitle);
			string newDirpath = Path.Combine(dirpath, newTitle);
			if (Directory.Exists(oldDirpath))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(oldDirpath);
				directoryInfo.MoveTo(newDirpath);
				return true;
			}
			return false;
		}
		static public bool WriteAlbumsJson(string dirpath, List<Album> albums)
		{
			try
			{
				Directory.CreateDirectory(dirpath);
				string filepath = GetAlbumsJsonFilepath(dirpath);
				using (StreamWriter sw = new StreamWriter(filepath))
				{
					// serialize JSON directly from a file
					JsonSerializer serializer = new JsonSerializer();
					serializer.Serialize(sw, albums);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("アルバム情報の保存に失敗しました。");
				Console.WriteLine(e);
				return false;
			}
			return true;
		}
		static public List<Album> ReadAlbumsJson(string dirpath)
		{
			string filepath = GetAlbumsJsonFilepath(dirpath);
			if (File.Exists(filepath))
			{
				using (StreamReader sr = new StreamReader(filepath))
				{
					// deserialize JSON directly from a file
					JsonSerializer serializer = new JsonSerializer();
					return (List<Album>)serializer.Deserialize(sr, typeof(List<Album>));
				}
			}
			return null;
		}
		static public bool WriteMediaItemsJson(string dirpath, string albumTitle, List<MediaItem> mediaItems)
		{
			try
			{
				string filepath = GetMediaItemsJsonFilepath(dirpath, albumTitle);
				Directory.CreateDirectory(Path.GetDirectoryName(filepath));
				using (StreamWriter sw = new StreamWriter(filepath))
				{
					// serialize JSON directly from a file
					JsonSerializer serializer = new JsonSerializer();
					serializer.Serialize(sw, mediaItems);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("メディアアイテム情報の保存に失敗しました。({0})", albumTitle);
				Console.WriteLine(e);
				return false;
			}
			return true;
		}
		static public List<MediaItem> ReadMediaItemsJson(string dirpath, string albumTitle)
		{
			string filepath = GetMediaItemsJsonFilepath(dirpath, albumTitle);
			if (File.Exists(filepath))
			{
				using (StreamReader sr = new StreamReader(filepath))
				{
					// deserialize JSON directly from a file
					JsonSerializer serializer = new JsonSerializer();
					return (List<MediaItem>)serializer.Deserialize(sr, typeof(List<MediaItem>));
				}
			}
			return null;
		}

		static public bool MediaItemFileExists(string dirpath, string albumTitle, string filename)
		{
			string filepath = GetMediaItemFilepath(dirpath, albumTitle, filename);
			return File.Exists(filepath);
		}
		static public bool WriteMediaItemFile(string dirpath, string albumTitle, string filename, byte[] mediaData)
		{
			string filepath = GetMediaItemFilepath(dirpath, albumTitle, filename);
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(filepath));
				File.WriteAllBytes(filepath, mediaData);
			}
			catch (Exception e)
			{
				Console.WriteLine("メディアアイテム情報の保存に失敗しました。({0})", filepath);
				Console.WriteLine(e);
				return false;
			}
			return true;
		}
		static private string GetAlbumsJsonFilepath(string dirpath)
		{
			return Path.Combine(dirpath, albumsJsonFilename);
		}
		static private string GetMediaItemsJsonFilepath(string dirpath, string albumTitle)
		{
			return Path.Combine(dirpath, albumTitle, mediaItemsJsonFilename);
		}
		static private string GetMediaItemFilepath(string dirpath, string albumTitle, string filename)
		{
			return Path.Combine(dirpath, albumTitle, filename);
		}
	}
}
