
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

public class TextureConverter
{
    public static async Task ConvertDDSToDXT1( string path )
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathNVTTEExecutive}";
        startInfo.Arguments = $"-o \"{path}\" -f bc1 \"{path}\"";

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        await Task.Run( () => process.WaitForExit() );
    }

    public static bool IsDXT1( string path )
    {
        using ( var fileStream = File.OpenRead( path ) )
        using ( var binaryReader = new BinaryReader( fileStream ) )
        {
            binaryReader.BaseStream.Seek( 84, SeekOrigin.Begin );
            var fourCc = binaryReader.ReadChars( 4 );
            string format = new string( fourCc );

            return format == "DXT1";
        }
    }

    public static async Task ResizeTextures( string path = null )
    {
        path = string.IsNullOrEmpty( path ) ? UnityInfo.relativePathMaterials : path;

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = $"{UnityInfo.currentDirectoryPath}/{UnityInfo.relativePathTexConv}";
        startInfo.Arguments = $"-nologo -bc dx -w 512 -h 512 -m 1 -f BC1_UNORM -o \"{path}\" -r \"{path}/*.dds\" -y";

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        await Task.Run( () => process.WaitForExit() );
    }
}