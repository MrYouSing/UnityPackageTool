
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace UnityPackageTool.Postprocessors {
	public class AssetBundlePostprocessor
		:Postprocessor
	{
		public class Entry {
			public string assetBundleName;
			public string assetBundleVariant;

			public List<Regex> paths=new List<Regex>();

			public virtual bool Contains(string path) {
				for(int i=0,imax=paths?.Count??0;i<imax;++i) {
					if(paths[i].IsMatch(path)) {
						return true;
					}
				}
				return false;
			}

			public virtual void Add(string path) {
				paths.Add(new Regex(path));
			}
		}

		#region Fields

		public List<Entry> entries=new List<Entry>();

		#endregion Fields

		#region Methods

		public AssetBundlePostprocessor(Importer importer):base(importer) {
		}

		public virtual Entry FindEntry(string path) {
			Entry e;
			for(int i=0,imax=entries?.Count??0;i<imax;++i) {
				e=entries[i];
				if(e.Contains(path)) {
					return e;
				}
			}
			return null;
		}

		public override void OnPostImport(string path) {
			Entry e=FindEntry(path);
			if(e!=null) {
				path+=".meta";
				if(File.Exists(path)) {
					bool b=false;
					//System.DateTime dt=File.GetLastWriteTime(path);
					string text=File.ReadAllText(path);
					//
					Regex regex;
					if(e.assetBundleName!=null) {
						b=true;
						//
						regex=new Regex("  assetBundleName: .*");
						text=regex.Replace(text,"  assetBundleName: "+e.assetBundleName.ToLower());
					}
					if(e.assetBundleVariant!=null) {
						b=true;
						//
						regex=new Regex("  assetBundleVariant: .*");
						text=regex.Replace(text,"  assetBundleVariant: "+e.assetBundleVariant.ToLower());
					}
					//
					if(b) {
						File.WriteAllText(path,text);
						//File.SetLastWriteTime(path,dt);
					}
				}
			}
		}

		#endregion Methods
	}
}
