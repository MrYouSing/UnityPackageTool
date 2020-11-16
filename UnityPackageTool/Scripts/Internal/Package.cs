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

			public virtual bool isFile=>asset!=null;
			public virtual bool isDirectory=>asset==null;
		}

		#endregion Nested Types

		#region Fields

		public Dictionary<string,Entry> entries=new Dictionary<string,Entry>();

		#endregion Fields

		#region Methods

		public Package() {
		}

		public Package(string path) {
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
				byte[] bytes;
				using(MemoryStream ms=new MemoryStream()) {
					ts.ReadNextFile(ms);
					bytes=ms.ToArray();
				}
				switch(Path.GetFileName(te.Name)) {
					case "pathname":
						e.path=Encoding.ASCII.GetString(bytes);
						//
						int n=e.path.IndexOf('\r');
						if(n>=0) {
							e.path=e.path.Substring(0,n);
						}
					break;
					case "asset":
						e.asset=bytes;
					break;
					case "asset.meta":
						e.meta=bytes;
					break;
				}
			}
		}

		#endregion Methods
	}
}