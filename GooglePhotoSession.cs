using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GooglePhotoBackupConsole.Query;
using GooglePhotoBackupConsole.ResponseData;

namespace GooglePhotoBackupConsole
{
	class GooglePhotoSession
	{
		private readonly string[] scope = new string[] { "https://www.googleapis.com/auth/photoslibrary" };
		private readonly string apiBaseUri = "https://photoslibrary.googleapis.com";
		private static HttpClient client = new HttpClient();
		private CredentialData credential;
		private FetchTokenData fetchTokenData;
		private string authorizationCode = "";

		public GooglePhotoSession(CredentialData data)
		{
			credential = data;
		}

		// -- 認可コード関連 --
		public Uri GetAuthenticateUri()
		{
			AuthenticationQuery query = GetAuthenticationQuery();
			return CreateUri(credential.auth_uri, query.ToString());
		}

		public void SetAuthorizationCode(string code)
		{
			authorizationCode = code;
		}

		public string GetAuthorizationCode()
		{
			return authorizationCode;
		}

		// -- トークン取得関連 --

		public void SetFetchToken(FetchTokenData data)
		{
			fetchTokenData = data;
		}


		public async Task<FetchTokenData> FetchToken()
		{
			FetchTokenQuery query = GetFetchTokenQuery();
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, credential.token_uri);
			request.Content = new System.Net.Http.StringContent(query.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
			HttpResponseMessage response = await client.SendAsync(request);

			//HttpResponseMessage response = await client.PostAsJsonAsync(credential.token_uri, query);
			if (response.IsSuccessStatusCode)
			{
				//レスポンスからJSON文字列を取得
				var jsonString = await response.Content.ReadAsStringAsync();
				//JSON文字列をデシリアライズしてFetchTokenResponseData型のデータに変換
				FetchTokenData data = JsonConvert.DeserializeObject<FetchTokenData>(jsonString);
				fetchTokenData = data;
				return data;
			}
			else
			{
				var jsonString = await response.Content.ReadAsStringAsync();
				Console.WriteLine("トークン取得に失敗しました。");
				Console.WriteLine("StatusCode={0}", response.StatusCode);
				Console.WriteLine("Reason={0}", response.ReasonPhrase);
				Console.WriteLine("RequestContent={0}", query.ToString());
				Console.WriteLine("ResponseContent={0}", jsonString);
				return null;
			}
		}

