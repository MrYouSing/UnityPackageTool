-- 
clr.System.Reflection.Assembly:Load("UnityPackageTool");

local UPT=clr.UnityPackageTool;
local src="C:/Users/Administrator/AppData/Roaming/Unity/Asset Store-5.x/Quang Phan/3D ModelsCharactersHumanoidsFantasy/BerserkerS2 Mage and Archer.unitypackage";
local dst="D:/Documents/Unity Projects/Proj_Imp";

do 
	-- 
	pkg=UPT.Package();
	importer=UPT.Importer();
	texture=UPT.Postprocessors.TexturePostprocessor(importer);
	-- 
	pkg.cacheSize=200*1024*1024;
	texture.maxSize=1024;
	-- 
	importer.Import(src,dst,pkg);
end;