using System.IO;
using FreeImageAPI;

namespace UnityPackageTool.Postprocessors {
	public class TexturePostprocessor
		:Postprocessor
	{
		#region Fields

		public int maxSize=1024;

		#endregion Fields

		#region Methods

		public TexturePostprocessor(Importer importer):base(importer) {
		}

		public override void OnPostImport(string path) {
			FREE_IMAGE_FORMAT fif=FreeImage.GetFileType(path,0);
			if(fif==FREE_IMAGE_FORMAT.FIF_UNKNOWN) {return;}
			//
			FIBITMAP dib=FreeImage.LoadEx(path,ref fif);
			if(!dib.IsNull) {
				byte b=0x0;
				int w=(int)FreeImage.GetWidth(dib);
				int h=(int)FreeImage.GetHeight(dib);
				//
				if(fif!=FREE_IMAGE_FORMAT.FIF_JPEG&&fif!=FREE_IMAGE_FORMAT.FIF_PNG) {
					b|=0x1;
					string tmp=path;
					path=Path.ChangeExtension(path,".png");
					//
					File.Delete(tmp);
					tmp=tmp+".meta";
					if(File.Exists(tmp)) {
						File.Move(tmp,path+".meta");
					}
				}
				if(w>=h) {
					if(w>maxSize) {
						b|=0x2;
						h=maxSize*w/h;
						w=maxSize;
					}
				}else {
					if(h>maxSize) {
						b|=0x2;
						w=maxSize*h/w;
						h=maxSize;
					}
				}
				//
				if(b!=0x0) {
					if((b&0x2)!=0) {
						var tmp=FreeImage.Rescale(dib,w,h,FREE_IMAGE_FILTER.FILTER_BILINEAR);
						FreeImage.UnloadEx(ref dib);dib=tmp;
					}
					FreeImage.SaveEx(dib,path);
				}
				//
				FreeImage.UnloadEx(ref dib);
			}
		}

		#endregion Methods
	}
}
