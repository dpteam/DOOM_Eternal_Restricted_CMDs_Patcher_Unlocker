using System;
using System.IO;
using System.Reflection;

namespace DOOM_Eternal_Restricted_CMDs_Patcher_Unlocker
{
	public static class Program
	{
		static void Main(string[] args)
		{
			string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string doomexe = "DOOMEternalx64vk.exe";
			if (File.Exists(doomexe + ".devcmds-unlocker-bak"))
			{
				Console.WriteLine("Backup found. Already patched?");
				Console.ReadKey();
			}
			File.Copy(assemblyPath + Path.DirectorySeparatorChar + doomexe, assemblyPath + Path.DirectorySeparatorChar + doomexe + ".devcmds-unlocker-bak", false);
			Console.WriteLine("Backup created");
			byte[] sourceBytes = StringHexToByteArray("CCCCCCCCCCCCCCCC40534883EC50488B842488000000488B");
			byte[] targetBytes = StringHexToByteArray("4183C91053EB0390EBF64883EC50488B842488000000488B");
			BinaryReplace(doomexe + ".devcmds-unlocker-bak", sourceBytes, doomexe, targetBytes);
			Console.WriteLine("Pattern found and replaced. DOOM Eternal patched successful.");
			Console.ReadKey();
		}

		public static void BinaryReplace(string sourceFile, byte[] sourceSeq, string targetFile, byte[] targetSeq)
		{
			FileStream sourceStream = File.OpenRead(sourceFile);
			FileStream targetStream = File.Create(targetFile);

			try
			{
				int b;
				long foundSeqOffset = -1;
				int searchByteCursor = 0;

				while ((b = sourceStream.ReadByte()) != -1)
				{
					if (sourceSeq[searchByteCursor] == b)
					{
						if (searchByteCursor == sourceSeq.Length - 1)
						{
							targetStream.Write(targetSeq, 0, targetSeq.Length);
							searchByteCursor = 0;
							foundSeqOffset = -1;
						}
						else
						{
							if (searchByteCursor == 0)
							{
								foundSeqOffset = sourceStream.Position - 1;
							}

							++searchByteCursor;
						}
					}
					else
					{
						if (searchByteCursor == 0)
						{
							targetStream.WriteByte((byte)b);
						}
						else
						{
							targetStream.WriteByte(sourceSeq[0]);
							sourceStream.Position = foundSeqOffset + 1;
							searchByteCursor = 0;
							foundSeqOffset = -1;
						}
					}
				}
			}
			finally
			{
				sourceStream.Dispose();
				targetStream.Dispose();
			}
		}

		public static byte[] StringHexToByteArray(String hex)
		{
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
			bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}
	}
}
