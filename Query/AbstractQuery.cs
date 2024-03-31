using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePhotoBackupConsole.Query
{
	abstract class AbstractQuery<T>
	{
		public override string ToString()
		{
			string query = "";
			foreach (var f in typeof(T).GetProperties())
			{
				var n = f.Name;
				var v = f.GetValue(this);
				if (v != null)
				{
					if (query == "")
					{
						query = n + "=" + ValueToString(v);
					}
					else
					{
						query += "&" + n + "=" + ValueToString(v);
					}
				}
			}
			foreach (var f in typeof(T).GetFields())
			{
				var n = f.Name;
				var v = f.GetValue(this);
				if (v != null)
				{
					if (query == "")
					{
						query = n + "=" + ValueToString(v);
					}
					else
					{
						query += "&" + n + "=" + ValueToString(v);
					}
				}
			}
			return query;
		}

		static public string ValueToString(object value)
		{
			Type type = value.GetType();
			if (type.IsArray)
			{
				object[] array = (object[])value;
				return string.Join(",", array);
			}
			else if (typeof(List<>).IsAssignableFrom(type))
			{
				List<object> list = (List<object>)value;
				return string.Join(",", list);
			}
			else
			{
				return value.ToString();
			}
		}
	}
}
