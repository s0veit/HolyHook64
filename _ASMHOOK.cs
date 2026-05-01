using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace HolyHook
	{
	public class _ASMHOOK
		{
		//no more tables and no more pinvokes
		private unsafe delegate* unmanaged[Stdcall]<IntPtr, uint,uint,uint, IntPtr> _VirtualAlloc;
		private unsafe delegate* unmanaged[Stdcall]<IntPtr, UIntPtr, uint, uint*, bool> _VirtualProtect;
		private const uint MEM_COMMIT = 0x1000;
		private const uint MEM_RESERVE = 0x2000;
		private const uint PAGE_EXECUTE_READWRITE = 0x40;

		private const int JMP_SIZE = 12;

		public unsafe _ASMHOOK()
		{
			IntPtr kernel32 = NativeLibrary.Load("kernel32.dll");
			IntPtr v_alloc_ptr = NativeLibrary.GetExport(kernel32, "VirtualAlloc");
			IntPtr v_protect_ptr = NativeLibrary.GetExport(kernel32, "VirtualProtect");

			_VirtualAlloc = (delegate* unmanaged[Stdcall]<IntPtr, uint,uint,uint, IntPtr>)v_alloc_ptr;
			_VirtualProtect = (delegate* unmanaged[Stdcall]<IntPtr, UIntPtr, uint, uint*, bool>)v_protect_ptr;
		}

		public unsafe IntPtr Install(IntPtr target_address, IntPtr hook_func_address, int bytes_2_steal){
			//well make the game jmp back in order to ensure some stability? perhaps? -> update: yes it works
			IntPtr trampoline_address = _VirtualAlloc(IntPtr.Zero,(uint)(bytes_2_steal + JMP_SIZE),MEM_COMMIT | MEM_RESERVE,PAGE_EXECUTE_READWRITE);
			//steal the original instructions from the engine and insert into our trampoline afterwards
			byte* target_ptr = (byte*)target_address;
			byte* trampoline_ptr = (byte*)trampoline_address;
			Unsafe.CopyBlockUnaligned(trampoline_ptr, target_ptr, (uint)bytes_2_steal);	//IL cpblk instruct

			//now we build a pseudo-_asm return jmp to the function + b2s
			//csharp has no asm blocks(dambasses) so this is the best i cna do
			// 48 B8 === 8bytes of pointer === FF E0
			// MOV RAX, ptr, JMP RAX
			byte* return_jmp= stackalloc byte[]{
			0x48, //sets instruct to x64 operand length
			0xB8, //MOV RAX opcodew -> imm64 (move 64bit immediate to RAX reg)
			0,0,0,0,0,0,0,0, //pointer bytes -> 1(little endian, pretty cool, research if unknown) - 8; overwritten by bitconvert
			0xFF,//indirect waiter opcode for E0
			0xE0// JMP RAX
			};
			IntPtr return_address = target_address+bytes_2_steal;
			*(long*)(return_jmp+2) = return_address.ToInt64();//writes rtrnjmp right after the stealage in trampoline, 8 byte address
			//directly into the alloc at idx 2
			Unsafe.CopyBlockUnaligned(trampoline_ptr + bytes_2_steal, return_jmp, JMP_SIZE);

			//build hookjmp - engine->delegate
			//MOV RAX, ptr, JMP RAX, explained in returnjmp for more details
			byte* hook_jmp = stackalloc byte[] { 0x48,0xB8,0,0,0,0,0,0,0,0,0xFF,0xE0 };
			*(long*)(hook_jmp+2) = hook_func_address.ToInt64();

			//build patch and insert nop instructs into the remnant tail
			byte* p = stackalloc byte[bytes_2_steal];
			Unsafe.InitBlockUnaligned(p,0x90, (uint)bytes_2_steal);//native memset
			Unsafe.CopyBlockUnaligned(p, hook_jmp,JMP_SIZE);

			//finally, hook into engine
			uint old;
			_VirtualProtect(target_address,(UIntPtr)bytes_2_steal, PAGE_EXECUTE_READWRITE, &old);
			Unsafe.CopyBlockUnaligned(target_ptr,p,(uint)bytes_2_steal);
			_VirtualProtect(target_address, (UIntPtr)bytes_2_steal, old, &old);

			return trampoline_address;
			}

		}
	}
