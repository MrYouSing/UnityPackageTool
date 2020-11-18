using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace UnityPackageTool {
	public partial class Package {
		#region Nested Types

		public class Entry {
			// Essential
			public string path;
			public byte[] asset;
			public byte[] meta;
			// 
			public bool enabled;
			public string guid;

			public System.DateTime assetTime;
			public System.DateTime metaTime;

			public virtual bool isFile=>asset!=null;
			public virtual bool isDirectory=>asset==null;

			public virtual void Clear() {
				asset=null;
				meta=null;
			}
		}

		#endregion Nested Types

		#region Fields

		public bool onlyList;
		public int cacheSize;
		public Dictionary<string,Entry> entries=new Dictionary<string,Entry>();
		public System.Action<Entry> onEntryCompleted=null;

		#endregion Fields

		#region Methods

		public Package() {
		}

		public virtual void Init(string path) {
			string ext=Path.GetExtension(path);
			switch(ext.ToLower()) {
				case ".unitypackage":
				case ".zip":
					InitWithUnityPackage(File.OpenRead(path));
				break;
			}
		}

		public virtual Entry GetEntry(string guid) {
			if(string.IsNullOrEmpty(guid)) {return null;}
			if(!entries.TryGetValue(guid,out var e)||e==null) {
				e=new Entry();
				e.enabled=true;
				e.guid=guid;
				entries[guid]=e;
			}
			return e;
		}

		public virtual void InitWithUnityPackage(Stream stream) {
			using(GZipInputStream zs=new GZipInputStream(stream)) {
			using(TarInputStream ts=new TarInputStream(zs)) {
				TarEntry te;
				while((te=ts.GetNextEntry())!=null) {
					if(te.IsDirectory){
						continue;
					}
					AddTarEntry(ts,te);
				}
			}}
		}

		public virtual void AddTarEntry(TarInputStream ts,TarEntry te) {
			Entry e=GetEntry(Path.GetDirectoryName(te.Name));
			if(e!=null) {
				//
				int usage=-1;
				switch(Path.GetFileName(te.Name)) {
					case "pathname":
						usage=0;
					break;
					case "asset":
						if(!onlyList) {
							usage=1;
						}
					break;
					case "asset.meta":
						if(!onlyList) {
							usage=2;
						}
					break;
				}
				if(usage<0) {return;}
				//
				byte[] bytes;
				using(MemoryStream ms=(cacheSize>0)?new MemoryStream(cacheSize):new MemoryStream()) {
					ts.ReadNextFile(ms);
					bytes=ms.ToArray();
				}
				switch(usage) {
					case 0:
						e.path=Encoding.ASCII.GetString(bytes);
						//
						int n=e.path.IndexOf('\r');
						if(n>=0) {
							e.path=e.path.Substring(0,n);
						}
					break;
					case 1:
						e.asset=bytes;
						e.assetTime=te.ModTime;
					break;
					case 2:
						e.meta=bytes;
						e.metaTime=te.ModTime;
					break;
				}
				// TODO: Directories????
				if(!string.IsNullOrEmpty(e.path)
					&&e.asset!=null&&e.meta!=null
				) {
					if(onEntryCompleted!=null) {
						onEntryCompleted(e);
					}
				}
			}
		}

		public virtual List<string> ToList(List<string> list=null) {
			int n=entries.Count;
			if(list==null) {list=new List<string>(n);}
			int i=list.Count;n=0;
			//
			Entry e;
			foreach(var it in entries) {
				e=it.Value;
				if(e!=null&&!string.IsNullOrEmpty(e.path)) {
					list.Add(e.path);
					++n;
				}
			}
			//
			list.Sort(string.CompareOrdinal);//i,n????
			return list;
		}

		#endregion Methods
	}
}