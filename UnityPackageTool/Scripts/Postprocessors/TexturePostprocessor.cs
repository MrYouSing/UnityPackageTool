using System.IO;
using FreeImageAPI;
using static UnityPackageTool.IOExtension;

namespace UnityPackageTool.Postprocessors {
	public class TexturePostprocessor
		:Postprocessor
	{
		#region Fields

		public int maxSize=-1;
		public System.Func<string,bool> filter=null;

		#endregion Fields

		#region Methods

		public TexturePostprocessor(Importer importer):base(importer) {
		}

		public virtual bool CanConvert(ref FREE_IMAGE_FORMAT fif) {
			//
			switch(fif) {
				case FREE_IMAGE_FORMAT.FIF_JPEG:
				case FREE_IMAGE_FORMAT.FIF_PNG:
				//case FREE_IMAGE_FORMAT.FIF_TIFF:
				return false;
				// Skyboxes and lightmaps,and so on.
				case FREE_IMAGE_FORMAT.FIF_EXR:
				case FREE_IMAGE_FORMAT.FIF_HDR:
				return false;
				default:
					fif=FREE_IMAGE_FORMAT.FIF_PNG;
				return true;
			}
		}

		public override void OnPostImport(string path) {
			if(filter!=null&&!filter(path)) {return;}
			FREE_IMAGE_FORMAT fif=FreeImage.GetFileType(path,0);
			if(fif==FREE_IMAGE_FORMAT.FIF_UNKNOWN) {return;}
			//
			FIBITMAP dib=FreeImage.LoadEx(path,ref fif);
			if(!dib.IsNull) {
				byte b=0x0;
				int w=(int)FreeImage.GetWidth(dib);
				int h=(int)FreeImage.GetHeight(dib);
				//
				if(CanConvert(ref fif)) {
					b|=0x1;
					string tmp=path;
					path=Path.ChangeExtension(path,"."+FreeImage.GetFIFExtensionList(fif).Split(',')[0]);
					//
					File_ChangeExtension(tmp,path);
				}
				if(maxSize>=0) {
				if(w>=h) {
					if(w>maxSize) {
						b|=0x2;
						h=maxSize*h/w;
						w=maxSize;
					}
				}else {
					if(h>maxSize) {
						b|=0x2;
						w=maxSize*w/h;
						h=maxSize;
					}
				}}
				//
				if(b!=0x0) {
					if((b&0x2)!=0) {
						var tmp=FreeImage.Rescale(dib,w,h,FREE_IMAGE_FILTER.FILTER_BILINEAR);
						FreeImage.UnloadEx(ref dib);dib=tmp;
					}
					FreeImage.SaveEx(dib,path,fif);
				}
				//
				FreeImage.UnloadEx(ref dib);
			}
		}

		#endregion Methods
	}
}
