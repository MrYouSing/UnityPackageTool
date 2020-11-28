using System.Collections.Generic;
using System.IO;
using static UnityPackageTool.IOExtension;

namespace UnityPackageTool {
	public class Importer {
		#region Fields

		public string destination;
		public System.Func<string,bool> filter=null;
		public System.Action<string> onPostImport=null;

		#endregion Fields

		#region Methods

		public virtual bool CanImport(Package.Entry e) {
			if(e==null||!e.enabled) {return false;}
			if(filter!=null&&!filter(e.path)) {return false;}
			return true;
		}

		public virtual string GetPath(string path) {
			return Path.Combine(destination,path);
		}

		public virtual void Import(Package package) {
			Package.Entry e;
			foreach(var it in package.entries) {
				e=it.Value;
				if(CanImport(e)) {
					Import(e);
				}
			}
		}

		public virtual void Import(Package.Entry e) {
			//
			string fn=GetPath(e.path);
			string dn=Path.GetDirectoryName(fn);
			if(!Directory.Exists(dn)) {
				Directory.CreateDirectory(dn);
			}
			//
			if(e.meta!=null) {
				File_WriteAllBytes(fn+".meta",e.meta,e.metaTime);
			}
			if(e.isFile) {
				File_WriteAllBytes(fn,e.asset,e.assetTime);
			}
			e.Clear();
			//
			OnPostImport(fn);
		}

		public virtual void OnPostImport(string path) {
			//
			path=path.Replace('\\','/');
			//
			if(onPostImport!=null) {
				onPostImport(path);
			}
			System.Console.WriteLine("Import:"+path);
		}

		public virtual void TryImport(Package.Entry e) {
			if(CanImport(e)) {
				Import(e);
			}else {
				e?.Clear();
			}
		}

		public virtual void Import(string src,string dst,ref Package package) {
			if(!string.IsNullOrEmpty(dst)) {
				destination=dst;
			}
			if(package==null) {package=new Package();}
			//
			package.onEntryCompleted+=TryImport;
			package.Init(src);
			package.onEntryCompleted-=TryImport;
			// TODO: Directories????
			Package.Entry e;
			foreach(var it in package.entries) {
				e=it.Value;
				if(CanImport(e)) {
				if(e.meta!=null) {
					Import(e);
				}}
			}
		}

		public virtual void Import(string src,string dst) {
			Package package=null;
			Import(src,dst,ref package);
		}

		public virtual void Import(string path) {
			if(!Path.GetExtension(path).Equals(".meta",System.StringComparison.OrdinalIgnoreCase)) {
				OnPostImport(path);
			}
			//
			if(Directory.Exists(path)) {
				foreach(string fn in Directory.GetFiles(path)) {
					if(!Path.GetExtension(fn).Equals(".meta",System.StringComparison.OrdinalIgnoreCase)) {
						OnPostImport(fn);
					}
				}
				foreach(string dn in Directory.GetDirectories(path)) {
					Import(dn);
				}
			}
		}

		#endregion Methods
	}
}
