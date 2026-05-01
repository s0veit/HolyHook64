using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;


// playerstate_t -> 0x7B4000
// HP: 0x7B40B8
// Ground Flag: 0x7B4044
// CL_CreateCMD: offset -> ? -> 0xF410 found
//				 byte count -> ? -> 12 found
namespace HolyHook
	{
	public class EntryPoint
		{
		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void CreateCmdDelegate(IntPtr cmd_out);
		private static CreateCmdDelegate _detour;
		private static CreateCmdDelegate _original;
		private static IntPtr _exe_base;

		[UnmanagedCallersOnly(EntryPoint = "Attach")]
		public static void Attach()
			{
			Thread thread = new Thread(Initialize);
			thread.IsBackground = true;
			thread.Priority = ThreadPriority.Highest;
			thread.Start();
			}
		private static unsafe void Initialize()
			{
			Thread.Sleep(2000);

			if(AllocConsole())
				{
				Console.WriteLine("=======================================================");
				Console.WriteLine("    HOLYHOOK INTERNAL - PURE C# MAGNIFICENCE    ");
				Console.WriteLine("	   Status: ATTACHED AND RUNNING WITH NO SHITAPI");
				Console.WriteLine("=======================================================");
				_exe_base = Process.GetCurrentProcess().MainModule.BaseAddress; // this is shit
				IntPtr target_address = _exe_base + _OFFSETS._FUNCS._CL_CREATECMD; //replace with found offset

				_detour = new CreateCmdDelegate(HookedCreateCmd);
				IntPtr hook_ptr = Marshal.GetFunctionPointerForDelegate(_detour);//mutilates gcs intestines, otherwise gc mutilates our cockage, baically? memory pin
				_ASMHOOK hooker = new();
				int bytes_2_steal = _OFFSETS._FUNCS._CL_CREATECMD_BYTES_2_STEAL;
				IntPtr trampoline_address = hooker.Install(target_address, hook_ptr, bytes_2_steal);
				_original = Marshal.GetDelegateForFunctionPointer<CreateCmdDelegate>(trampoline_address);
				Console.WriteLine("hooked");
				}
			}

		private static unsafe void HookedCreateCmd(IntPtr cmd_out)
			{
				_original(cmd_out);
				Structs.usercmd_t* cmd = (Structs.usercmd_t*)cmd_out;
				int ground_state = *(int*)(_exe_base+_OFFSETS._ENGINE_PTRS._GROUND_FLAG);
				bool is_grounded = (ground_state != _OFFSETS._CONSTS._ENTITYNUM_NONE);
				bool is_jump_held = (cmd->upmove > 0);
				if(is_jump_held)//proper perfect 1tick bunnyhop would be done via jumpTime within the src but this serves well for what i wanted to do
				{
					if(is_grounded)
						{
							cmd->upmove = _OFFSETS._CONSTS._UPMOVE_MAX; // this is how quake processes jumping, very dfferent from source
						}
					else
						{
							cmd->upmove=0; //reset engine, dont spam hop in air
						}
				}
			}
		}

	}
		
