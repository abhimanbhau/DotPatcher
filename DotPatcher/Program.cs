/* Copyleft 2015 Abhimanbhau Kolte
 * The file is part of DotPatcher project and is licensed under MIT License
 * 
 * @TODO
 * # Injection Demos -> Do pseudo malacious tasks
 * Inject protection -> part of DotProtect project
 * 
 * */


using System;
using System.IO;

namespace DotPatcher
{
	public class Program
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("DotPatcher v1.0.1b");
			Console.WriteLine ("Enter exe path to be patched : ");
			string exePath = Console.ReadLine ();
			if (!File.Exists (exePath))
				throw new FileNotFoundException ();
			if (args.Length > 0) {
				if (args [0] == "oneshot") {
					Console.WriteLine ("Patching the exe, 'oneshot'-way");
					PatchTools.patchGeneric (true, exePath, "DotPatcher - OneShot patch");
					PatchTools.patchGeneric (false, exePath, "DotPatcher - OneShot patch");
					PatchTools.wipeAssembly (exePath);
				}
			}else {
				try {
					Console.WriteLine ("Patching Methods\n1. Inject Custom Message" +
					"\n2. Inject Popup message\n3. Wipe out whole assembly\n4. Malware Patch");
					short patchMethod = Int16.Parse (Console.ReadLine ());

					Console.WriteLine ("Enter custom message : ");
					string message = Console.ReadLine ();

					switch (patchMethod) {
					case 1:
						PatchTools.patchGeneric (true, exePath, message);
						break;

					case 2:
						PatchTools.patchGeneric (false, exePath, message);
						break;

					case 3:
						PatchTools.wipeAssembly (exePath);
						break;

					case 4:
						PatchTools.patchMalware(exePath);
						break;
					}
					Console.WriteLine ("\nAbhimanbhau Kolte (c)CopyLeft 2015");
				} catch (Exception ex) {
					File.WriteAllText ("crash-" + DateTime.Now.ToLongDateString () + ".log", 
						ex.Message + "\n" + ex.StackTrace);
				}
			}
		}
	}
}