		public async Task<FetchTokenData> RefreshToken()
		{
			RefreshTokenQuery query = GetRefreshTokenQuery();
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, credential.token_uri);
			request.Content = new System.Net.Http.StringContent(query.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
			HttpResponseMessage response = await client.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				//レスポンスからJSON文字列を取得
				var jsonString = await response.Content.ReadAsStringAsync();
				//JSON文字列をデシリアライズしてFetchTokenResponseData型のデータに変換
				FetchTokenData data = JsonConvert.DeserializeObject<FetchTokenData>(jsonString);
				fetchTokenData.access_token = data.access_token;
				return fetchTokenData;
			}
			else
			{
				var jsonString = await response.Content.ReadAsStringAsync();
				Console.WriteLine("トークン取得に失敗しました。");
				Console.WriteLine("StatusCode={0}", response.StatusCode);
				Console.WriteLine("Reason={0}", response.ReasonPhrase);
				Console.WriteLine("RequestContent={0}", query.ToString());
				Console.WriteLine("ResponseContent={0}", jsonString);
				return null;
			}
		}
		// -- リスト表示 -- 
		public async Task<List<Album>> GetAlbumList(bool isShared = false)
		{
			List<Album> albumList = new List<Album>();

			string requestUri = "";
			if (isShared)
			{
				requestUri = SharedAlbumsUri();
			}
			else
			{
				requestUri = AlbumsUri();
			}

			string nextPageToken = "";
			while (true)
			{
				AlbumListQuery query = GetAlbumListQuery(nextPageToken);
				string requestData = JsonConvert.SerializeObject(query);
				requestUri = string.Format("{0}?{1}", requestUri, query.ToString());
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri);
				request.Headers.Add("Authorization", "Bearer " + fetchTokenData.access_token);
				HttpResponseMessage response = await client.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					//レスポンスからJSON文字列を取得
					var jsonString = await response.Content.ReadAsStringAsync();
					//JSON文字列をデシリアライズしてAlbumListResponseData型のデータに変換
					if (isShared)
					{
						SharedAlbumListData data = JsonConvert.DeserializeObject<SharedAlbumListData>(jsonString);
						albumList.AddRange(data.sharedAlbums);
						nextPageToken = data.nextPageToken;
					}
					else
					{
						AlbumListData data = JsonConvert.DeserializeObject<AlbumListData>(jsonString);
						albumList.AddRange(data.albums);
						nextPageToken = data.nextPageToken;
					}
				}
				else
				{
					var jsonString = await response.Content.ReadAsStringAsync();
					Console.WriteLine("アルバム情報の取得に失敗しました。");
					Console.WriteLine("StatusCode={0}", response.StatusCode);
					Console.WriteLine("Reason={0}", response.ReasonPhrase);
					Console.WriteLine("RequestContent={0}", query.ToString());
					Console.WriteLine("ResponseContent={0}", jsonString);
					break;
				}
			}
			return albumList;
		}

		// -- リスト表示 -- 
		public async Task<List<MediaItem>> GetMediaItemList(Album album)
		{
			List<MediaItem> mediaItemList = new List<MediaItem>();
			string requestUri = MediaItemsUri();
			requestUri = string.Format("{0}:search", requestUri);

			int mediaItemsCount;
			if (!int.TryParse(album.mediaItemsCount, out mediaItemsCount))
			{
				return mediaItemList;
			}
			int curMediaItemsCount = 0;

			string nextPageToken = "";
			while (mediaItemsCount > curMediaItemsCount)
			{
				MediaItemListQuery query = GetMediaItemListQuery(album.id, nextPageToken);
				HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri);
				request.Headers.Add("Authorization", "Bearer " + fetchTokenData.access_token);
				request.Content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(query), Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					//レスポンスからJSON文字列を取得
					var jsonString = await response.Content.ReadAsStringAsync();
					//JSON文字列をデシリアライズしてMediaItemListResponseData型のデータに変換
					MediaItemListData data = JsonConvert.DeserializeObject<MediaItemListData>(jsonString);
					foreach (var mediaItem in data.mediaItems)
					{
						mediaItem.filename = ToSafeFilename(mediaItem.filename);
					}
					mediaItemList.AddRange(data.mediaItems);
					curMediaItemsCount += data.mediaItems.Count;
					nextPageToken = data.nextPageToken;
					Console.WriteLine("[{0,3}%]メディアアイテムを取得しました。", 100 * curMediaItemsCount / mediaItemsCount);
				}
				else
				{
					var jsonString = await response.Content.ReadAsStringAsync();
					Console.WriteLine("メディアアイテム情報の取得に失敗しました。");
					Console.WriteLine("StatusCode={0}", response.StatusCode);
					Console.WriteLine("Reason={0}", response.ReasonPhrase);
					Console.WriteLine("RequestContent={0}", query.ToString());
					Console.WriteLine("ResponseContent={0}", jsonString);
					break;
				}
			}
			return mediaItemList;
		}

		// -- ダウンロード -- 
		public async Task<byte[]> DownloadMediaData(MediaItem mediaItem)
		{
			string requestUri = mediaItem.baseUrl;
			requestUri = string.Format("{0}=d", requestUri);

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri);
			request.Headers.Add("Authorization", "Bearer " + fetchTokenData.access_token);
			HttpResponseMessage response = await client.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
				//レスポンスからJSON文字列を取得
				var bytes = await response.Content.ReadAsByteArrayAsync();
				return bytes;
			}
			else
			{
				Console.WriteLine("メディアデータのダウンロードに失敗しました。");
				Console.WriteLine("StatusCode={0}", response.StatusCode);
				Console.WriteLine("Reason={0}", response.ReasonPhrase);
				return null;
			}
		}

		// -- サブルーチン --
		private Uri CreateUri(string path, string query)
		{
			var uriBuilder = new UriBuilder(path);
			uriBuilder.Query = query;
			return uriBuilder.Uri;
		}

		private AuthenticationQuery GetAuthenticationQuery()
		{
			AuthenticationQuery query = new AuthenticationQuery();
			query.client_id = credential.client_id;
			query.redirect_uri = credential.redirect_uris[0];
			query.response_type = "code";
			query.scope = scope;
			return query;
		}

		private FetchTokenQuery GetFetchTokenQuery()
		{
			FetchTokenQuery query = new FetchTokenQuery();
			query.client_id = credential.client_id;
			query.client_secret = credential.client_secret;
			query.redirect_uri = credential.redirect_uris[0];
			query.code = authorizationCode;
			return query;
		}

		private RefreshTokenQuery GetRefreshTokenQuery()
		{
			RefreshTokenQuery query = new RefreshTokenQuery();
			query.client_id = credential.client_id;
			query.client_secret = credential.client_secret;
			query.refresh_token = fetchTokenData.refresh_token;
			return query;
		}

		private AlbumListQuery GetAlbumListQuery(string pageToken = "")
		{
			AlbumListQuery query = new AlbumListQuery();
			query.pageSize = 50;
			query.pageToken = pageToken;
			query.excludeNonAppCreatedData = false;
			return query;
		}

		private MediaItemListQuery GetMediaItemListQuery(string albumId, string pageToken = "")
		{
			MediaItemListQuery query = new MediaItemListQuery();
			query.albumId = albumId;
			query.pageSize = 50;
			query.pageToken = pageToken;
			return query;
		}

		private string ToSafeFilename(string filename)
		{
			return filename
			.Replace("\\", "")
			.Replace("/", "")
			.Replace("\"", "")
			.Replace("*", "")
			.Replace(":", "")
			.Replace("?", "")
			.Replace("<", "")
			.Replace(">", "")
			.Replace("|", "");
		}

		private string AlbumsUri()
		{
			return apiBaseUri + "/v1/albums";
		}

		private string SharedAlbumsUri()
		{
			return apiBaseUri + "/v1/sharedAlbums";
		}

		private string MediaItemsUri()
		{
			return apiBaseUri + "/v1/mediaItems";
		}
	}
}
