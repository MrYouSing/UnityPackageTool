namespace UnityPackageTool {
	public abstract class Postprocessor {
		#region Methods

		public Postprocessor(Importer importer) {
			importer.onPostImport+=OnPostImport;
		}

		public abstract void OnPostImport(string path);

		#endregion Methods
	}
}
