using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GooglePhotoBackupConsole.ResponseData;

namespace GooglePhotoBackupConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			Config config = LoadConfig();

			GooglePhotoSession session = CreateSession();
			if (session == null) return;

			FetchTokenData fetchTokenData = LoadToken();
			if (fetchTokenData == null)
			{
				if (!Authenticate(session)) return;

				fetchTokenData = FetchToken(session);
				if (fetchTokenData == null) return;
			}
			else
			{
				Console.WriteLine("トークンを読み込みました。");
				Console.WriteLine("access_token={0}", fetchTokenData.access_token);
				Console.WriteLine("refresh_token={0}", fetchTokenData.refresh_token);
				session.SetFetchToken(fetchTokenData);

				Console.WriteLine("トークンを更新します。");
				fetchTokenData = RefreshToken(session);
				if (fetchTokenData == null) return;
			}

			List<Album> albumList = GetAlbumList(session);
			if (albumList.Count == 0)
			{
				Console.WriteLine("アルバム情報の取得に失敗しました。");
				Console.ReadLine();
			}
			if (SaveAlbums(config, albumList))
			{
				Console.WriteLine("アルバム情報を保存しました。");
			}
			else
			{
				Console.WriteLine("アルバム情報の保存に失敗しました。");
			}

			int albumCount = albumList.Count;
			Console.WriteLine("メディアアイテムを取得します。");
			for (int i = 0; i < albumCount; i++)
			{
				Album album = albumList[i];
				List<MediaItem> mediaItemList = GetMediaItemList(session, album);
				Console.WriteLine("[{0,4}/{1,4}] {2}のメディアアイテムを取得しました。メディアアイテム数={3}", i, albumCount, album.title, mediaItemList.Count);
				if (SaveMediaItems(config, album, mediaItemList))
				{
					Console.WriteLine("メディアアイテム情報を保存しました。({0})", album.title);
					int mediaItemCount = mediaItemList.Count;
					for (int j = 0; j < mediaItemCount; j++)
					{
						MediaItem mediaItem = mediaItemList[j];
						if (!MediaDataExists(config, album, mediaItem))
						{
							byte[] data = DownloadMediaData(session, mediaItem);
							if (data != null)
							{
								SaveMediaData(config, album, mediaItem, data);
							}
						}
					}
				}
				else
				{
					Console.WriteLine("メディアアイテム情報の保存に失敗しました。({0})", album.title);
				}
			}

			Console.ReadLine();
		}

		private static Config LoadConfig()
		{
			Config config;
			if (File.Exists("./config.json"))
			{
				using (StreamReader reader = new StreamReader("./config.json"))
				{
					var json = reader.ReadToEnd();
					config = JsonConvert.DeserializeObject<Config>(json);
				}
			}
			else
			{
				config = new Config();
				config.outputPath = "C:/GooglePhotoBackup";
			}
			
			// 保存先フォルダの生成
			if (!File.Exists(config.outputPath))
			{
				Directory.CreateDirectory(config.outputPath);
			}
			return config;
		}

		private static GooglePhotoSession CreateSession()
		{
			CredentialData credentialData;
			using (StreamReader reader = new StreamReader("./credential.json"))
			{
				var json = reader.ReadToEnd();
				credentialData = JsonConvert.DeserializeObject<CredentialData>(json);
			}

			credentialData.redirect_uris[0] = "http://localhost:" + CodeRedirectServer.Port.ToString();
			GooglePhotoSession session = new GooglePhotoSession(credentialData);
			return session;
		}

		private static FetchTokenData LoadToken()
		{
			if (File.Exists("./token.json"))
			{
				using (StreamReader reader = new StreamReader("./token.json"))
				{
					var json = reader.ReadToEnd();
					var data = JsonConvert.DeserializeObject<FetchTokenData>(json);
					if (data.access_token == null || data.refresh_token == null)
					{
						Console.WriteLine("トークンが無効のため、破棄します。");
						return null;
					}
					return data;
				}
			}
			return null;
		}

		private static bool Authenticate(GooglePhotoSession session)
		{
			// 認証コード取得
			CodeRedirectServer server = new CodeRedirectServer();
			server.OnCodeReceived += (code) =>
			{
				Console.WriteLine("認証コード取得={0}", code);
				session.SetAuthorizationCode(code);
				Console.Write("次に進む(Enter):");
			};
			server.Start();

			Uri uri = session.GetAuthenticateUri();
			string str = uri.ToString();
			// ブラウザを起動するプロセスを作成
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "chrome.exe", // ここには使用するブラウザの実行ファイル名を指定
				Arguments = uri.ToString()
			};

			// プロセスを起動
			Process process = Process.Start(startInfo);

			Console.WriteLine("認証コード取得のため、ブラウザで認証処理を行ってください");
			Console.Write("途中終了(Enter):");
			Console.ReadLine();

			server.Close();

			if (session.GetAuthorizationCode() == "")
			{
				return false;
			}
			return true;
		}

		private static FetchTokenData FetchToken(GooglePhotoSession session)
		{
			// トークン取得
			Task<FetchTokenData> fetchTokenTask = session.FetchToken();
			fetchTokenTask.Wait();

			FetchTokenData fetchTokenData = fetchTokenTask.Result;
			Console.WriteLine("トークンを取得しました。");
			Console.WriteLine("access_token={0}", fetchTokenData.access_token);
			Console.WriteLine("refresh_token={0}", fetchTokenData.refresh_token);

			using (var sw = new StreamWriter("./token.json"))
			{
				string json = JsonConvert.SerializeObject(fetchTokenData);
				sw.Write(json);
			}
			return fetchTokenData;
		}

		private static FetchTokenData RefreshToken(GooglePhotoSession session)
		{
			// トークン取得
			Task<FetchTokenData> refreshTokenTask = session.RefreshToken();
			refreshTokenTask.Wait();

			FetchTokenData fetchTokenData = refreshTokenTask.Result;
			Console.WriteLine("トークンを取得しました。");
			Console.WriteLine("access_token={0}", fetchTokenData.access_token);
			Console.WriteLine("refresh_token={0}", fetchTokenData.refresh_token);

			using (var sw = new StreamWriter("./token.json"))
			{
				string json = JsonConvert.SerializeObject(fetchTokenData);
				sw.Write(json);
			}
			return fetchTokenData;
		}

		private static List<Album> GetAlbumList(GooglePhotoSession session)
		{
			// トークン取得
			Task<List<Album>> getAlbumTask = session.GetAlbumList(true);
			getAlbumTask.Wait();

			List<Album> albumList = getAlbumTask.Result;
			Console.WriteLine("アルバム情報を取得しました。");
			foreach (var album in albumList)
			{
				Console.WriteLine("{0}", album.title);
			}
			return albumList;
		}

		private static List<MediaItem> GetMediaItemList(GooglePhotoSession session, Album album)
		{
			// トークン取得
			Task<List<MediaItem>> getMediaItemTask = session.GetMediaItemList(album);
			getMediaItemTask.Wait();

			List<MediaItem> mediaItemList = getMediaItemTask.Result;
			//Console.WriteLine("アルバム情報を取得しました。");
			//foreach (var mediaItem in mediaItemList)
			//{
			//	Console.WriteLine("{0}:{1}", mediaItem.id, mediaItem.filename);
			//}
			return mediaItemList;
		}

		private static bool SaveAlbums(Config config, List<Album> albums)
		{
			string dirpath = config.outputPath;
			List<Album> oldAlbums = BackupFileIO.ReadAlbumsJson(dirpath);
			foreach (var album in albums)
			{
				foreach (var old in oldAlbums)
				{
					if (album.id == old.id)
					{
						if (album.title != old.title)
						{
							BackupFileIO.RenameAlbumDirectory(dirpath, old.title, album.title);
						}
						break;
					}
				}
			}
			return BackupFileIO.WriteAlbumsJson(dirpath, albums);
		}

		private static bool SaveMediaItems(Config config, Album album, List<MediaItem> mediaItems)
		{
			string dirpath = config.outputPath;
			return BackupFileIO.WriteMediaItemsJson(dirpath, album.title, mediaItems);
		}

		private static bool MediaDataExists(Config config, Album album, MediaItem mediaItem)
		{
			string dirpath = config.outputPath;
			string albumTitle = album.title;
			string filename = mediaItem.filename;
			return BackupFileIO.MediaItemFileExists(dirpath, albumTitle, filename);
		}

		private static byte[] DownloadMediaData(GooglePhotoSession session, MediaItem mediaItem)
		{
			Task<byte[]> downloadTask = session.DownloadMediaData(mediaItem);
			downloadTask.Wait();
			return downloadTask.Result;
		}

		private static bool SaveMediaData(Config config, Album album, MediaItem mediaItem, byte[] mediaData)
		{
			string dirpath = config.outputPath;
			string albumTitle = album.title;
			string filename = mediaItem.filename;
			return BackupFileIO.WriteMediaItemFile(dirpath, albumTitle, filename, mediaData);
		}
	}
}
