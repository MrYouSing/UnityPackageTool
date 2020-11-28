using System.IO;
using NAudio;
using NAudio.Wave;
using static UnityPackageTool.IOExtension;

namespace UnityPackageTool.Postprocessors {
	public class AudioClipPostprocessor
		:Postprocessor
	{
		#region Fields

		public bool mp3ToWav=true;
		public bool wavToOgg=true;

		#endregion Fields

		#region Methods

		public AudioClipPostprocessor(Importer importer):base(importer) {
		}

		public override void OnPostImport(string path) {
			if(mp3ToWav&&Path.HasExtension(".mp3")) {
				using(Mp3FileReader reader=new Mp3FileReader(path)) {
					string tmp=path;
					path=Path.ChangeExtension(path,".wav");
					WaveFileWriter.CreateWaveFile(path,reader);
					File_ChangeExtension(tmp,path);
				}
			}
			if(wavToOgg&&Path.HasExtension(".wav")) {
				// TODO: To Ogg????
				System.Console.WriteLine("new System.NotImplementedException()");
			}
		}

		#endregion Methods
	}
}
