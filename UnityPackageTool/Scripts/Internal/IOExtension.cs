using System;
using System.IO;
using ICSharpCode.SharpZipLib.Tar;

namespace UnityPackageTool {
	public static class IOExtension {

		// Taken from https://github.com/Lachee/Unity-Package-Exporter/blob/master/UnityPackageExporter/Program.cs
		public static long ReadNextFile(this TarInputStream tarIn,Stream outStream) {
			long totalRead = 0;
			byte[] buffer = new byte[4096];
			bool isAscii = true;
			bool cr = false;

			int numRead = tarIn.Read(buffer,0,buffer.Length);
			int maxCheck = Math.Min(200,numRead);

			totalRead+=numRead;

			for(int i = 0;i<maxCheck;i++) {
				byte b = buffer[i];
				if(b<8||(b>13&&b<32)||b==255) {
					isAscii=false;
					break;
				}
			}

			while(numRead>0) {
				if(isAscii) {
					// Convert LF without CR to CRLF. Handle CRLF split over buffers.
					for(int i = 0;i<numRead;i++) {
						byte b = buffer[i];     // assuming plain Ascii and not UTF-16
						if(b==10&&!cr)     // LF without CR
							outStream.WriteByte(13);
						cr=(b==13);

						outStream.WriteByte(b);
					}
				} else
					outStream.Write(buffer,0,numRead);

				numRead=tarIn.Read(buffer,0,buffer.Length);
				totalRead+=numRead;
			}

			return totalRead;
		}

		public static void File_WriteAllBytes(string path,byte[] bytes,DateTime dateTime) {
			File.WriteAllBytes(path,bytes);
			File.SetLastWriteTime(path,dateTime);
		}

		public static void File_Move(string src,string dst) {
			if(File.Exists(dst)) {
				File.Delete(dst);
			}
			File.Move(src,dst);
		}

		public static void File_ChangeExtension(string src,string dst) {
			File.Delete(src);
			src=src+".meta";
			if(File.Exists(src)) {
				File_Move(src,dst+".meta");
			}
		}

		public static bool IsMetaFile(this string thiz) {
			return thiz.EndsWith(".meta",System.StringComparison.OrdinalIgnoreCase);
		}
	}
}
